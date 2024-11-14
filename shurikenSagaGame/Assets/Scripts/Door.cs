using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    Key key; // Reference to the Key script
    private DialogueOnCollide dialogueOnCollide; // Reference to the DialogueOnCollide component

    void Start()
    {
        dialogueOnCollide = GetComponent<DialogueOnCollide>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.name == "player" && key.isPickedUp) {
            gameObject.SetActive(false); // Disable the door when the player has picked up the key
        } else if (collision.collider.gameObject.name == "player" && !key.isPickedUp) {
            // Call the TriggerDialogueAction function of DialogueOnCollide
            if (dialogueOnCollide != null) {
                dialogueOnCollide.TriggerDialogueAction(); // Trigger dialogue when the player collides and doesn't have the key
            } else {
                Debug.LogError("DialogueOnCollide component is missing on the door.");
            }
        }
    }
}
