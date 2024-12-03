using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    public Image Background; // Assign the image in the inspector
    [SerializeField]
    private Vector2 originalPosition; // To save the original position of the image
    private Vector2 originalScale;
    private Vector2 targetPosition; // The position to tween to
    [SerializeField]
    private Vector2 targetScale; // The position to tween to
    [SerializeField]
    private float moveDistance = 840f; // Adjust how far the image moves upwards
    [SerializeField]
    private float scaleChange = 1.5f; // Adjust how far the image moves upwards
    [SerializeField]

    private float tweenDuration = 4; // Duration of the tween
    private AnimationCurve tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default easing curve
    void Start()
    {
        originalPosition = Background.rectTransform.anchoredPosition;
        originalScale = Background.rectTransform.localScale;
        targetPosition = originalPosition - new Vector2(0, moveDistance);
        targetScale = originalScale * scaleChange;
        StartCoroutine(moveBackground(Background.rectTransform, targetPosition, targetScale));
    }

    
    void Update()
    {
        
    }

    IEnumerator moveBackground(RectTransform rectTransform, Vector2 targetPosition, Vector2 targetScale){
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 startScale = rectTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < tweenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / tweenDuration;
            float curveValue = tweenCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            rectTransform.localScale = Vector2.Lerp(startScale, targetScale, curveValue);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPosition;
        rectTransform.localScale = targetScale;
    }
}
