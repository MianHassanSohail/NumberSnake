using UnityEngine;
[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    [Header("Player Movement")]
    public float forwardSpeed = 5f;
    public float horizontalSpeed = 10f;
    public float horizontalBounds = 3f;
    public float platformWidth = 6f; // Total platform width

    [Header("Controls")]
    public float mobileSensitivity = 0.1f;  // For mobile devices
    public float editorSensitivity = 0.02f;  // For Unity Editor
    public bool useAccelerometer = false; // Optional tilt controls

    [Header("Chain Settings")]
    public float editorchainSpacing = 1.5f;
    public float mobileChainSpacing = 0.5f;
    public int maxPathHistorySize = 200; // Increased to support longer chains
    public float chainFollowSpeed = 15f;

    [Header("Level Generation")]
    public float levelLength = 100f;
    public float pickupSpacing = 10f;
    public float obstacleSpacing = 15f;
    public float laneWidth = 2f;
    public Vector2 pickupValueRange = new Vector2(2, 6);
    public bool allowNegativePickups = true; // Enable negative number pickups
    public Vector2 negativeValueRange = new Vector2(-3, -1); // Range for negative pickups
    public float negativePickupChance = 0.3f; // 30% chance of negative pickup

    [Header("Visual Effects")]
    public float pickupRotationSpeed = 50f;
    public float pickupBobSpeed = 2f;
    public float pickupBobHeight = 0.3f;
    public float obstaclePulseSpeed = 2f;
    public float obstaclePulseAmount = 0.1f;
}

