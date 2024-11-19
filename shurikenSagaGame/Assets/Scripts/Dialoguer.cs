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

    public Transform player;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component in Player_Art
    private Animator animator; // Reference to the Animator component in Player_Art
    public Sprite prayingSprite;

    public ScreenFade screenFade; // Reference to the ScreenFade script
    public ScreenShake screenShake; // Reference to the ScreenShake script
    public float shakeDuration;
    public bool playOnStart;
    public AudioSource popSFX;
    private float originalDialogueBoxOpacity;
    

    void Start()
    {
        DialogueIndex = 0;

        spriteRenderer = player.Find("player_art").GetComponent<SpriteRenderer>();
        animator = player.Find("player_art").GetComponent<Animator>();

        if (playOnStart) {
            StartDialogueSegment();
        } 
    }

    void Update()
    {
        Skip.enabled = CanContinue;
        if (Input.GetKeyDown(KeyCode.P) && CanContinue) {
            popSFX.Play();
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

        // Apply shake effect before dialogue if specified
        if (currentSegment.ShouldShakeBefore) {
            originalDialogueBoxOpacity = DialogueBox.color.a;
            
            // Temporarily reduce opacity
            SetUIOpacity(DialogueBox, 0f);
            SetUIOpacity(SpeakerName, 0f);
            SetUIOpacity(DialogueSpeech, 0f);
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
            
            animator.enabled = false; // Disable the animator
            spriteRenderer.sprite = prayingSprite; // Set the praying sprite
        } /*else {
            // if (animator.enabled == false) {
            //     Debug.Log("Enabling animator");
            //     animator.enabled = true; // Enable the animator
            // }
            // Debug.Log("Disabled animator successfully");
        }*/

        SetStyle(currentSegment.Character);
        StartCoroutine(PlayDialogue(currentSegment.Dialogue, currentSegment.IsFinalSegment));
        Debug.Log("Starting dialog...");
    }

    private IEnumerator ResumeDialogueAfterShake(float shakeDuration)
    {
        yield return new WaitForSeconds(shakeDuration);

        // Restore opacity
        SetUIOpacity(DialogueBox, originalDialogueBoxOpacity);
        SetUIOpacity(SpeakerName, 1f);
        SetUIOpacity(DialogueSpeech, 1f);
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
            //player.GetComponent<PlayerMove>().enabled = true; //turns movement back on
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
        public bool IsPraying;
        public bool ShouldFadeIn;
        public bool ShouldShakeBefore;
    }
}