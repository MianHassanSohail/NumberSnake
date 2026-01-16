using UnityEngine;
using System.Collections;

public class NumberScaleEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Coroutine currentEffect;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void PlayPunchEffect(float duration = 0.3f, float punchScale = 1.5f)
    {
        Debug.Log("Playing punch scale effect");
        if (currentEffect != null)
        {
            StopCoroutine(currentEffect);
        }
        currentEffect = StartCoroutine(PunchScaleCoroutine(duration, punchScale));
    }

    private IEnumerator PunchScaleCoroutine(float duration, float punchScale)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Fast scale up, then bounce back down
            float scale;
            if (progress < 0.3f)
            {
                // Quick scale up
                scale = Mathf.Lerp(1f, punchScale, progress / 0.3f);
            }
            else
            {
                // Bounce back with overshoot
                float bounceProgress = (progress - 0.3f) / 0.7f;
                scale = Mathf.Lerp(punchScale, 1f, bounceProgress);

                // Add slight overshoot at the end
                if (bounceProgress > 0.8f)
                {
                    float overshoot = Mathf.Sin((bounceProgress - 0.8f) * Mathf.PI * 5f) * 0.1f;
                    scale += overshoot;
                }
            }

            transform.localScale = originalScale * scale;

            yield return null;
        }

        // Ensure we end at original scale
        transform.localScale = originalScale;
        currentEffect = null;
    }
}
