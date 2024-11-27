using UnityEngine;

public class BGSoundScript : MonoBehaviour {

    private static BGSoundScript instance = null;
    public static BGSoundScript Instance {
        get { return instance; }
    }

    public AudioClip overworldClip;
    public AudioClip shadowClip;
    private AudioSource audioSource;
    private float currentTimestamp = 0f; //timestamp of song

    void Start() {
        audioSource = GetComponent<AudioSource>();
        UpdateMusic(true);
    }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Update() {
        // Optionally, check and update if `isOverWorld` changes during runtime
        UpdateMusic(false);
    }

    public void UpdateMusic(bool forceUpdate) {
        if (audioSource == null || overworldClip == null || shadowClip == null) {
            Debug.LogWarning("AudioSource or Clips are not set in BGSoundScript.");
            return;
        }

        // Save the current timestamp only if switching clips
        if (audioSource.isPlaying && (forceUpdate || (GameHandler.isOverWorld && audioSource.clip == shadowClip) || (!GameHandler.isOverWorld && audioSource.clip == overworldClip))) {
            currentTimestamp = audioSource.time; // Save current playback time
        }

        // Switch to the correct clip if necessary
        if (GameHandler.isOverWorld && audioSource.clip != overworldClip) {
            audioSource.clip = overworldClip;
            audioSource.time = currentTimestamp; // Resume from saved time
            audioSource.Play();
        } else if (!GameHandler.isOverWorld && audioSource.clip != shadowClip) {
            audioSource.clip = shadowClip;
            audioSource.time = currentTimestamp; // Resume from saved time
            audioSource.Play();
        }
    }
}