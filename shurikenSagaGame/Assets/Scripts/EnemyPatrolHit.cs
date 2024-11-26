using UnityEngine;

public class EnemyPatrolHit : MonoBehaviour {
    public float speed = 2f;
    public float sightRange = 5f; // The distance the enemy can see the player
    public float damageRange = 1f; // The distance at which the enemy can attack the player
    public int damage = 10; // Damage dealt to the player
    private GameHandler gameHandler; // Reference to GameHandler for player health management

    public Transform player; // Reference to the player's transform
    private bool chasingPlayer = false; // Track if the enemy is chasing the player

    // Define the target return position
    private Vector2 returnPosition = new Vector2(1f, 2.5f);

    void Start() {
        // Find the GameHandler to manage player health
        GameObject gameHandlerObject = GameObject.FindWithTag("GameHandler");
        if (gameHandlerObject != null) {
            gameHandler = gameHandlerObject.GetComponent<GameHandler>();
            if (gameHandler == null) {
                Debug.LogError("GameHandler found but component is null.");
            }
        } else {
            Debug.LogError("No GameHandler found with the tag 'GameHandler'.");
        }
    }

    void Update() {
        // Check if the player is within sight range
        if (player != null) {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer < sightRange) {
                // The enemy sees the player
                chasingPlayer = true; // Start chasing the player
                speed = 2.7f;
            } else {
                chasingPlayer = false; // Stop chasing if player is out of sight
                speed = 2.0f;

                ReturnToCoordinate(returnPosition); // Return to the defined coordinates
                Patrol();
            }

            if (chasingPlayer) {
                ChasePlayer();

                // Check if the enemy can attack the player
                if (distanceToPlayer < damageRange) {
                    //yield return new WaitForSeconds(0.5);
                    AttackPlayer(distanceToPlayer);
                }
            }
        }

        // Move the enemy left or right in patrol mode if not chasing
        if (!chasingPlayer) {
            Patrol();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // Reverse direction on collision
        Flip();
    }

    private void Patrol() {
        // Move in the current direction
        float moveDirection = transform.localScale.x > 0 ? 1 : -1; // Move based on the current scale
        transform.Translate(Vector2.right * moveDirection * speed * Time.deltaTime);
    }

    private void Flip() {
        // Flip the enemy by reversing the X scale
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void ChasePlayer() {
        // Move towards the player
        float step = speed * Time.deltaTime; // Calculate distance to move
        Vector2 targetPosition = new Vector2(player.position.x, player.position.y); // Move towards both x and y positions
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, step); 

        // Flip the enemy based on player position
        if (player.position.x < transform.position.x && transform.localScale.x > 0) {
            Flip(); // Flip to face left
        } else if (player.position.x > transform.position.x && transform.localScale.x < 0) {
            Flip(); // Flip to face right
        }
    }

    private void ReturnToCoordinate(Vector2 targetPosition) {
        // Move towards the defined return position
        float step = speed * Time.deltaTime; // Calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, step); 
    }

    private void AttackPlayer(float distanceToPlayer) {
        // Call the method in GameHandler to damage the player
        Debug.Log($"Distance to Player: {distanceToPlayer}, Damage Range: {damageRange}");
        if (gameHandler != null) {
            //Debug.Log("hello");
            
            gameHandler.playerGetHit();
            Debug.Log("Attacked player for " + damage + " damage!");
        } else {
            Debug.Log("GameHandler is null");
        }
    }
}
