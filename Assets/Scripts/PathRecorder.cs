using UnityEngine;

public class PathRecorder
{
    private Vector3[] pathBuffer;
    private int currentIndex;
    private int count;
    private readonly int capacity;

    public PathRecorder(int capacity)
    {
        this.capacity = capacity;
        pathBuffer = new Vector3[capacity];
        currentIndex = 0;
        count = 0;
    }

    public void RecordPosition(Vector3 position)
    {
        pathBuffer[currentIndex] = position;
        currentIndex = (currentIndex + 1) % capacity;
        if (count < capacity)
            count++;
    }

    public bool TryGetPosition(int stepsBack, out Vector3 position)
    {
        if (stepsBack >= count)
        {
            position = Vector3.zero;
            return false;
        }

        int index = (currentIndex - 1 - stepsBack + capacity) % capacity;
        position = pathBuffer[index];
        return true;
    }

    public int Count => count;

    public void Clear()
    {
        currentIndex = 0;
        count = 0;
    }
}
