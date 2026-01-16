using UnityEngine;

public interface IInputProvider
{
    float GetHorizontalInput();
    bool IsInputActive();
}

// Smooth drag control - best for free horizontal movement
public class DragInputProvider : IInputProvider
{
    private Vector2 lastTouchPos;
    private float currentDelta;
    private bool isDragging;
    private readonly float sensitivity;

    public DragInputProvider(float sensitivity = 0.02f)
    {
        this.sensitivity = sensitivity;
    }

    public float GetHorizontalInput()
    {
        currentDelta = 0f;

        // Touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                currentDelta = (touch.position.x - lastTouchPos.x) * sensitivity;
                lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                currentDelta = 0f;
            }

            return currentDelta;
        }

        // Mouse input for editor testing
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            currentDelta = (currentPos.x - lastTouchPos.x) * sensitivity;
            lastTouchPos = currentPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            currentDelta = 0f;
        }
#endif

        return currentDelta;
    }

    public bool IsInputActive()
    {
        return isDragging;
    }
}

// Alternative: Tilt/Accelerometer control
public class AccelerometerInputProvider : IInputProvider
{
    private readonly float sensitivity;
    private Vector3 calibrationAcceleration;
    private bool isCalibrated;

    public AccelerometerInputProvider(float sensitivity)
    {
        this.sensitivity = sensitivity;
        Calibrate();
    }

    public void Calibrate()
    {
        calibrationAcceleration = Input.acceleration;
        isCalibrated = true;
    }

    public float GetHorizontalInput()
    {
        if (!isCalibrated) return 0f;

        Vector3 currentAccel = Input.acceleration;
        float tilt = (currentAccel.x - calibrationAcceleration.x) * sensitivity;

        return Mathf.Clamp(tilt, -1f, 1f);
    }

    public bool IsInputActive()
    {
        return Mathf.Abs(Input.acceleration.x - calibrationAcceleration.x) > 0.05f;
    }
}

// Continuous input - hold and drag smoothly
public class TouchInputProvider : IInputProvider
{
    private Vector2 touchStartPos;
    private Vector2 currentTouchPos;
    private bool isDragging;
    private readonly float sensitivity;
    private readonly float screenWidthHalf;

    public TouchInputProvider(float sensitivity)
    {
        this.sensitivity = sensitivity;
        this.screenWidthHalf = Screen.width / 2f;
    }

    public float GetHorizontalInput()
    {
        // Mobile touch - continuous position-based control
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                currentTouchPos = touch.position;
                isDragging = true;
            }
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
            {
                currentTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                return 0f;
            }

            if (isDragging)
            {
                // Calculate offset from center (continuous control)
                float offsetFromStart = currentTouchPos.x - touchStartPos.x;
                float normalizedOffset = offsetFromStart / screenWidthHalf;
                return normalizedOffset * sensitivity;
            }
        }

        // Mouse for editor - continuous
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            currentTouchPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            currentTouchPos = Input.mousePosition;

            float offsetFromStart = currentTouchPos.x - touchStartPos.x;
            float normalizedOffset = offsetFromStart / screenWidthHalf;
            return normalizedOffset * sensitivity;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
#endif

        return 0f;
    }

    public bool IsInputActive()
    {
        return isDragging;
    }
}