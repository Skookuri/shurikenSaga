using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickBubble : MonoBehaviour
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

    public Transform player;
    private float originalDialogueBoxOpacity;

    void Start()
    {
        DialogueIndex = 0;
    }

    void Update()
    {
        Skip.enabled = CanContinue;
        if (Input.GetKeyDown(KeyCode.P) && CanContinue) {
            DialogueIndex++;
            if (DialogueIndex == DialogueSegments.Length) {
                gameObject.SetActive(false); // Ends display if no more segments
                return;
            }

            StartDialogueSegment();
        }
    }

    public void StartDialogueSegment()
    {
        if (DialogueSegments == null || DialogueIndex >= DialogueSegments.Length) {
            Debug.LogError("DialogueSegments is not properly initialized or the index is out of bounds.");
            return;
        }
        
        player.GetComponent<PlayerMove>().enabled = false; //turns off movement temporarily
        DialogueSegment currentSegment = DialogueSegments[DialogueIndex];

        SetStyle(currentSegment.Character);
        StartCoroutine(PlayDialogue(currentSegment.Dialogue, currentSegment.IsFinalSegment));
    }

    void SetStyle(Speaker Subject)
    {
        if (Subject.SpeakerSprite == null) {
            SpeakerImg.color = new Color(0, 0, 0, 0);
        } else {
            SpeakerImg.sprite = Subject.SpeakerSprite;
            //SpeakerImg.color = Color.white;
        }

        SpeakerName.SetText(Subject.SpeakerName);
    }

    IEnumerator PlayDialogue(string Dialogue, bool isFinalSegment) {
        CanContinue = false;
        DialogueSpeech.SetText(string.Empty);

        for (int i = 0; i < Dialogue.Length; i++) {
            DialogueSpeech.text += Dialogue[i];
            yield return new WaitForSeconds(1f / TextSpeed);
        }
        CanContinue = true;

        if (isFinalSegment) {
            yield return new WaitForSeconds(1f); // Optional delay before hiding
            gameObject.SetActive(false);
        }
    }

    // Restart movement when the dialogue is disabled
    private void OnDisable() {
    if (player != null && player.GetComponent<PlayerMove>() != null) {
        player.GetComponent<PlayerMove>().enabled = true;
    } else {
        Debug.LogWarning("Player or PlayerMove component not found.");
    }
}

    [System.Serializable]
    public class DialogueSegment
    {
        public string Dialogue;
        public Speaker Character;
        public bool IsFinalSegment;
        // public bool IsPraying;
        // public bool ShouldFadeIn;
        // public bool ShouldShakeBefore;
    }
}