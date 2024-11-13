using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage; // Drag and drop your Image component here in the Inspector

    public void StartFade(float targetAlpha, float duration)
    {
        StartCoroutine(FadeScreen(targetAlpha, duration));
    }

    private IEnumerator FadeScreen(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Color newColor = fadeImage.color;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            fadeImage.color = newColor;
            yield return null;
        }
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }
}
