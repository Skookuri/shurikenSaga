using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Color originalColor;
    [SerializeField]
    private float chargeAmp;

    private bool pds = true;

    [SerializeField]
    private startBoss sb;

    // Start is called before the first frame update
    void Start()
    {
        //sb = GameObject.Find("beginZone").GetComponent<startBoss>();
        player = GameObject.Find("player").transform;
        originalColor = gameObject.GetComponent<SpriteRenderer>().color;
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
            rb.AddForce(chargeDir * chargeAmp, ForceMode2D.Impulse);
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
