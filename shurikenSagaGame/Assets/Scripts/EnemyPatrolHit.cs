using UnityEngine;

public class EnemyPatrolHit : MonoBehaviour {

    public float speed = 2f;
    private bool movingRight = true;

    void Update() {
        // Move in the current direction
        float moveDirection = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * moveDirection * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // Reverse direction on collision
        movingRight = !movingRight;
        Flip();
    }

    private void Flip() {
        // Flip the enemy by reversing the X scale
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
