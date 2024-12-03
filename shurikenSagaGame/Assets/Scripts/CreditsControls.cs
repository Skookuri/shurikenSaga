using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class CreditsControls : MonoBehaviour{
    [SerializeField] public GameObject creditsPopup;
    [SerializeField] public GameObject controlsPopup;

    void Start()
    {
        // Ensure the pause menu is inactive initially
        if (creditsPopup != null) {
            creditsPopup.SetActive(false);
        }
        if (controlsPopup != null) {
            controlsPopup.SetActive(false);
        }

        // Ensure game is running at normal time scale
        Time.timeScale = 1.0f;
    }
    public void ActivateCredits()
    {
        if (creditsPopup != null){
            creditsPopup.SetActive(true);
            Time.timeScale = 0.0f; // Pause the game
        }
    }
    public void ActivateControls()
    {
        if (controlsPopup != null){
            controlsPopup.SetActive(true);
            Time.timeScale = 0.0f; // Pause the game
        }
    }
    public void Exit()
    {
        if (creditsPopup != null){
            creditsPopup.SetActive(false);

            Time.timeScale = 1.0f; // Resume the game
        }
        if (controlsPopup != null){
            controlsPopup.SetActive(false);

            Time.timeScale = 1.0f; // Resume the game
        }
    }
}
