using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;

    private Vector3 originalScale;
    private Material material;
    private Color originalColor;

    private void Start()
    {
        originalScale = transform.localScale;

        // Get material for color pulsing
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            originalColor = material.color;
        }
    }

    private void Update()
    {
        // Pulse scale
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scale;

        // Pulse color intensity (optional)
        if (material != null)
        {
            float intensity = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
            material.color = Color.Lerp(Color.red, originalColor, intensity);
        }
    }

    private void OnDestroy()
    {
        // Clean up material instance to avoid memory leak
        if (material != null)
        {
            Destroy(material);
        }
    }
}