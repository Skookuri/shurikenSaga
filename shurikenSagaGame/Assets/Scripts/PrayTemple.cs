using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayTemple : MonoBehaviour
{
    public GameObject prayText; // Pray button UI element
    public GameObject dialogueCanvas; // Dialogue Canvas 
    public Transform player; // player
    private Dialoguer dialoguer; // Declare a Dialoguer variable
    private bool playerInTrigger = false; // To track if the player is in the trigger area
    

    void Start()
    {
        prayText.SetActive(false); // Hide the button at the start
        dialogueCanvas.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering collider belongs to the player
        if (other.transform == player) {
            playerInTrigger = true; // Player is inside the trigger area
            prayText.SetActive(true); // Show the pray button when player is near
            Debug.Log("Player entered the trigger area.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the exiting collider belongs to the player
        if (other.transform == player) {
            playerInTrigger = false; // Player is no longer in the trigger area
            
            prayText.SetActive(false); // Hide the pray button when player leaves
            Debug.Log("Player exited the trigger area.");
        }
    }

    void Update()
    {
        // Check for input to trigger the prayer action when the player is in the trigger area
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E)) {
            TriggerPrayAction();
            Debug.Log("Triggering pray action...");
        }
    }

    // This function is called when the Pray button is clicked (or 'E' is pressed while in the trigger area)
    void TriggerPrayAction()
    {
        // Deactivate the pray button once clicked
        prayText.SetActive(false);
        dialoguer = dialogueCanvas.GetComponent<Dialoguer>();
        dialogueCanvas.SetActive(true);
        dialoguer.StartDialogueSegment();
    }
}