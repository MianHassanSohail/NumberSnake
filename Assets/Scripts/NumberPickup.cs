using TMPro;
using UnityEngine;

public class NumberPickup : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private int value = 2;
    [SerializeField] bool rotation;
    private GameConfig config;
    private Vector3 startPos;
    private ObjectPool<NumberPickup> parentPool;
    private Renderer objectRenderer;

    public void Initialize(GameConfig config, int value)
    {
        this.config = config;
        this.value = value;
        startPos = transform.position;

        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshPro>();

        if (objectRenderer == null)
            objectRenderer = GetComponent<Renderer>();

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (textMesh != null)
        {
            // Display with + or - sign
            string sign = value >= 0 ? "+" : "";
            textMesh.text = sign + value.ToString();

            // Color: Green for positive, Red for negative
            textMesh.color = value >= 0 ? Color.green : Color.red;
        }

        // Change material color based on value
        if (objectRenderer != null && objectRenderer.material != null)
        {
            objectRenderer.material.color = value >= 0 ?
                new Color(0.2f, 1f, 0.2f) : // Green for positive
                new Color(1f, 0.2f, 0.2f);   // Red for negative
        }
    }

    private void Update()
    {
        if (config == null) return;

        if(rotation)
        transform.Rotate(Vector3.up, config.pickupRotationSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * config.pickupBobSpeed) * config.pickupBobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void Collect()
    {
        Destroy(gameObject);
    }

    public int GetValue() => value;
    public void SetPool(ObjectPool<NumberPickup> pool) => parentPool = pool;
}
