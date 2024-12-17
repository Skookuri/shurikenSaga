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
    //public GameObject tokensText;
    
    public static string respawnScene;
    public bool hit = false;
    public bool isImmune = false;
    private float playerFlashingTime = 0;
    [SerializeField]
    private Color playerImmuneColor;
    private SpriteRenderer playerSpriteRenderer;
    public static int gotTokens = 0;
    
    public bool isDefending = false;
    public static bool stairCaseUnlocked = false;
    //this is a flag check. Add to other scripts: GameHandler.stairCaseUnlocked = true;
    private string sceneName;
    public static string lastLevelDied;  //allows replaying the Level where you died
    public static bool isOverWorld = true;
    private GameObject[] allOverworld = new GameObject[0];
    private GameObject[] allShadow = new GameObject[0];
    private bool cooldownDone = true;
    public bool switching = false;
    private float timePassedWhileSwitching = 0;
    public Image overlayImage;
    [SerializeField]
    public float switchDuration;
    public bool switchRealms = false;
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

    public float timeSinceHeal = 0f;

    public bool doneSwitchingRealms = false;
    public bool resetLinkedKozous = false;
    public AudioSource toShadow;
    public AudioSource toHome;
    public static bool shiftUnlocked;
    public NotificationBehavior n;

    public bool noSwitchZone = false;
    public bool firstRun = true;
    public bool recordPos = true;

    void Start(){
        if(SceneManager.GetActiveScene().name != "Lose Scene"){
            respawnScene = SceneManager.GetActiveScene().name;
            Debug.Log("Scene:");
        }
        Debug.Log(respawnScene);
        playerHealth = StartPlayerHealth;

        if (GameObject.Find("NotificationCanvas").TryGetComponent<NotificationBehavior>(out var script))
            n = script;
        else
            n = null;

        if (SceneManager.GetActiveScene().name == "Dungeon3")
        {
            GameObject.Find("BackgroundMusic").GetComponent<BGSoundScript>().audioSource.Stop();
        }

        allOverworld = GameObject.FindGameObjectsWithTag("overworld");
        allShadow = GameObject.FindGameObjectsWithTag("shadow");
        playerSpriteRenderer = GameObject.Find("player").transform.Find("player_art").GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player");
        updateStatsDisplay();
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
        originalCameraPosition = mainCamera.transform.localPosition;
        
        // Start in the Overworld
        isOverWorld = true;
        //allOverworld.SetActive(true);
        //allShadow.SetActive(false);
        overlayImage.color = new Color(0, 0, 0, 0);
        playerHealth = StartPlayerHealth;
        updateStatsDisplay();
        audioSource = GetComponent<AudioSource>();
    }

    public void killPlayer()
    {
        playerDies();
        playerHealth -= 1000;
        updateStatsDisplay();
        //playerDies();
    }

    private void Update() {
        if (playerHealth != 100)
        {
            timeSinceHeal += Time.deltaTime;
            if (timeSinceHeal > 10f)
            {
                timeSinceHeal = 0f;
                playerHealth += 5;
                updateStatsDisplay();
            }
        }

        if (firstRun)
        {
            firstRun = false;
            if (isOverWorld)
            {
                toHome.Play();
                foreach (GameObject g in allShadow)
                {
                    g.SetActive(false);
                }
                foreach (GameObject g in allOverworld)
                {
                    g.SetActive(true);
                }
            }
            else
            {
                toShadow.Play();
                foreach (GameObject g in allShadow)
                {
                    g.SetActive(true);
                }
                foreach (GameObject g in allOverworld)
                {
                    g.SetActive(false);
                }
            }
        }
        HandlePlayerHit();

        if (Input.GetButtonDown("ShiftRealms") && cooldownDone && shiftUnlocked && !noSwitchZone) {
            StartRealmSwitch();
        } else if (Input.GetButtonDown("ShiftRealms")) {
            PlayCannotSwitchSound();
            if (!cooldownDone)
            {
                if (n != null)
                {
                    n.startNotif("You must wait before switching realms again.");
                }
            }
            else if (noSwitchZone)
            {
                n.startNotif("Something is blocking you from switching realms here.");
            }
            Debug.Log("Can't switch yet!");
        }

        if (switching) {
            PerformRealmSwitch();
        }
        if (playerHealth <= 0)
        {
            playerDies();
        }
    }

    private void StartRealmSwitch() {
        cooldownDone = false;
        switching = true;
        timePassedWhileSwitching = 0f;

        // Start overlay fade and realm activation
        overlayImage.color = new Color(!isOverWorld ? 0 : 1, !isOverWorld ? 0 : 1, !isOverWorld ? 0 : 1, 0);
        StartCoroutine(Cooldown());
    }
    
    private void PerformRealmSwitch() {
        CameraShake();
        timePassedWhileSwitching += Time.deltaTime;

        if (timePassedWhileSwitching < switchDuration) {
            // Fade to black
            float alpha = Mathf.Clamp01(timePassedWhileSwitching / switchDuration);
            overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);
            switchRealms = true;
        } else if (timePassedWhileSwitching < switchDuration * 2) {
            if (switchRealms) {
                // Switch realms once halfway through the fade
                recordPos = false;
                Debug.Log("recordPos false");
                switchRealms = false;
                if (!isOverWorld) {
                    toHome.Play();
                    foreach (GameObject g in allShadow)
                    {
                        g.SetActive(false);
                    }
                    foreach (GameObject g in allOverworld)
                    {
                        g.SetActive(true);
                    }
                } else {
                    toShadow.Play();
                    foreach (GameObject g in allShadow)
                    {
                        g.SetActive(true);
                    }
                    foreach (GameObject g in allOverworld)
                    {
                        g.SetActive(false);
                    }
                }
                // Toggle the Overworld/Shadow state
                isOverWorld = !isOverWorld;

                GameObject[] allLinkedEnemies = GameObject.FindGameObjectsWithTag("linkedenemy");

                if (allLinkedEnemies != null && allLinkedEnemies.Length > 0)
                {
                    foreach (GameObject obj in allLinkedEnemies)
                    {
                        obj.GetComponent<LinkedEnemies>().hasProcessedSwitch = false;
                    }
                }
                else
                {
                    Debug.Log("No objects found with the specified tag.");
                }

                Debug.Log("doneSwitchingRealms true");
                //doneSwitchingRealms = true;
                resetLinkedKozous = true;
                //recordPos = false;
            }

            // Fade back to transparent
            float alpha = Mathf.Clamp01((timePassedWhileSwitching - switchDuration) / switchDuration);
            overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, 1 - alpha);
        } else {
            // End of switching
            switching = false;
            overlayImage.color = new Color(0, 0, 0, 0);
            mainCamera.transform.localPosition = new Vector3(originalCameraPosition.x, originalCameraPosition.y, -10f);
        }
    }

    private void PlayCannotSwitchSound() {
        if (audioSource != null && cantSwitchSound != null) {
            audioSource.PlayOneShot(cantSwitchSound);
        }
    }

    private void HandlePlayerHit() {
        if (hit && !isImmune) {
            playerGetHit();
            hit = false;
            isImmune = true;
            playerFlashingTime = 0f;
            StartCoroutine(hitCooldown());
        }

        if (isImmune) {
            playerFlashingTime += Time.deltaTime;
            if (playerFlashingTime > 0.08f) {
                playerSpriteRenderer.color = (playerSpriteRenderer.color == playerImmuneColor)
                    ? new Color(1, 1, 1, 1)
                    : playerImmuneColor;
                playerFlashingTime = 0f;
            }
        }
    }

    private IEnumerator Cooldown() {
        yield return new WaitForSeconds(3);
        Debug.Log("2 seconds have passed!");
        cooldownDone = true;
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

    public void playerGetTokens(int newTokens) {
        gotTokens += newTokens;
        //updateStatsDisplay();
    }

    public IEnumerator hitCooldown() {
        yield return new WaitForSeconds(1.5f);
        //Debug.Log("2 seconds have passed!");
        isImmune = false;
        playerSpriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void playerGetHit() {
        if (!isDefending){
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

    public void playerDies() {
        //player.GetComponent<PlayerHurt>().playerDead();       //play Death animation
        lastLevelDied = sceneName;       //allows replaying the Level where you died
        SceneManager.LoadScene("Lose Scene");
        //StartCoroutine(DeathPause());
    }

    public void updateStatsDisplay(){
            Text healthTextTemp = healthText.GetComponent<Text>();
            healthTextTemp.text = "HEALTH: " + playerHealth;

            //Text tokensTextTemp = tokensText.GetComponent<Text>();
            //tokensTextTemp.text = "GOLD: " + gotTokens;
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

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    // Return to MainMenu
    public void RestartGame() {
        Time.timeScale = 1f;
        Debug.Log("restart");
        Debug.Log(respawnScene);
        SceneManager.LoadScene(respawnScene);
            // Reset all static variables here, for new games:
        playerHealth = StartPlayerHealth;
    }

    // Replay the Level where you died
    public void ReplayLastLevel() {
        Time.timeScale = 1f;
        Debug.Log("replay");
        Debug.Log(respawnScene);
        SceneManager.LoadScene(respawnScene);
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