using System.Collections;
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

    public ScreenFade screenFade; // Reference to the ScreenFade script
    public ScreenShake screenShake; // Reference to the ScreenShake script
    public float shakeDuration;
    private float originalDialogueBoxOpacity;

    void Start()
    {
        DialogueIndex = 0;
        //StartDialogueSegment();
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
        DialogueSegment currentSegment = DialogueSegments[DialogueIndex];

        // Apply shake effect before dialogue if specified
        if (currentSegment.ShouldShakeBefore) {
            originalDialogueBoxOpacity = DialogueBox.color.a;
            
            // Temporarily reduce opacity
            SetUIOpacity(DialogueBox, 0f);
            SetUIOpacity(SpeakerImg, 0f);

            // Start the screen shake effect
            screenShake.StartShake(shakeDuration);
            StartCoroutine(ResumeDialogueAfterShake(shakeDuration));
        }

        // Apply fade-in before dialogue if specified
        if (currentSegment.ShouldFadeIn) {
            screenFade.StartFade(0f, 1f); // Fade from black to transparent
        }

        // Apply fade if the character is praying (half dark screen effect)
        if (currentSegment.IsPraying) {
            screenFade.StartFade(0.7f, 1f); // Fade to 70% opacity (half dark screen)
        }

        SetStyle(currentSegment.Character);
        StartCoroutine(PlayDialogue(currentSegment.Dialogue, currentSegment.IsFinalSegment));
    }

    private IEnumerator ResumeDialogueAfterShake(float shakeDuration)
    {
        yield return new WaitForSeconds(shakeDuration);

        // Restore opacity
        SetUIOpacity(DialogueBox, originalDialogueBoxOpacity);
        SetUIOpacity(SpeakerImg, 1f);
    }

    private void SetUIOpacity(Graphic uiElement, float alpha)
    {
        Color color = uiElement.color;
        color.a = alpha;
        uiElement.color = color;
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
            // Fade-out after the final segment
            screenFade.StartFade(1f, 1f);
            yield return new WaitForSeconds(1f); // Optional delay before hiding
            gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class DialogueSegment
    {
        public string Dialogue;
        public Speaker Character;
        public bool IsFinalSegment;
        public bool IsPraying;
        public bool ShouldFadeIn;
        public bool ShouldShakeBefore;
    }
}
