using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMumble : MonoBehaviour
{
    public AudioClip[] mumbleClips; // Array of mumble clips to choose from

    private AudioSource audioSource; // The AudioSource on the current object
    private int lastMumbleIndex = -1; // Track the last played mumble index to avoid repeats
    private bool isPlaying = false; // Tracks if the AudioSource is currently playing

    void Start()
    {
        // Get the AudioSource attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component attached to this GameObject!");
            return;
        }

        if (mumbleClips == null || mumbleClips.Length == 0)
        {
            Debug.LogError("No mumble clips assigned!");
        }
    }

    public void QueueRandomMumble()
    {
        if (mumbleClips == null || mumbleClips.Length == 0)
        {
            Debug.LogWarning("No mumble clips available to queue.");
            return;
        }

        // If the AudioSource is currently playing, return to queue the mumble later
        if (isPlaying)
        {
            return;
        }

        // Randomize a new mumble clip index
        int mumbleIndex;
        do
        {
            mumbleIndex = Random.Range(0, mumbleClips.Length);
        } while (mumbleIndex == lastMumbleIndex && mumbleClips.Length > 1);

        lastMumbleIndex = mumbleIndex;

        // Assign the new clip and play it
        audioSource.clip = mumbleClips[mumbleIndex];
        PlayMumble();
    }

    private void PlayMumble()
    {
        if (audioSource.clip != null)
        {
            isPlaying = true;
            audioSource.Play();
            Invoke(nameof(OnMumbleComplete), audioSource.clip.length); // Handle the end of playback
        }
    }

    private void OnMumbleComplete()
    {
        isPlaying = false;
        // Optionally, you could trigger another mumble or logic here
    }
}

