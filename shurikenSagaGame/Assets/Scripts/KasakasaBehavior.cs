using System.Collections;
using UnityEngine;

public class KasakasaBehavior : MonoBehaviour
{
    Transform player;
    public float detRange; // Detection range
    private float originalDR;
    public float aggroDetRange;

    public Color blinkColor;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float timeBeforeBlinking;
    Vector2 basePosition;

    public float hopDistance = 2.5f; // Distance of each hop
    public float hopDuration = 0.5f; // Duration of each hop (shorter = faster hops)
    [Range(2f, 10f)]
    public float hopCooldown = 0.2f; // Time between hops
    public float hopHeight = 1.5f; // Maximum height of the hop arc
    private bool isHopping = false;
    [SerializeField]
    private float maxDistanceAwayWhileDormant;

    //private float timePassedInAggro = 0;
    private bool aggroActive = false;
    [SerializeField]
    private float aggroHopDistance;
    [SerializeField]
    [Range(0f, 10f)]
    private float aggroHopCooldown;
    private float OGHopDistance;
    private float OGHopCooldown;
    [SerializeField]
    public float randomOffsetRadius = 1f;
    [SerializeField]
    private float aggroJumpMult;
    //private bool hasNotBlinkedYet = true;
    private float elapsedTimeForBlinking = 0f;

    private void Start()
    {
        player = GameObject.Find("player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalDR = detRange;
        basePosition = transform.position;

        OGHopDistance = hopDistance;
        OGHopCooldown = hopCooldown;
    }

    private void Update()
    {
        float pDistance = Vector2.Distance(transform.position, player.position);
        if (pDistance > detRange)
        {
            spriteRenderer.color = originalColor;
            detRange = originalDR;
            if (aggroActive)
            {
                basePosition = transform.position;
            }
            aggroActive = false;
            hopDistance = OGHopDistance;
            hopCooldown = OGHopCooldown;
            //hasNotBlinkedYet = true;
            if (!isHopping)
                LittleHops();
        }
        else
        {
            detRange = aggroDetRange;
            aggroActive = true;
            hopDistance = aggroHopDistance;
            hopCooldown = aggroHopCooldown;
            if (!isHopping)
                LittleHops();
        }

        if (aggroActive)
        {
            elapsedTimeForBlinking += Time.deltaTime;

            // Alternate color based on time
            int blinkCount = Mathf.FloorToInt(elapsedTimeForBlinking / 0.1f);
            spriteRenderer.color = (blinkCount % 2 == 0) ? blinkColor : originalColor;
        }
    }

    private void LittleHops()
    {
        // Start the hopping coroutine
        StartCoroutine(HopRoutine());
    }

    private IEnumerator HopRoutine()
    {
        isHopping = true;

        // Generate a random direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Calculate the target position
        Vector2 startPosition = transform.position;
        Vector2 targetPosition;
        if (aggroActive)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            targetPosition = ((Vector2)transform.position + (directionToPlayer * aggroJumpMult) + Random.insideUnitCircle * randomOffsetRadius);
        } else
        {
            hopCooldown += Random.Range(-2f, 2f);
            targetPosition = (Vector2)transform.position + randomDirection * hopDistance;
        }

        // Duration of the hop
        float elapsedTime = 0f;
        if (Vector2.Distance(targetPosition, basePosition) <= maxDistanceAwayWhileDormant || aggroActive)
        {
            while (elapsedTime < hopDuration)
            {
                elapsedTime += Time.deltaTime;

                // Linear interpolation for horizontal movement
                float t = elapsedTime / hopDuration;
                Vector2 horizontalPosition = Vector2.Lerp(startPosition, targetPosition, t);

                // Parabolic equation for vertical arc
                float verticalOffset = hopHeight * Mathf.Sin(t * Mathf.PI); // Sin curve for smooth arc

                // Update the position
                transform.position = new Vector2(horizontalPosition.x, horizontalPosition.y + verticalOffset);

                yield return null;
            }
        }
        yield return null;
        // Ensure the position is set exactly to the target at the end
        if (Vector2.Distance(targetPosition, basePosition) <= maxDistanceAwayWhileDormant)
        {
            transform.position = targetPosition;
        }

        // Wait for the cooldown before the next hop
        yield return new WaitForSeconds(hopCooldown);
        isHopping = false;
    }
}