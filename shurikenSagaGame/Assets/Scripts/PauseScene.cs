using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    public AudioSource PauseSFX;
    public AudioSource PlaySFX;
    public AudioMixer mixer;

    public static float BGMusicVolVal = 1.0f;
    public static float SFXVolVal = 1.0f;

    private Slider musicSliderCtrl;
    private Slider sfxSliderCtrl;

    void Start()
    {
        // Ensure the pause menu is inactive initially
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        // Initialize volume levels
        SetMusicVolume(BGMusicVolVal);
        SetSFXVolume(SFXVolVal);

        // Locate and initialize sliders
        InitializeSliders();

        // Ensure game is running at normal time scale
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        // Toggle pause menu on Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu != null)
            {
                if (pauseMenu.activeSelf)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    private void InitializeSliders()
    {
        // Locate and set up music slider
        GameObject musicSliderTemp = GameObject.FindWithTag("VolumeMusicSlider");
        if (musicSliderTemp != null)
        {
            musicSliderCtrl = musicSliderTemp.GetComponent<Slider>();
            musicSliderCtrl.value = BGMusicVolVal;
            musicSliderCtrl.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            Debug.LogWarning("Music slider not found! Ensure it is tagged as 'VolumeMusicSlider'.");
        }

        // Locate and set up SFX slider
        GameObject sfxSliderTemp = GameObject.FindWithTag("VolumeSFXSlider");
        if (sfxSliderTemp != null)
        {
            sfxSliderCtrl = sfxSliderTemp.GetComponent<Slider>();
            sfxSliderCtrl.value = SFXVolVal;
            sfxSliderCtrl.onValueChanged.AddListener(SetSFXVolume);
        }
        else
        {
            Debug.LogWarning("SFX slider not found! Ensure it is tagged as 'VolumeSFXSlider'.");
        }
    }

    public void PauseGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            PauseSFX?.Play();
            Time.timeScale = 0.0f; // Pause the game
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
            PlaySFX?.Play();
            Time.timeScale = 1.0f; // Resume the game
        }
    }

    public void SetMusicVolume(float sliderValue)
    {
        // Update the music volume in the mixer
        mixer.SetFloat("BGMusicVol", Mathf.Log10(sliderValue) * 20);
        BGMusicVolVal = sliderValue;
    }

    public void SetSFXVolume(float sliderValue)
    {
        // Update the SFX volume in the mixer
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        SFXVolVal = sliderValue;
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
        #if UNITY_STANDALONE
            Application.Quit();
        #elif UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
