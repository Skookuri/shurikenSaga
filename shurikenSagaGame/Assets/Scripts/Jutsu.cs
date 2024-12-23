using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Make sure to include this if you're dealing with UI buttons

public class Jutsu : MonoBehaviour
{
    [SerializeField]
    private Scroll scroll; // Reference to the Scroll script
    private GameObject jutsuUI; // Reference to the jutsu UI element (child of the Jutsu object)
    private Button exitButton; // Reference to the Exit Button
    private bool exitButtonClicked = false; // Placeholder for tracking button state

    void Start()
    {
        // Find the child jutsuUI GameObject from the Jutsu object
        jutsuUI = transform.Find("JutsuUI").gameObject;

        if (jutsuUI == null) {
            Debug.LogError("JutsuUI child not found in the Jutsu object!");
        }

        // Find the exit button inside jutsuUI
        exitButton = jutsuUI.transform.Find("ExitButton").GetComponent<Button>();

        if (exitButton != null) {
            exitButton.onClick.AddListener(OnExitButtonClick); // Add listener for exit button click
        } else {
            Debug.LogError("ExitButton child not found in jutsuUI!");
        }
    }

    void Update()
    {
        // Example check for exit button click
        if (exitButtonClicked || Input.GetButtonDown("TextContinue")) {
            // Hide jutsuUI when exit button is clicked
            if (jutsuUI != null) {
                jutsuUI.SetActive(false);
            } else {
                Debug.LogError("jutsuUI is not assigned!");
            }
        }
    }

    // This method should be called when the exit button is clicked
    public void OnExitButtonClick()
    {
        exitButtonClicked = true; // Set to true when button is clicked
        Debug.Log("Exit button clicked!");
    }
}
