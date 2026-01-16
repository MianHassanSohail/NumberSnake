using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    private GameConfig config;
    private IInputProvider inputProvider;
    private float currentX;
    private float targetX;

    public void Initialize(GameConfig config, IInputProvider inputProvider)
    {
        this.config = config;
        this.inputProvider = inputProvider;
        currentX = transform.position.x;
        targetX = currentX;
    }

    public void UpdateMovement()
    {
        MoveForward();
        HandleHorizontalMovement();
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * config.forwardSpeed * Time.deltaTime, Space.World);
    }

    private void HandleHorizontalMovement()
    {
        float input = inputProvider.GetHorizontalInput();

        // Continuous control - input directly sets target position
        if (inputProvider.IsInputActive())
        {
            targetX = Mathf.Clamp(targetX + input, -config.horizontalBounds, config.horizontalBounds);
        }

        currentX = Mathf.Lerp(currentX, targetX, config.horizontalSpeed * Time.deltaTime);

        // Clamp to platform bounds (hard limit)
        float maxBound = config.platformWidth / 2f;
        currentX = Mathf.Clamp(currentX, -maxBound, maxBound);

        Vector3 pos = transform.position;
        pos.x = currentX;
        transform.position = pos;
    }
}
