using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    public AudioSource PauseSFX;
    public AudioSource PlaySFX;
    void Start() {
        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pauseMenu.activeInHierarchy) {
                //PauseSFX
                PlaySFX.Play();
                ResumeGame();
            } else {
                PauseSFX.Play();
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #elif UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}