using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private NumberPickup pickupPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject finishLinePrefab;

    private List<Vector3> occupiedPositions = new List<Vector3>();
    private float minDistanceBetweenObjects = 1.5f; // Minimum distance to avoid overlap

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        occupiedPositions.Clear();
        GeneratePickups();
        GenerateObstacles();
        GenerateFinishLine();
    }

    private Vector3 GetRandomLanePosition(float zPosition)
    {
        int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int lane = Random.Range(0, 3);
            float xPos = (lane - 1) * config.laneWidth;
            Vector3 position = new Vector3(xPos, 0.5f, zPosition);

            // Check if position is far enough from other objects
            bool isFarEnough = true;
            foreach (Vector3 occupied in occupiedPositions)
            {
                if (Vector3.Distance(position, occupied) < minDistanceBetweenObjects)
                {
                    isFarEnough = false;
                    break;
                }
            }

            if (isFarEnough)
            {
                occupiedPositions.Add(position);
                return position;
            }
        }

        // Fallback: just use a random lane if all attempts failed
        int fallbackLane = Random.Range(0, 3);
        float fallbackX = (fallbackLane - 1) * config.laneWidth;
        Vector3 fallbackPos = new Vector3(fallbackX, 0.5f, zPosition);
        occupiedPositions.Add(fallbackPos);
        return fallbackPos;
    }

    private void GeneratePickups()
    {
        for (float z = 10f; z < config.levelLength; z += config.pickupSpacing)
        {
            Vector3 pos = GetRandomLanePosition(z);

            NumberPickup pickup = Instantiate(pickupPrefab, pos, Quaternion.identity);

            // Determine if this should be a negative pickup
            int value;
            if (config.allowNegativePickups && Random.value < config.negativePickupChance)
            {
                // Generate negative value
                value = Random.Range((int)config.negativeValueRange.x, (int)config.negativeValueRange.y + 1);
            }
            else
            {
                // Generate positive value
                value = Random.Range((int)config.pickupValueRange.x, (int)config.pickupValueRange.y);
            }

            pickup.Initialize(config, value);
        }
    }

    private void GenerateObstacles()
    {
        // Start obstacles a bit later and space them differently from pickups
        for (float z = 20f; z < config.levelLength; z += config.obstacleSpacing)
        {
            Vector3 pos = GetRandomLanePosition(z);
            Instantiate(obstaclePrefab, pos, Quaternion.identity);
        }
    }

    private void GenerateFinishLine()
    {
        if (finishLinePrefab != null)
        {
            Vector3 pos = new Vector3(0f, 0f, config.levelLength);
            Instantiate(finishLinePrefab, pos, Quaternion.identity);
        }
    }
}
