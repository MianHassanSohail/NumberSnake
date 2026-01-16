using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private ChainNumber chainNumberPrefab;

    private PlayerMovement movement;
    private ChainManager chainManager;
    private PathRecorder pathRecorder;
    private IInputProvider inputProvider;
    private NumberScaleEffect scaleEffect;
    private int currentValue = 1;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        chainManager = gameObject.AddComponent<ChainManager>();

        // Add scale effect to the number text
        TextMeshPro textMesh = GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            scaleEffect = textMesh.gameObject.AddComponent<NumberScaleEffect>();
        }

        inputProvider = new TouchInputProvider(config.touchinputSensitivity);
        pathRecorder = new PathRecorder(config.maxPathHistorySize);

        movement.Initialize(config, inputProvider);
        chainManager.Initialize(chainNumberPrefab, config);

        UpdateHeadDisplay();
    }

    private void Update()
    {
        movement.UpdateMovement();
        pathRecorder.RecordPosition(transform.position);
        chainManager.UpdateChain(pathRecorder);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            HandlePickupCollision(other);
        }
        else if (other.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(other);
        }
        else if (other.CompareTag("Finish"))
        {
            HandleFinishLine();
        }
    }

    private void HandlePickupCollision(Collider other)
    {
        NumberPickup pickup = other.GetComponent<NumberPickup>();
        if (pickup != null)
        {
            int value = pickup.GetValue();
            int previousValue = currentValue;

            // Add or subtract the value (supports negative pickups)
            currentValue += value;

            // Check for game over
            if (currentValue <= 0)
            {
                currentValue = 0;
                EventManager.Instance.Events.OnGameOver.Invoke(0);
                pickup.Collect();
                return;
            }

            chainManager.RebuildChain(currentValue, pathRecorder);
            UpdateHeadDisplay();

            // Play scale punch effect on player number
            if (scaleEffect != null)
            {
                float punchScale = value > 0 ? 1.8f : 1.5f; // Bigger for positive
                scaleEffect.PlayPunchEffect(0.3f, punchScale);
            }


            // Camera shake for negative pickups
            if (value < 0)
            {
                CameraShake.Instance?.Shake(0.3f, 0.15f);
            }

            EventManager.Instance.Events.OnNumberCollected.Invoke(value);
            EventManager.Instance.Events.OnScoreChanged.Invoke(currentValue);

            pickup.Collect();
        }
    }

    private void HandleObstacleCollision(Collider other)
    {
        if (currentValue > 1)
        {
            currentValue--;
            chainManager.RemoveLastNumber();
            UpdateHeadDisplay();

            // Play scale punch effect
            if (scaleEffect != null)
            {
                scaleEffect.PlayPunchEffect(0.25f, 1.3f);
            }

            EventManager.Instance.Events.OnObstacleHit.Invoke();
            EventManager.Instance.Events.OnScoreChanged.Invoke(currentValue);

            // Camera shake on obstacle hit
            CameraShake.Instance?.Shake(0.4f, 0.25f);
        }
        else
        {
            // Game over if hitting obstacle with value 1
            currentValue = 0;
            EventManager.Instance.Events.OnGameOver.Invoke(0);
        }

        Destroy(other.gameObject);
    }

 
    private void HandleFinishLine()
    {
        EventManager.Instance.Events.OnLevelComplete.Invoke(currentValue);
    }

    private void UpdateHeadDisplay()
    {
        TextMeshPro textMesh = GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = currentValue.ToString();
        }
    }

    public int GetCurrentValue() => currentValue;
}
