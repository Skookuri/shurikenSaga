using UnityEngine;

public class BGSoundScript : MonoBehaviour {

    private static BGSoundScript instance = null;

    public static BGSoundScript Instance {
        get { return instance; }
    }

    public AudioClip overworldClip; // Assign in the Inspector: Music.Homeworld.Overworld
    public AudioClip shadowClip;   // Assign in the Inspector: Music.Homeworld.Shadow
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        UpdateMusic();
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
        UpdateMusic();
    }

    public void UpdateMusic() {
        if (audioSource == null || overworldClip == null || shadowClip == null) {
            Debug.LogWarning("AudioSource or Clips are not set in BGSoundScript.");
            return;
        }

        // Set the correct clip based on isOverWorld
        if (GameHandler.isOverWorld) {
            if (audioSource.clip != overworldClip) {
                audioSource.clip = overworldClip;
                audioSource.Play();
            }
        } else {
            if (audioSource.clip != shadowClip) {
                audioSource.clip = shadowClip;
                audioSource.Play();
            }
        }
    }
}