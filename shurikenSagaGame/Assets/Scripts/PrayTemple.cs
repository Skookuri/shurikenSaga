using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayTemple : MonoBehaviour
{
    public GameObject prayText;
    public GameObject dialogueCanvas;
    public Transform player;
    private Dialoguer dialoguer;
    private ScrollDrop scrollDrop;
    private bool playerInTrigger = false; // Track if the player is in the trigger area
    private bool hasInteracted = false; // Track if the player has interacted with the object    
    private bool hasDroppedScrolls = false; // Flag to track if Scrolls have already been dropped
    //private bool notTriggered = true; //cant be triggered again

    //public bool isTemple = false;


    void Start()
    {
        prayText.SetActive(false); // Hide the button at the start
        dialogueCanvas.SetActive(false);
        scrollDrop = GetComponent<ScrollDrop>();
        if (scrollDrop == null) {
            Debug.LogWarning("ScrollDrop script not found in the scene.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering collider belongs to the player
        if (other.transform == player)
        {
            playerInTrigger = true; // Player is inside the trigger area
            prayText.SetActive(true); // Show the pray button when the player is near
            //TriggerPrayAction(); // Start prayer interaction
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the exiting collider belongs to the player
        if (other.transform == player)
        {
            playerInTrigger = false; // Player is no longer in the trigger area
            prayText.SetActive(false); // Hide the pray button when player leaves
        }
    }

    void Update()
    {
        // Check for input to trigger the prayer action when the player is in the trigger area
        if (playerInTrigger && Input.GetButtonDown("Pray") && !hasInteracted) {
            hasInteracted = true; // Mark the player as having interacted
            TriggerPrayAction(); // Start prayer interaction
        } else if (playerInTrigger && hasInteracted) {
            prayText.SetActive(false); // Hide pray text after interaction
        }
    }

    // Function to trigger the prayer interaction
    void TriggerPrayAction()
    {
        prayText.SetActive(false); // Deactivate the pray text
        dialoguer = dialogueCanvas.GetComponent<Dialoguer>();
        if (dialoguer != null) {
            dialogueCanvas.SetActive(true);
            dialoguer.StartDialogueSegment();
            // Call DropScrolls after dialogue completes (you could trigger this in the dialogue segment itself if needed)
            StartCoroutine(WaitForDialogueToEnd());
        } else {
            Debug.LogWarning("Dialoguer component is missing on the dialogueCanvas!");
        }
    }

    // Coroutine to wait for the dialogue to finish before activating the ScrollJutsu pairs
    IEnumerator WaitForDialogueToEnd()
    {
        // Wait for the dialogue to end 
        yield return new WaitUntil(() => !dialogueCanvas.activeSelf);

        // After dialogue is done, drop scrolls if they haven't been dropped yet
        if (!hasDroppedScrolls)
        {
            if (scrollDrop != null)
            {
                scrollDrop.DropScrolls(); // Activate ScrollJutsuPairs after interaction
                hasDroppedScrolls = true; // Set the flag to true to prevent repeating this action
            }
            else
            {
                Debug.LogWarning("ScrollDrop reference is missing.");
            }
        }
    }
}