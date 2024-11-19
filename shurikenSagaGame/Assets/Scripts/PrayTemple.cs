using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayTemple : MonoBehaviour
{
    public GameObject prayText; // Pray text UI element
    public GameObject dialogueCanvas; // Dialogue Canvas 
    public Transform player; // player
    public GameObject ScrollJutsuPair; //scroll jutsu pair that drops after interaction is over
    private Dialoguer dialoguer; // Declare a Dialoguer variable
    private bool playerInTrigger = false; // To track if the player is in the trigger area
    private bool hasInteracted = false; // To track if the player has interacted with the object
    

    void Start()
    {
        prayText.SetActive(false); // Hide the button at the start
        dialogueCanvas.SetActive(false);
        ScrollJutsuPair.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering collider belongs to the player
        if (other.transform == player) {
            playerInTrigger = true; // Player is inside the trigger area
            prayText.SetActive(true); // Show the pray button when player is near
            //Debug.Log("Player entered the trigger area.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the exiting collider belongs to the player
        if (other.transform == player) {
            playerInTrigger = false; // Player is no longer in the trigger area
            
            prayText.SetActive(false); // Hide the pray button when player leaves
            //Debug.Log("Player exited the trigger area.");
        }
    }

    void Update()
    {
        // Check for input to trigger the prayer action when the player is in the trigger area
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E) && !hasInteracted) {
            TriggerPrayAction(); //starts prayer interaction when pressing e in location, first time doing so
        } else if (playerInTrigger && hasInteracted) {
            prayText.SetActive(false); //hides pray text now that you already have started the interaction
        } else if (!dialogueCanvas.activeSelf && hasInteracted) {
            ScrollJutsuPair.SetActive(true); //shows scroll jutsu pair after interaction has ended
            Debug.Log("Activating Scroll Jutsu Pair");
        }
    }

    // This function is called when the Pray button is clicked (or 'E' is pressed while in the trigger area)
    void TriggerPrayAction()
    {
        // Deactivate the pray text once clicked
        prayText.SetActive(false);
        hasInteracted = true; // Mark that the player has interacted
        dialoguer = dialogueCanvas.GetComponent<Dialoguer>();
        dialogueCanvas.SetActive(true);
        dialoguer.StartDialogueSegment();
    }
}