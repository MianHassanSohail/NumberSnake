using UnityEngine;

public class ParticleEffectManager : MonoBehaviour
{
    private static ParticleEffectManager instance;
    public static ParticleEffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ParticleEffectManager");
                instance = go.AddComponent<ParticleEffectManager>();
            }
            return instance;
        }
    }

    [Header("Particle Prefabs")]
    [SerializeField] private GameObject collectEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 10;

    private ObjectPool<ParticleSystem> collectPool;
    private ObjectPool<ParticleSystem> hitPool;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InitializePools();
    }

    private void InitializePools()
    {
        if (collectEffectPrefab != null)
        {
            ParticleSystem collectPs = collectEffectPrefab.GetComponent<ParticleSystem>();
            if (collectPs != null)
            {
                collectPool = new ObjectPool<ParticleSystem>(collectPs, poolSize, transform);
            }
        }

        if (hitEffectPrefab != null)
        {
            ParticleSystem hitPs = hitEffectPrefab.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                hitPool = new ObjectPool<ParticleSystem>(hitPs, poolSize, transform);
            }
        }
    }

    public void PlayCollectEffect(Vector3 position)
    {
        if (collectPool != null)
        {
            ParticleSystem ps = collectPool.Get();
            ps.transform.position = position;
            ps.Play();

            // Return to pool after particle duration
            StartCoroutine(ReturnToPoolAfterDelay(ps, collectPool, ps.main.duration));
        }
        else if (collectEffectPrefab != null)
        {
            // Fallback if pool not initialized
            GameObject effect = Instantiate(collectEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public void PlayHitEffect(Vector3 position)
    {
        if (hitPool != null)
        {
            ParticleSystem ps = hitPool.Get();
            ps.transform.position = position;
            ps.Play();

            StartCoroutine(ReturnToPoolAfterDelay(ps, hitPool, ps.main.duration));
        }
        else if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(ParticleSystem ps, ObjectPool<ParticleSystem> pool, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ps != null && pool != null)
        {
            ps.Stop();
            pool.Return(ps);
        }
    }
}
