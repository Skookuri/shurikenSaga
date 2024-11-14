using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnCollide : MonoBehaviour
{
    public GameObject dialogueCanvas; // Dialogue Canvas that contains the dialogue component
    public string dialoguerComponent; // Name of the script to get
    private MonoBehaviour dialoguer; // A generic MonoBehaviour to hold any script

    void Start()
    {
        dialogueCanvas.SetActive(false); // Hide dialogue canvas initially

        if (!string.IsNullOrEmpty(dialoguerComponent) && dialogueCanvas != null)
        {
            Type componentType = Type.GetType(dialoguerComponent);

            if (componentType != null)
            {
                dialoguer = dialogueCanvas.GetComponent(componentType) as MonoBehaviour;

                if (dialoguer == null)
                {
                    Debug.LogError($"Component of type '{dialoguerComponent}' not found on dialogueCanvas.");
                }
            }
            else
            {
                Debug.LogError($"Type '{dialoguerComponent}' not recognized. Make sure the name is correct and includes the namespace if necessary.");
            }
        }
        else if (dialogueCanvas == null)
        {
            Debug.LogError("DialogueCanvas is not assigned.");
        }
    }

    // Public helper function to trigger dialogue action
    public void TriggerDialogueAction()
    {
        if (dialoguer != null && dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(true); // Show dialogue canvas

            // Invoke StartDialogueSegment using reflection
            dialoguer.Invoke("StartDialogueSegment", 0f);
        }
        else if (dialoguer == null)
        {
            Debug.LogError("Dialoguer component is not initialized.");
        }
        else
        {
            Debug.LogError("DialogueCanvas is not assigned.");
        }
    }
}