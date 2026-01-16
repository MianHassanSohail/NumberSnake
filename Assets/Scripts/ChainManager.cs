using UnityEngine;
using System.Collections.Generic;

public class ChainManager : MonoBehaviour
{
    private ObjectPool<ChainNumber> numberPool;
    private List<ChainNumber> activeChain = new List<ChainNumber>();
    private GameConfig config;

    public void Initialize(ChainNumber prefab, GameConfig config, int poolSize = 20)
    {
        this.config = config;
        numberPool = new ObjectPool<ChainNumber>(prefab, poolSize, transform);
    }

    public void UpdateChain(PathRecorder pathRecorder)
    {
#if UNITY_EDITOR
        int spacing = Mathf.RoundToInt(config.editorchainSpacing * 10);
#else
        int spacing = Mathf.RoundToInt(config.mobileChainSpacing * 10);
#endif
        for (int i = 0; i < activeChain.Count; i++)
        {
            int stepsBack = (i + 1) * spacing;

            if (pathRecorder.TryGetPosition(stepsBack, out Vector3 targetPos))
            {
                activeChain[i].transform.position = Vector3.Lerp(
                    activeChain[i].transform.position,
                    targetPos,
                    config.chainFollowSpeed * Time.deltaTime
                );
            }
        }
    }

    public void AddNumbers(int count, int startValue)
    {
        for (int i = 0; i < count; i++)
        {
            ChainNumber num = numberPool.Get();
            num.SetValue(startValue - i - 1);
            num.transform.position = transform.position;
            activeChain.Add(num);
        }
    }

    public void RebuildChain(int newValue, PathRecorder pathRecorder)
    {
#if UNITY_EDITOR
        int spacing = Mathf.RoundToInt(config.editorchainSpacing * 10);
#else
        int spacing = Mathf.RoundToInt(config.mobileChainSpacing * 10);
#endif
        // Keep existing chain numbers that are still valid
        int oldChainCount = activeChain.Count;
        int newChainCount = newValue - 1;

        if (newChainCount > oldChainCount)
        {
            // Adding numbers - add new ones progressively along the path
            for (int i = oldChainCount; i < newChainCount; i++)
            {
                ChainNumber num = numberPool.Get();
                int value = newValue - 1 - i;
                num.SetValue(value);

                // Position new numbers along the existing path, close to the last chain element
                int stepsBack = (i + 1) * spacing;

                if (pathRecorder.TryGetPosition(stepsBack, out Vector3 pathPos))
                {
                    num.transform.position = pathPos;
                }
                else if (activeChain.Count > 0)
                {
                    // If path doesn't have enough history, place near the last chain number
                    num.transform.position = activeChain[activeChain.Count - 1].transform.position - Vector3.forward * 0.5f;
                }
                else
                {
                    // Fallback to player position with offset
                    num.transform.position = transform.position - Vector3.forward * (i + 1) * 0.5f;
                }

                activeChain.Add(num);
            }

            // Update all values in case they changed
            for (int i = 0; i < activeChain.Count; i++)
            {
                activeChain[i].SetValue(newValue - 1 - i);
            }
        }
        else if (newChainCount < oldChainCount)
        {
            // Removing numbers - remove from the end
            while (activeChain.Count > newChainCount)
            {
                ChainNumber last = activeChain[activeChain.Count - 1];
                activeChain.RemoveAt(activeChain.Count - 1);
                numberPool.Return(last);
            }

            // Update remaining values
            for (int i = 0; i < activeChain.Count; i++)
            {
                activeChain[i].SetValue(newValue - 1 - i);
            }
        }
        else
        {
            // Same count, just update values
            for (int i = 0; i < activeChain.Count; i++)
            {
                activeChain[i].SetValue(newValue - 1 - i);
            }
        }
    }

    public void RemoveLastNumber()
    {
        if (activeChain.Count > 0)
        {
            // Get the last number in the chain
            ChainNumber last = activeChain[activeChain.Count - 1];
            activeChain.RemoveAt(activeChain.Count - 1);

            // Smoothly fade/shrink before returning to pool
            StartCoroutine(SmoothRemove(last));
        }
    }

    private System.Collections.IEnumerator SmoothRemove(ChainNumber number)
    {
        Vector3 originalScale = number.transform.localScale;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Shrink to zero
            number.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

            yield return null;
        }

        // Reset scale and return to pool
        number.transform.localScale = originalScale;
        numberPool.Return(number);
    }

    public void Clear()
    {
        numberPool.ReturnAll(activeChain);
    }
}
