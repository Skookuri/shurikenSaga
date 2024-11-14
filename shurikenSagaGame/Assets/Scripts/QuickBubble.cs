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
        if (DialogueSegments == null || DialogueSegments.Length == 0)
        {
            Debug.LogError("DialogueSegments is not properly initialized.");
            gameObject.SetActive(false);  // Disable the object if there's no dialogue
            return;
        }
    }

    void Update()
    {
        Skip.enabled = CanContinue;
        if (Input.GetKeyDown(KeyCode.P) && CanContinue) {
            if (DialogueSegments.Length != 1) {
                DialogueIndex++;
            }
            
            if (DialogueIndex == DialogueSegments.Length - 1) {
                gameObject.SetActive(false); // Ends display if no more segments
                return;
            }

            StartDialogueSegment();
        }
    }

    public void StartDialogueSegment() {
        // Ensure that the DialogueSegments array and the current index are valid
        if (DialogueIndex >= DialogueSegments.Length) {
            Debug.LogError("DialogueSegments is not properly initialized or the index is out of bounds.");
            return;
        }

        player.GetComponent<PlayerMove>().enabled = false; // Disable movement temporarily
        DialogueSegment currentSegment = DialogueSegments[DialogueIndex];

        SetStyle(currentSegment.Character);
        StartCoroutine(PlayDialogue(currentSegment.Dialogue, currentSegment.IsFinalSegment));
    }

    void SetStyle(Speaker Subject)
    {
        if (Subject.SpeakerSprite == null)
        {
            SpeakerImg.color = new Color(0, 0, 0, 0); // Hide speaker image if no sprite is provided
        } else {
            SpeakerImg.sprite = Subject.SpeakerSprite;
        }

        SpeakerName.SetText(Subject.SpeakerName);
    }

    IEnumerator PlayDialogue(string Dialogue, bool isFinalSegment)
    {
        CanContinue = false;
        DialogueSpeech.SetText(string.Empty);

        for (int i = 0; i < Dialogue.Length; i++) {
            DialogueSpeech.text += Dialogue[i];
            yield return new WaitForSeconds(1f / TextSpeed);
        }

        CanContinue = true;

        // If it's the final segment, reset DialogueIndex and hide dialogue after a short delay
        if (isFinalSegment) {
            DialogueIndex = 0; // Reset the DialogueIndex to start from the beginning
            yield return new WaitForSeconds(1f); // Optional delay before hiding the dialogue
            gameObject.SetActive(false); // Hide the dialogue box
        }
    }

    // Restart movement when the dialogue is disabled
    private void OnDisable()
    {
        if (player != null && player.GetComponent<PlayerMove>() != null)
        {
            player.GetComponent<PlayerMove>().enabled = true; // Re-enable movement
        }
    }

    [System.Serializable]
    public class DialogueSegment
    {
        public string Dialogue;
        public Speaker Character;
        public bool IsFinalSegment;
    }
}