using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameEvents
{
    public UnityEvent<int> OnNumberCollected = new UnityEvent<int>();
    public UnityEvent OnObstacleHit = new UnityEvent();
    public UnityEvent<int> OnGameOver = new UnityEvent<int>();
    public UnityEvent<int> OnLevelComplete = new UnityEvent<int>();
    public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
}

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("EventManager");
                instance = go.AddComponent<EventManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public readonly GameEvents Events = new GameEvents();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
