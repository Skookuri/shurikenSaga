using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationBehavior : MonoBehaviour
{
    public Image notifImage; // Assign the image in the inspector
    [SerializeField]
    private string notifText;

    private Vector2 originalPosition; // To save the original position of the image
    private Vector2 targetPosition; // The position to tween to
    [SerializeField]
    private float moveDistance = 100f; // Adjust how far the image moves upwards
    [SerializeField]
    private float tweenDuration; // Duration of the tween
    bool currentlyNotifing = false;

    [SerializeField]
    private AnimationCurve tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default easing curve

    void Start()
    {
        if (notifImage != null)
        {
            notifImage.rectTransform.anchoredPosition = notifImage.rectTransform.anchoredPosition;
            // Cache the original position
            originalPosition = notifImage.rectTransform.anchoredPosition;
            targetPosition = originalPosition - new Vector2(moveDistance, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player" && notifImage != null)
        {
            TextMeshProUGUI textComponent = notifImage.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = notifText; // Set the text
            }

            StopAllCoroutines(); // Stop any ongoing tweens
            StartCoroutine(TweenPosition(notifImage.rectTransform, targetPosition));
        }
    }

    public void startNotif(string text)
    {
        notifText = text;
        TextMeshProUGUI textComponent = notifImage.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = notifText; // Set the text
        }

        if (!currentlyNotifing)
        {
            StartCoroutine(NotificationSequence());
        }
    }

    private IEnumerator NotificationSequence()
    {
        currentlyNotifing = true;

        // Tween to target position
        yield return StartCoroutine(TweenPosition(notifImage.rectTransform, targetPosition));

        // Wait for a specified duration
        yield return StartCoroutine(Wait());

        // Tween back to original position
        yield return StartCoroutine(TweenPosition(notifImage.rectTransform, originalPosition));

        currentlyNotifing = false;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
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
