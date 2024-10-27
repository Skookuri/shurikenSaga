using UnityEngine;

public class EnemyPatrolHit : MonoBehaviour {

    public float speed = 2f;
    public LayerMask wallLayer;
    public float rayLength = 0.5f;
    public int damage = 10;
    private bool faceRight = true;
    private GameHandler gameHandler;

    void Start() {
        if (GameObject.FindWithTag("GameHandler") != null) {
            gameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
        }
    }

    void Update() {
        // Move enemy in the current facing direction
        float moveDirection = faceRight ? 1 : -1;
        transform.Translate(Vector2.right * moveDirection * speed * Time.deltaTime);

        // Raycast to detect wall directly in front of the enemy
        Vector2 rayOrigin = transform.position + new Vector3(moveDirection * 0.5f, 0, 0);
        Vector2 direction = faceRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayLength, wallLayer);
        Debug.DrawRay(rayOrigin, direction * rayLength, Color.green);

        // If wall is detected, flip direction
        if (hit.collider != null) {
            faceRight = !faceRight; // Toggle the movement direction
            UpdateOrientation(); // Adjust the visual orientation
        }
    }

    private void UpdateOrientation() {
        // Set localScale based on faceRight to flip the enemy
        transform.localScale = new Vector3(faceRight ? 1 : -1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            gameHandler.playerGetHit(damage);
        }
    }
}
