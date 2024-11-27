using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameHandler : MonoBehaviour {

    private GameObject player;
    public int playerHealth = 100;
    public int StartPlayerHealth = 100;
    public GameObject healthText;
    public bool hit = false;
    public bool isImmune = false;
    private float playerFlashingTime = 0;
    [SerializeField]
    private Color playerImmuneColor;
    private SpriteRenderer playerSpriteRenderer;

    public static int gotTokens = 0;
    public GameObject tokensText;

    public bool isDefending = false;

    public static bool stairCaseUnlocked = false;
    //this is a flag check. Add to other scripts: GameHandler.stairCaseUnlocked = true;

    private string sceneName;
    public static string lastLevelDied;  //allows replaying the Level where you died

    public bool isOverWorld = true;
    private GameObject allOverworld;
    private GameObject allShadow;
    private bool cooldownDone = true;
    private bool switching = false;
    private float timePassedWhileSwitching = 0;
    public Image overlayImage;
    [SerializeField]
    public float switchDuration;
    private bool switchRealms = false;
    public Camera mainCamera;
    [SerializeField]
    public float shakeIntensity;
    [SerializeField]
    public float shakeFrequency;
    private Vector3 originalCameraPosition;
    [SerializeField] 
    AudioClip cantSwitchSound;
    [SerializeField]
    private AudioSource audioSource;

    private bool firstRunThrough = true;

    public AudioSource toShadow;
    public AudioSource toHome;


    void Start(){

        allOverworld = GameObject.FindWithTag("overworld");
        allShadow = GameObject.FindWithTag("shadow");
        playerSpriteRenderer = GameObject.Find("player").transform.Find("player_art").GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player");
        sceneName = SceneManager.GetActiveScene().name;
        //if (sceneName=="MainMenu"){ //uncomment these two lines when the MainMenu exists
                playerHealth = StartPlayerHealth;
        //}
        updateStatsDisplay();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        originalCameraPosition = mainCamera.transform.localPosition;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (firstRunThrough)
        {
            firstRunThrough = false;
            if (isOverWorld) {
                allShadow.SetActive(true);
                allOverworld.SetActive(false);
            } else {
                allShadow.SetActive(false);
                allOverworld.SetActive(true);
            }
        }
        
        if (hit && !isImmune)
        {
            playerGetHit();
            hit = false;
            isImmune = true;
            playerFlashingTime = 0f;
            StartCoroutine(hitCooldown());
        }
        if (isImmune)
        {
            playerFlashingTime += Time.deltaTime;
            if (playerFlashingTime > .08)
            {
                playerSpriteRenderer.color = (playerSpriteRenderer.color == playerImmuneColor) ? new Color (1, 1, 1, 1) : playerImmuneColor;
                playerFlashingTime = 0f;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (cooldownDone)
            {
                cooldownDone = false;
                if (isOverWorld)
                {
                    overlayImage.color = new Color(255, 255, 255, 0);
                    isOverWorld = false;
                }
                else
                {
                    overlayImage.color = new Color(0, 0, 0, 0);
                    isOverWorld = true;
                }
                Debug.Log("isOverWorld: " + isOverWorld);
                switching = true;
                timePassedWhileSwitching = 0f;
                StartCoroutine(Cooldown());
            }
            else
            {
                // Play the "cannot switch" sound
                if (audioSource != null && cantSwitchSound != null)
                {
                    audioSource.PlayOneShot(cantSwitchSound);
                }
            }
        }
        if (switching)
        {
            CameraShake();

            timePassedWhileSwitching += Time.deltaTime;

            if (timePassedWhileSwitching < switchDuration)
            {
                float alpha = Mathf.Clamp01(timePassedWhileSwitching / switchDuration);
                overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);
                switchRealms = true;
            } else if (timePassedWhileSwitching < switchDuration * 2)
            {
                if(switchRealms)
                {
                    switchRealms = false;
                    if (!isOverWorld) {
                        toHome.Play();
                        allShadow.SetActive(false);
                        allOverworld.SetActive(true);
                    } else {
                        toShadow.Play();
                        allShadow.SetActive(true);
                        allOverworld.SetActive(false);
                    }
                }

                float alpha = Mathf.Clamp01((timePassedWhileSwitching - switchDuration) / switchDuration);
                overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, 1 - alpha);
            } else
            {
                switching = false;
                overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, 0);
                mainCamera.transform.localPosition = new Vector3(originalCameraPosition.x, originalCameraPosition.y, -10f);
            }
        }
    }
    
    public IEnumerator hitCooldown()
    {
        yield return new WaitForSeconds(1.5f);
        //Debug.Log("2 seconds have passed!");
        isImmune = false;
        playerSpriteRenderer.color = new Color(1, 1, 1, 1);
    }
    
    private void CameraShake()
    {
        if (mainCamera == null) return;

        // Generate random offsets for x and y axes
        float offsetX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) * 2 - 1) * shakeIntensity;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) * 2 - 1) * shakeIntensity;

        // Apply the offsets to the camera's original position, keeping z constant
        mainCamera.transform.position = originalCameraPosition + new Vector3(offsetX, offsetY, 0);

        // Lock the z position to -10
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10f);
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("2 seconds have passed!");
        cooldownDone = true;
    }

    public void playerGetTokens(int newTokens){
        gotTokens += newTokens;
        //updateStatsDisplay();
    }

    public void playerGetHit(){
        if (isDefending == false){
            //yield return new WaitForSeconds(1.0f);
            if (playerHealth >=0){
                updateStatsDisplay();
            } else
            {
                playerHealth = 0;
                playerDies();
            }
        }

        if (playerHealth > StartPlayerHealth){
                playerHealth = StartPlayerHealth;
                updateStatsDisplay();
        }
    }

    


    public void updateStatsDisplay(){
            Text healthTextTemp = healthText.GetComponent<Text>();
            healthTextTemp.text = "HEALTH: " + playerHealth;

            Text tokensTextTemp = tokensText.GetComponent<Text>();
            tokensTextTemp.text = "GOLD: " + gotTokens;
    }

    public void playerDies(){
        //player.GetComponent<PlayerHurt>().playerDead();       //play Death animation
        lastLevelDied = sceneName;       //allows replaying the Level where you died
        SceneManager.LoadScene("Lose Scene");
        //StartCoroutine(DeathPause());
    }

    IEnumerator DeathPause(){
        player.GetComponent<PlayerMove>().isAlive = false;
        //player.GetComponent<PlayerJump>().isAlive = false;
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Lose Scene");
    }

    public void StartGame() {
        SceneManager.LoadScene("IntroScene");
    }

    public void EndGame() {
        SceneManager.LoadScene("Lose Scene");
    }

    // Return to MainMenu
    public void RestartGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
            // Reset all static variables here, for new games:
        playerHealth = StartPlayerHealth;
    }

    // Replay the Level where you died
    public void ReplayLastLevel() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(lastLevelDied);
            // Reset all static variables here, for new games:
        playerHealth = StartPlayerHealth;
    }

    public void QuitGame() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
    }

    public void Credits() {
        SceneManager.LoadScene("Credits");
    }
}