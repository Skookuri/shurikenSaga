using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnCollide : MonoBehaviour
{
    public GameObject dialogueCanvas; // Dialogue Canvas that contains Dialoguer component
    public Transform player; // Reference to the player
    private Dialoguer dialoguer; // Reference to the Dialoguer component

    void Start()
    {
        dialogueCanvas.SetActive(false); // Hide dialogue canvas initially
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the player collided with this object
        if (other.transform == player)
        {
            TriggerDialogueAction();
        }
    }

    void TriggerDialogueAction()
    {
        dialoguer = dialogueCanvas.GetComponent<Dialoguer>();
        dialogueCanvas.SetActive(true); // Show dialogue canvas
        dialoguer.StartDialogueSegment(); // Start the dialogue
        Debug.Log("Dialogue started on collision.");
    }
}
