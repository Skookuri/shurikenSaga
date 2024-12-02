using System.Collections;
using UnityEngine;

public class KozouBehavior : MonoBehaviour
{
    private float maxPatrolDist = 1000;
    [SerializeField]
    public float detRange;
    private float timeInSight;
    [SerializeField]
    private float maxTimeInSight;
    [SerializeField]
    private Vector2[] movementPattern;
    [SerializeField]
    private float moveSpeed; // Speed of movement
    [SerializeField]
    private float pauseTime; // Time to pause at each location

    private Transform player; // Reference to the player
    private int index = 0;
    private bool pickTarget = true;
    private Vector2 currTargetPos;
    private Vector2 basePosition;
    private bool isPatrolling = false;
    private bool cantMove = false;
    private Coroutine moveCoroutine; // Reference to the move coroutine
    private int numberInvalidPos = 0;

    public SpriteRenderer spriteRenderer;
    public Color blinkColor;
    public Color originalColor;
    private float nextBlinkTime = 0f;
    private bool lightSpotted = false;
    //private float elapsedTimeLooking = 0f;

    public Vector2 lastKnownPosition; // Player's last known position
    private bool movingToLastKnownPosition = false; // Flag for moving to the last known position

    private bool caught = false;

    private GameHandler gh;

    public bool hurt = false;
    private BasicEnemyValues b;

    void Start()
    {
        b = GetComponent<BasicEnemyValues>();
        originalColor = spriteRenderer.color;
        basePosition = transform.position;
        player = GameObject.Find("player").transform;
        gh = GameObject.Find("GameHandler").GetComponent<GameHandler>();
    }

    void Update()
    {
        if (!caught)
        {
            if (gh.resetLinkedKozous)
            {
                Debug.Log("resetting kozous");
                StopAllCoroutines();
                pickTarget = true;
                cantMove = false;
                lightSpotted = false;
                isPatrolling = false;
                movingToLastKnownPosition = false;
                basePosition = transform.position;
                gh.resetLinkedKozous = false;
            }
            // Check distance to player
            float playerDistance = Vector2.Distance(transform.position, player.position);
            
            if (playerDistance <= detRange)
            {
                // If player is in range
                if (!lightSpotted)
                {
                    if (isPatrolling && moveCoroutine != null)
                    {
                        StopCoroutine(moveCoroutine); // Stop the move coroutine
                        moveCoroutine = null;
                        MoveToLastKnownPosition();
                    }
                    isPatrolling = false;
                    pickTarget = false;

                    lightSpotted = true;
                }

                lastKnownPosition = player.position; // Update player's last known position
                HandleDetection(); // Handle detection logic
            }
            else if (lightSpotted && !movingToLastKnownPosition)
            {
                spriteRenderer.color = originalColor;
                timeInSight = 0f;
                // Player is out of range, move toward last known position
                MoveToLastKnownPosition();
            }
            else if (movingToLastKnownPosition)
            {
                spriteRenderer.color = originalColor;
                timeInSight = 0f;
                // Check if the enemy has reached the last known position
                if (Vector2.Distance(transform.position, lastKnownPosition) < 0.1f)
                {
                    movingToLastKnownPosition = false;
                    lightSpotted = false; // Reset detection
                    pickTarget = true; // Resume patrol
                }
            }
            else if (pickTarget && !cantMove)
            {
                timeInSight = 0f;
                // Continue normal patrol behavior
                SelectTarget();
            }
        }
        else
        {
            Debug.Log("lmao u r caught ez get good L");
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            gh.killPlayer();
        }
    }

    public IEnumerator hurtFlash()
    {
        //float knockbackDuration = 0.3f; // Duration of knockback
        hurt = true;
        spriteRenderer.color = b.hurtColor;
        yield return new WaitForSeconds(.4f);

        spriteRenderer.color = originalColor;
        hurt = false;
    }

    public void ResetBools()
    {
        pickTarget = true;
        cantMove = false;
        lightSpotted = false;
        isPatrolling = false;
        movingToLastKnownPosition = false;
    }

    public void MoveToLastKnownPosition()
    {
        movingToLastKnownPosition = true;

        // Stop any ongoing patrol coroutine
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        // Start moving to the last known position
        moveCoroutine = StartCoroutine(MoveToPosition(lastKnownPosition));
    }

    private void SelectTarget()
    {
        pickTarget = false;

        if (movementPattern.Length != 0)
        {
            currTargetPos = basePosition + movementPattern[index];
        }
        else
        {
            Debug.LogError("Movement pattern list is empty!");
            cantMove = true;
            return;
        }

        if (Vector2.Distance(basePosition, currTargetPos) < maxPatrolDist)
        {
            isPatrolling = true;
            moveCoroutine = StartCoroutine(MoveToPosition(currTargetPos));
        }
        else
        {
            Debug.LogWarning($"Invalid patrol location at index {index}");
            numberInvalidPos += 1;
            if (numberInvalidPos > movementPattern.Length)
            {
                Debug.Log("resetting kozou");
                basePosition = transform.position;
            }
            pickTarget = true;
        }
    }

    private IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            if (Vector2.Distance(transform.position, player.position) < detRange)
            {
                targetPosition = player.position;
            }
            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
            yield return null;
        }

        // If moving to patrol position, pause and then go to the next target
        if (isPatrolling)
        {
            yield return new WaitForSeconds(pauseTime);
            index = (index + 1) % movementPattern.Length; // Wrap around to the beginning
            pickTarget = true;
        }

        isPatrolling = false;
        movingToLastKnownPosition = false; // Ensure movement to last known position ends here
    }

    private void HandleDetection()
    {
        timeInSight += Time.deltaTime;

        // Ensure timeInSight does not exceed the cap
        timeInSight = Mathf.Min(timeInSight, maxTimeInSight);

        // Calculate the blink interval based on the proportion of timeInSight to timeBeforeBlinking
        float blinkInterval = Mathf.Lerp(0.2f, 0.04f, timeInSight / maxTimeInSight);

        // Check if it's time to blink
        if (Time.time >= nextBlinkTime)
        {
            // Toggle between blinkColor and originalColor
            if (!hurt)
            {
                spriteRenderer.color = spriteRenderer.color == originalColor ? blinkColor : originalColor;
            }

            // Set the next blink time based on the current blink interval
            nextBlinkTime = Time.time + blinkInterval;
        }

        // Reset the color to original if timeInSight exceeds timeBeforeBlinking
        if (timeInSight >= maxTimeInSight)
        {
            spriteRenderer.color = blinkColor;
            caught = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            if (!hurt)
            {
                spriteRenderer.color = blinkColor;
            }
            caught = true;
        }
    }
}