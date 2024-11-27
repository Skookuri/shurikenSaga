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
    private AudioSource SpeakerSpeech;
    public float TextSpeed;
    public bool CanContinue;
    private int DialogueIndex;
    private bool isTyping; // Flag to track if dialogue is being typed

    public Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Sprite prayingSprite;
    public ScreenFade screenFade;
    public ScreenShake screenShake;
    public float shakeDuration;
    public bool playOnStart;
    public AudioSource popSFX;
    private float originalDialogueBoxOpacity;

    void Start()
    {
        DialogueIndex = 0;

        // Ensure the player object and its components are properly assigned
        if (player != null)
        {
            spriteRenderer = player.Find("player_art").GetComponent<SpriteRenderer>();
            animator = player.Find("player_art").GetComponent<Animator>();
        }

        // Ensure that an AudioSource component is assigned
        SpeakerSpeech = GetComponent<AudioSource>();
        if (SpeakerSpeech == null)
        {
            Debug.LogError("AudioSource not found on the Dialoguer object.");
        }

        if (playOnStart)
        {
            StartDialogueSegment();
        }
    }

    void Update()
    {
        Skip.enabled = CanContinue;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (CanContinue)
            {
                popSFX.Play();
                DialogueIndex++;
                if (DialogueIndex == DialogueSegments.Length)
                {
                    gameObject.SetActive(false); // Ends display if no more segments
                    return;
                }

                StartDialogueSegment();
            }
            else if (isTyping)
            {
                // Autocomplete the dialogue if it's still typing
                StopAllCoroutines();
                AutocompleteDialogue();
            }
        }
    }

    public void StartDialogueSegment()
    {
        if (DialogueSegments == null || DialogueIndex >= DialogueSegments.Length)
        {
            Debug.LogError("DialogueSegments is not properly initialized or the index is out of bounds.");
            return;
        }

        player.GetComponent<PlayerMove>().enabled = false; // Turn off movement temporarily
        DialogueSegment currentSegment = DialogueSegments[DialogueIndex];

        if (currentSegment.ShouldShakeBefore)
        {
            originalDialogueBoxOpacity = DialogueBox.color.a;
            SetUIOpacity(DialogueBox, 0f);
            SetUIOpacity(SpeakerName, 0f);
            SetUIOpacity(DialogueSpeech, 0f);
            SetUIOpacity(SpeakerImg, 0f);
            screenShake.StartShake(shakeDuration);
            Debug.Log("Starting shake...");
            StartCoroutine(ResumeDialogueAfterShake(shakeDuration));
        }

        if (currentSegment.ShouldFadeIn)
        {
            screenFade.StartFade(0f, 1f); // Fade from black to transparent
        }

        if (currentSegment.IsPraying)
        {
            screenFade.StartFade(0.7f, 1f); // Fade to 70% opacity (half dark screen)
            animator.enabled = false;
            spriteRenderer.sprite = prayingSprite;
            MonkController.MoveMonk = true;
        }

        SetStyle(currentSegment.Character);
        StartCoroutine(PlayDialogue(currentSegment.Dialogue, currentSegment.IsFinalSegment));
    }

    private IEnumerator ResumeDialogueAfterShake(float shakeDuration)
    {
        yield return new WaitForSeconds(shakeDuration);
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
        if (Subject.SpeakerSprite == null) 
        {
            SpeakerImg.color = new Color(0, 0, 0, 0); // Make speaker image invisible
        }
        else
        {
            SpeakerImg.sprite = Subject.SpeakerSprite;
        }

        SpeakerName.SetText(Subject.SpeakerName);

        if (Subject.MumbleClips != null && Subject.MumbleClips.Length > 0) 
        {
            int mumbleIndex = Random.Range(0, Subject.MumbleClips.Length);
            SpeakerSpeech.clip = Subject.MumbleClips[mumbleIndex];
        }
    }

    private IEnumerator PlayDialogue(string Dialogue, bool isFinalSegment)
    {
        if (SpeakerSpeech != null && SpeakerSpeech.clip != null)
        {
            SpeakerSpeech.Play(); // Play mumble clip if available
        }
        CanContinue = false;
        isTyping = true; // Set flag to indicate typing is in progress
        DialogueSpeech.SetText(string.Empty);

        for (int i = 0; i < Dialogue.Length; i++)
        {
            DialogueSpeech.text += Dialogue[i];
            yield return new WaitForSeconds(1f / TextSpeed);
        }

        isTyping = false; // Typing complete
        CanContinue = true;

        if (isFinalSegment)
        {
            screenFade.StartFade(1f, 1f);
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
    }

    private void AutocompleteDialogue()
    {
        DialogueSegment currentSegment = DialogueSegments[DialogueIndex];
        DialogueSpeech.SetText(currentSegment.Dialogue); // Show full dialogue
        isTyping = false; // Stop typing
        CanContinue = true; // Allow continuing to the next segment
    }

    private void OnDisable()
    {
        if (player != null && player.GetComponent<PlayerMove>() != null)
        {
            player.GetComponent<PlayerMove>().enabled = true;
        }
        else
        {
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
