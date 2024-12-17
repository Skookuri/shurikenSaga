using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossBehavior : MonoBehaviour
{
    //public startBoss sb;
    Transform player;
    [SerializeField]
    public float detRange;
    private float timePassedInAggro;
    [SerializeField]
    private float sideFrequency;
    [SerializeField]
    private float verticalFrequency;
    [SerializeField]
    private float sideAmp;
    [SerializeField]
    private float verticalAmp;
    [SerializeField]
    private float timeUntilCharge;
    [SerializeField]
    private float cooldown;
    private Vector2 randomTarget;
    private bool pickFloatDir;
    public float randomOffsetRadius;

    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    private float floatForce;
    [SerializeField]
    private float chargeInterval;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Color originalColor;

    [SerializeField]
    private Color dashColor;
    [SerializeField]
    private float chargeAmp;

    private bool pds = true;

    private float minionSpawnSpeed = 0f;
    private float dashInterval = 0f;
    private bool dashed = false;

    private bool stopDash = false;

    [SerializeField]
    GameObject minion;

    [SerializeField]
    private startBoss sb;

    private bool firstRun = true;

    Vector2 playerDirection;

    private bool pickedPlayerDir = false;

    // Start is called before the first frame update
    void Start()
    {
        //sb = GameObject.Find("beginZone").GetComponent<startBoss>();
        player = GameObject.Find("player").transform;
        //originalColor = GetComponent<SpriteRenderer>().color;
    }


    private void Update()
    {
        if (!sb.startFinalBoss)
        {
            return;
        }

        minionSpawnSpeed += Time.deltaTime;
        dashInterval += Time.deltaTime;

        if (minionSpawnSpeed > 14f)
        {
            minionSpawnSpeed = 0f;
            StartCoroutine(MinionSpawner());
        }

        if (dashInterval > 6f)
        {
            if (!pickedPlayerDir && dashInterval > 6.9f)
            {
                pickedPlayerDir = true;
                playerDirection = (player.position - gameObject.transform.position).normalized;

            }
            if ((dashInterval % .08f) < .04f){
                spriteRenderer.color = dashColor;
            } else
            {
                spriteRenderer.color = originalColor;
            }

            if(dashInterval > 7.2f && !dashed)
            {
                dashed = true;
                rb.AddForce(playerDirection * chargeAmp, ForceMode2D.Impulse);
            }

            if (dashInterval > 7.48f && !stopDash)
            {
                stopDash = true;
                rb.velocity = rb.velocity / 5;
            }

            if (dashInterval > 8.5f)
            {
                dashed = false;
                stopDash = false;
                pickedPlayerDir = false;
                dashInterval = 0f;
                spriteRenderer.color = originalColor;
            }
        }
    }

    public IEnumerator MinionSpawner()
    {
        int i = 0;
        while (i < 3)
        {
            SpawnMinion();
            yield return new WaitForSeconds(1);
            i++;
        }
    }

    private void SpawnMinion()
    {
        int i = 0;
        while (i < 1)
        {
            GameObject spawnedMinion = Instantiate(minion, transform.position, Quaternion.identity);
            Renderer renderer = spawnedMinion.GetComponent<Renderer>();
            Material newMaterial = null;

            if (renderer != null)
            {
                newMaterial = new Material(renderer.material); // Clone the material
                renderer.material = newMaterial; // Assign the unique material to the Renderer
            }

            GhostBehavior ghostBehavior = spawnedMinion.GetComponent<GhostBehavior>();
            if (ghostBehavior != null && newMaterial != null)
            {
                ghostBehavior.detRange = 100f;
                ghostBehavior.aggroDetRange = 100f;
                ghostBehavior.material = newMaterial; // Call a method to set the material
            }

            Debug.Log("Spawned minion at position: " + transform.position);
            i++;
        }
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (sb.startFinalBoss)
            Aggro();
        //GameObject.Find("BackgroundMusic").GetComponent<BGSoundScript>().audioSource.Play();
    }

    private bool isClockwise = true; // Tracks the current movement direction

    private void Aggro()
    {
        timePassedInAggro += Time.deltaTime;
        timeUntilCharge += Time.deltaTime;

        // Timer for random speed adjustments or when direction changes
        if (timePassedInAggro >= cooldown || pickFloatDir)
        {
            CalculateNewTarget();
            pickFloatDir = false;
            timePassedInAggro = 0f;

            // Randomly adjust speed (slow down or speed up)
            floatForce = Random.Range(0.5f, 5.0f); // Adjust the range as needed for noticeable speed changes
        }

        // Calculate direction and apply force for smooth movement
        Vector2 direction = (randomTarget - (Vector2)transform.position).normalized;
        rb.AddForce(direction * floatForce, ForceMode2D.Force);

        // Handle charging behavior
        if (timeUntilCharge >= chargeInterval)
        {
            spriteRenderer.color = originalColor;
            timeUntilCharge = 0f;

            // Direct charge toward the player
            Vector2 chargeDir = (player.position - transform.position).normalized;
            //rb.AddForce(chargeDir * chargeAmp, ForceMode2D.Impulse);
        }

        // Check if the boss reached the random target
        if (Vector2.Distance(transform.position, randomTarget) < 0.5f)
        {
            pickFloatDir = true;
        }

        // Randomly reverse direction
        if (Random.value < 0.01f && pds == true)
        {
            isClockwise = !isClockwise;
            pickFloatDir = true; // Ensure new target is recalculated
            Debug.Log("changed directions");
            StartCoroutine(WaitToSwitch());
        }
    }

    private void CalculateNewTarget()
    {
        // Calculate a direction vector from the player to the boss
        Vector2 directionToBoss = (Vector2)transform.position - (Vector2)player.position;

        // Normalize the direction to get a unit vector
        directionToBoss.Normalize();

        // Generate a random angle offset that avoids the directionToBoss line
        float randomAngle = isClockwise ? Random.Range(0, Mathf.PI / 4) : Random.Range(-Mathf.PI / 4, 0); // Clockwise or counterclockwise range
        Vector2 perpendicularOffset = new Vector2(-directionToBoss.y, directionToBoss.x); // Perpendicular to directionToBoss

        // Rotate the perpendicular offset by the random angle
        Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle * Mathf.Rad2Deg) * perpendicularOffset;

        // Scale the offset by the desired radius
        Vector2 randomOffset = randomDirection * randomOffsetRadius;

        // Set the target as a position near the player but avoiding a direct line
        randomTarget = (Vector2)player.position + randomOffset;
    }

    private IEnumerator WaitToSwitch()
    {
        pds = false;
        yield return new WaitForSeconds(5);
        pds = true;
    }
}
