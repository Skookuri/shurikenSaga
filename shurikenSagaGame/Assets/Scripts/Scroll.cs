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
                }
                else
                {
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
        // Set the ability flag based on the JutsuTypeUnlocked enum
        switch (jutsuType)
        {
            case JutsuType.Shuriken:
                Abilities.canShurithrow = true;
                Debug.Log("Can throw shuriken...");
                break;
            case JutsuType.Dash:
                Abilities.canDash = true;
                Debug.Log("Can dash...");
                break;
            case JutsuType.PlaneShift:
                Abilities.canShift = true;
                Debug.Log("Can plane shift...");
                break;
            // Add more cases as needed for other jutsu types
            default:
                Debug.LogWarning("Unknown JutsuType: " + jutsuType);
                break;
        }
    }
}