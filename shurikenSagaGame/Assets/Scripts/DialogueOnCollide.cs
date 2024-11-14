using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnCollide : MonoBehaviour
{
    public GameObject dialogueCanvas; // Dialogue Canvas that contains Dialoguer component
    private Dialoguer dialoguer; // Reference to the Dialoguer component
    //public Transform player; // Reference to the player

    void Start()
    {
        dialogueCanvas.SetActive(false); // Hide dialogue canvas initially
    }

    // Public helper function to trigger dialogue action
    public void TriggerDialogueAction()
    {
        // Ensure the dialoguer is correctly initialized
        if (dialogueCanvas != null) {
            dialoguer = dialogueCanvas.GetComponent<Dialoguer>();
            if (dialoguer != null) {
                dialogueCanvas.SetActive(true); // Show dialogue canvas
                dialoguer.StartDialogueSegment(); // Start the dialogue
                //Debug.Log("Dialogue started on collision.");
            } else {
                Debug.LogError("Dialoguer component is missing from the dialogueCanvas.");
            }
        } else {
            Debug.LogError("DialogueCanvas is not assigned.");
        }
    }
}
