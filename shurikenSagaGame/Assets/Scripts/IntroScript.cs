using System.Collections;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    // public GameObject dialogueCanvas; // Dialogue Canvas 
    // public Transform player; // player
    // private Dialoguer dialoguer; // Reference to the Dialoguer component
//     private bool playerInTrigger = false; // To track if the player is in the trigger area
// //     private bool initialDialogueCompleted = false; // Track if the initial dialogue has completed

// //     void Start()
// // {
// //     if (dialogueCanvas == null)
// //     {
// //         Debug.LogError("dialogueCanvas is not assigned in the Inspector.");
// //         return;
// //     }

// //     dialoguer = dialogueCanvas.GetComponent<Dialoguer>();
// //     if (dialoguer == null)
// //     {
// //         Debug.LogError("No Dialoguer component found on dialogueCanvas.");
// //         return;
// //     }

// //     if (dialoguer.DialogueSegments == null || dialoguer.DialogueSegments.Length == 0)
// //     {
// //         Debug.LogError("Dialoguer has no DialogueSegments assigned.");
// //         return;
// //     }

// //     // Start the initial dialogue sequence
// //     StartCoroutine(PlayInitialDialogue());
// //     Debug.Log("Starting intro dialogue...");
// // }

// //     IEnumerator PlayInitialDialogue()
// //     {
// //         // Go through each dialogue segment until the final one
// //         for (int i = 0; i < dialoguer.DialogueSegments.Length; i++)
// //         {
// //             // Check if it's the final segment of the intro dialogue
// //             if (dialoguer.DialogueSegments[i].IsFinalSegment)
// //             {
// //                 initialDialogueCompleted = true; // Mark initial dialogue as complete
// //                 break; // Exit the loop to stop here
// //             }

// //             // Display the current dialogue segment
// //             dialoguer.StartDialogueSegment();
// //             yield return new WaitUntil(() => dialoguer.CanContinue); // Wait until dialogue can continue
// //         }
// //     }

// //     void OnTriggerEnter2D(Collider2D other)
// //     {
// //         // Check if the entering collider belongs to the player
// //         if (other.transform == player)
// //         {
// //             playerInTrigger = true; // Player is inside the trigger area
// //         }
// //     }

// //     void OnTriggerExit2D(Collider2D other)
// //     {
// //         // Check if the exiting collider belongs to the player
// //         if (other.transform == player)
// //         {
// //             playerInTrigger = false; // Player is no longer in the trigger area
// //         }
// //     }

// //     void Update()
// //     {
// //         // Check for input to trigger the next dialogue when the player is in the trigger area
// //         if (playerInTrigger && initialDialogueCompleted && Input.GetKeyDown(KeyCode.E))
// //         {
// //             TriggerRemainingDialogue();
// //         }
// //     }

// //     void TriggerRemainingDialogue()
// //     {
// //         dialogueCanvas.SetActive(true);
// //         dialoguer.StartDialogueSegment(); // Continue with the remaining dialogue segments
// //         Debug.Log("Triggering remaining dialogue...");
// //     }
}
