using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;
    public static CameraShake Instance
    {
        get
        {
            if (instance == null)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    instance = cam.gameObject.GetComponent<CameraShake>();
                    if (instance == null)
                        instance = cam.gameObject.AddComponent<CameraShake>();
                }
            }
            return instance;
        }
    }

    private Vector3 originalPosition;
    private bool isShaking = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void Shake(float duration = 0.3f, float magnitude = 0.2f)
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        isShaking = true;
        originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            magnitude = Mathf.Lerp(magnitude, 0f, elapsed / duration);

            yield return null;
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }
}
