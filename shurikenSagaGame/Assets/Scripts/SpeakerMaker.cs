using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speaker", menuName = "Create new Speaker")]
public class Speaker : ScriptableObject
{
    public string SpeakerName;
    public Sprite SpeakerSprite;
    public AudioClip[] MumbleClips;
}
