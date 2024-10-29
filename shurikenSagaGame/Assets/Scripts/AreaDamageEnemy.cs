using System.Collections;
using UnityEngine;

public class AreaDamageEnemy : MonoBehaviour
{
    Transform target;
    Vector2 targetCoords;
    [SerializeField]
    float waitTime = 1.0f; // Time between jumps
    [SerializeField]
    float jumpDistance = 2.0f; // Distance of each jump
    [SerializeField]
    float smoothTime = 0.3f; // Time for SmoothDamp easing

    bool moving = false;
    Vector2 velocity = Vector2.zero; // Required for SmoothDamp

    void Start()
    {
        target = GameObject.Find("player").transform;
        StartCoroutine(JumpTowardsTarget());
    }

    IEnumerator JumpTowardsTarget()
    {
        while (true)
        {
            if (moving)
            {
                // Calculate direction towards the player and target jump position
                Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
                targetCoords = (Vector2)transform.position + direction * jumpDistance;

                // Move towards the jump position with easing
                while ((Vector2)transform.position != targetCoords)
                {
                    Vector2 newPosition = Vector2.SmoothDamp(
                        transform.position,
                        targetCoords,
                        ref velocity,
                        smoothTime
                    );
                    transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                    yield return null; // Wait for the next frame
                }

                // Call Smash when the enemy reaches the target position
                Smash();

                // Start waiting for the next jump
                moving = false;
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                moving = true;
            }
        }
    }

    void Smash()
    {
        Debug.Log("Smashed");
    }
}
