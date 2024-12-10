using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class backgroundTween : MonoBehaviour
{
    public Image Background; // Assign the image in the inspector
    [SerializeField]
    private Vector2 originalPosition; // To save the original position of the image
    private Vector2 targetPosition; // The position to tween to
    [SerializeField]
    private float moveDistance = 840f; // Adjust how far the image moves upwards
    [SerializeField]

    private float tweenDuration = 4; // Duration of the tween
    private AnimationCurve tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default easing curve
    void Start()
    {
        originalPosition = Background.rectTransform.anchoredPosition;
        targetPosition = originalPosition + new Vector2(0, moveDistance);
        StartCoroutine(moveBackground(Background.rectTransform, targetPosition));
    }
    // Update is called once per frame
    
    IEnumerator moveBackground(RectTransform rectTransform, Vector2 targetPosition){
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < tweenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / tweenDuration;
            float curveValue = tweenCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPosition;
    }

}
