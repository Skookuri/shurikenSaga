using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    public bool isPickedUp = false;
    private DialogueOnCollide collisionTrigger;  // Reference to the DialogueOnCollide script
    public AudioSource ScrollGetSFX;
    public GameObject LevelExitDoor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "player") {
            isPickedUp = true;
            gameObject.SetActive(false);

            // Find the jutsu object that is the sibling of this scroll
            Transform jutsuTransform = transform.parent.Find("jutsu");
            
            if (jutsuTransform != null) {
                // Enable the jutsuUI display child of jutsu
                Transform jutsuUI = jutsuTransform.Find("JutsuUI");
                if (jutsuUI != null) {
                    ScrollGetSFX.Play();
                    jutsuUI.gameObject.SetActive(true);
                    LevelExitDoor.SetActive(false);
                } else {
                    Debug.LogError("jutsuUI child not found in jutsu.");
                }
            } else {
                Debug.LogError("jutsu object not found as sibling of scroll.");
            }
        }
    }
}
