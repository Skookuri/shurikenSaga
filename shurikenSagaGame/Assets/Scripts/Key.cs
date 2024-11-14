using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public bool isPickedUp = false;
    private DialogueOnCollide collisionTrigger;  // Reference to the DialogueOnCollide script

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "player") 
        {
            isPickedUp = true;
            gameObject.SetActive(false);

            // Find the door object that is the sibling of this key
            Transform doorTransform = transform.parent.Find("door");
            
            if (doorTransform != null) {
                collisionTrigger = doorTransform.GetComponent<DialogueOnCollide>();
                
                // If the DialogueOnCollide component exists, disable it
                if (collisionTrigger != null) {
                    collisionTrigger.enabled = false;  // Disable the DialogueOnCollide script
                }
            }
        }
    }
}
