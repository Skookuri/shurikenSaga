using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class SignBehavior : MonoBehaviour
{
    public Image SignImage; // Assign the image in the inspector
    [SerializeField]
    private string signText;

    private Vector2 originalPosition; // To save the original position of the image
    private Vector2 targetPosition; // The position to tween to
    [SerializeField]
    private float moveDistance; // Adjust how far the image moves upwards
    [SerializeField]
    private float tweenDuration = 0.5f; // Duration of the tween

    private GameHandler gh;

    [SerializeField]
    private AnimationCurve tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default easing curve

    void Start()
    {
        gh = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        if (SignImage != null)
        {
            // Cache the original position
            originalPosition = SignImage.rectTransform.anchoredPosition;
            targetPosition = originalPosition + new Vector2(0, moveDistance);
        }
    }

    private void Update()
    {
        if (gh.switching)
        {
            SignImage.rectTransform.anchoredPosition = originalPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player" && SignImage != null)
        {
            TextMeshProUGUI textComponent = SignImage.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = signText; // Set the text
            }

            StopAllCoroutines(); // Stop any ongoing tweens
            StartCoroutine(TweenPosition(SignImage.rectTransform, targetPosition));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player" && SignImage != null)
        {
            StopAllCoroutines(); // Stop any ongoing tweens
            StartCoroutine(TweenPosition(SignImage.rectTransform, originalPosition));
        }
    }

    private IEnumerator TweenPosition(RectTransform rectTransform, Vector2 target)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < tweenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / tweenDuration;
            float curveValue = tweenCurve.Evaluate(t); // Apply the curve
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, target, curveValue);
            yield return null; // Wait for the next frame
        }

        // Ensure the final position is set accurately
        rectTransform.anchoredPosition = target;
    }
}
