using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    public bool isPickedUp = false;
    private DialogueOnCollide collisionTrigger;  // Reference to the DialogueOnCollide script
    public AudioSource ScrollGetSFX;
    public GameObject LevelExitDoor;
    public JutsuType jutsuTypeUnlocked;
    // private Abilities abilities;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "player" && !isPickedUp)
        {
            isPickedUp = true;

            // Set the ability based on the unlocked JutsuType
            UnlockAbility(jutsuTypeUnlocked);

            // Deactivate the scroll after pickup
            gameObject.SetActive(false);

            // Find the jutsu object that is the sibling of this scroll
            Transform jutsuTransform = transform.parent.Find("jutsu");
            
            if (jutsuTransform != null)
            {
                // Enable the jutsuUI display child of jutsu
                Transform jutsuUI = jutsuTransform.Find("JutsuUI");
                if (jutsuUI != null)
                {
                    ScrollGetSFX.Play();
                    jutsuUI.gameObject.SetActive(true);
                    LevelExitDoor.SetActive(false);
                } else {
                    Debug.LogError("jutsuUI child not found in jutsu.");
                }
            }
            else
            {
                Debug.LogError("jutsu object not found as sibling of scroll.");
            }
        }
    }

    private void UnlockAbility(JutsuType jutsuType)
    {
        switch (jutsuType)
        {
            case JutsuType.canShuriken:
                //Abilities.canShurithrow = true;
                Debug.Log("Can throw shuriken...");
                break;
            case JutsuType.canDash:
                //Abilities.canDash = true;
                Debug.Log("Can dash...");
                break;
            case JutsuType.canShift:
                GameHandler.shiftUnlocked = true; // Automatically updates shiftUnlocked in GameHandler
                Debug.Log("Can plane shift...");
                break;
            case JutsuType.canKatana:
                //Abilities.canKatana = true;
                Debug.Log("Can wield katana...");
                break;
            default:
                Debug.LogWarning("Unknown JutsuType: " + jutsuType);
                break;
        }
    }
}