using System.Collections;
using UnityEngine;

public class MonkController : MonoBehaviour
{
    public Transform monk; // Reference to the monk GameObject's transform
    public float speed = 2f; // Speed at which the monk moves
    public static bool MoveMonk = false; // Static bool to indicate if monk should be moved
    public int targetXCoord; // Target X coordinate for monk to arrive at
    public int targetYCoord; // Target Y coordinate for monk to arrive at

    private Vector3 targetPosition; // The target position for the monk
    public AudioSource footstepSFX; //monk footsteps
    private SpriteRenderer monkRenderer;

    /*private void Awake()
    {
        // This ensures the script is running even if the monk GameObject is initially disabled.
        if (monk == null)
        {
            Debug.LogError("Monk Transform is not assigned.");
        }
    }*/

    private void Start()
    {        
        // Initialize the target position to prevent issues
        if (monk != null) {            
            targetPosition = monk.position;
            monkRenderer = monk.GetComponent<SpriteRenderer>();
            monkRenderer.enabled = false; // This hides the monk visually but doesn't disable the GameObject
        } else {
            Debug.LogError("Monk Transform is not assigned.");
        }
    }

    void Update()
    {
        if (MoveMonk) {
            if (monkRenderer != null && !monkRenderer.enabled) {
                monkRenderer.enabled = true;
                BoxCollider2D monkCollider = monk.GetComponent<BoxCollider2D>();
                monkCollider.isTrigger = false;
            }
            // Check if already at the target location
            if (AtTargetLocation()) {
                MoveMonk = false;
                //Debug.Log("Monk has reached the target location.");

                if (footstepSFX.isPlaying) {
                    footstepSFX.Stop();
                    //Debug.Log("Stopping Footstep sound..");
                }
            } else {
                // If not at target location, slowly move towards it
                MonkStartMove();
            }
        }
    }

    private void MonkStartMove()
    {
        if (monk == null) {
            Debug.LogError("Monk Transform is not assigned.");
            return;
        }

        // Move the monk towards the target position
        float step = speed * Time.deltaTime;
        monk.position = Vector3.MoveTowards(monk.position, targetPosition, step);
        //Debug.Log($"Monk moving to target - X: {monk.position.x}, Y: {monk.position.y}");
        // If moving and footstep sound isn't playing, play the sound
        if (!footstepSFX.isPlaying) {
            footstepSFX.Play();
            //Debug.Log("Playing Footstep sound..");
        }
    }

    private bool AtTargetLocation() {
        // Check if monk is at the target location
        targetPosition = new Vector3(targetXCoord, targetYCoord, monk.position.z); // Preserve Z position
        return Vector3.Distance(monk.position, targetPosition) < 0.01f;
    }
}