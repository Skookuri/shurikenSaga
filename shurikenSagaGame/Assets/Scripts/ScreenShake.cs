using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public void StartShake(float duration)
    {
        StartCoroutine(ShakeScreen(duration));
    }

    private IEnumerator ShakeScreen(float duration)
    {
        Vector3 originalPosition = Camera.main.transform.position;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float xOffset = Random.Range(-0.1f, 0.1f);
            float yOffset = Random.Range(-0.1f, 0.1f);
            Camera.main.transform.position = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);
            yield return null;
        }
        Camera.main.transform.position = originalPosition;
    }
}
