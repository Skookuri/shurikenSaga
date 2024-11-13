using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialoguer : MonoBehaviour
{
    public DialogueSegment[] DialogueSegments;
    [Space]
    public Image SpeakerImg;
    public Image DialogueBox;
    public Image DialogueContent;
    public Image Skip;
    [Space]
    public TextMeshProUGUI SpeakerName;
    public TextMeshProUGUI DialogueSpeech;
    [Space]
    public float TextSpeed;
    public bool CanContinue;
    private int DialogueIndex;

    void Start()
    {
        SetStyle(DialogueSegments[0].Character);
        StartCoroutine(PlayDialogue(DialogueSegments[0].Dialogue));
    }

    void Update()
    {
        Skip.enabled = CanContinue;
        if (Input.GetKeyDown(KeyCode.P) && CanContinue)
        {
            DialogueIndex++;
            if (DialogueIndex == DialogueSegments.Length)
            {
                gameObject.SetActive(false);
                return;
            }

            SetStyle(DialogueSegments[DialogueIndex].Character);
            StartCoroutine(PlayDialogue(DialogueSegments[DialogueIndex].Dialogue));
        }
    }

    void SetStyle(Speaker Subject)
    {
        if (Subject.SpeakerSprite == null)
        {
            SpeakerImg.color = new Color(0, 0, 0, 0);
        }
        else
        {
            SpeakerImg.sprite = Subject.SpeakerSprite;
            SpeakerImg.color = Color.white;
        }

        SpeakerName.SetText(Subject.SpeakerName);
    }

    IEnumerator PlayDialogue(string Dialogue)
    {
        CanContinue = false;
        DialogueSpeech.SetText(string.Empty);

        for (int i = 0; i < Dialogue.Length; i++)
        {
            DialogueSpeech.text += Dialogue[i];
            yield return new WaitForSeconds(1f / TextSpeed);
        }
        CanContinue = true;
    }

    [System.Serializable]
    public class DialogueSegment
    {
        public string Dialogue;
        public Speaker Character;
    }
}
