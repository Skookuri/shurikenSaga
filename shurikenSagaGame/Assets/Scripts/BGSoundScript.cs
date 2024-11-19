using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BGSoundScript : MonoBehaviour {

    private static BGSoundScript instance = null;

    public static BGSoundScript Instance{
        get {return instance;}
    }

    void Start(){
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null && !audio.isPlaying){
            //Debug.Log("AudioSource is not playing. Starting playback.");
            audio.Play();
        }
    }

    void Awake(){
        if (instance != null && instance != this){
            //Debug.Log("Duplicate BGSoundScript detected, destroying this instance.");
            Destroy(this.gameObject);
            return;
        } else {
            //Debug.Log("BGSoundScript instance created.");
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

}