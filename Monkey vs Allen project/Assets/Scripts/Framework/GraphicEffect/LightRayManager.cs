using UnityEngine;

public class LightRayManager : MonoBehaviour
{
    [Header("Concurrency")]
    [Range(8, 24)]
    public int maxConcurrentRays = 16;

    [Header("Spawn")]
    public float spawnInterval = 0.1f;

    [Header("Lifetime")]
    public float averageLifeTime = 1.2f;
    public float lifeTimeVariance = 0.3f;

    [Header("Size")]
    public float averageLength = 3f;
    public float lengthVariance = 0.6f;
    public float averageWidth = 0.5f;
    public float widthVariance = 0.15f;

    [Header("Visual")]
    [Range(0f, 1f)]
    public float averageAlpha = 0.7f;
    public float alphaVariance = 0.2f;

    public LightRay rayPrefab;

    bool emitting;
    float timer;
    int aliveCount;

    void Update()
    {
        if (!emitting) return;
        if (aliveCount >= maxConcurrentRays) return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;

        timer = 0f;
        SpawnRay();
    }

    void SpawnRay()
    {
        LightRay ray = Instantiate(rayPrefab, transform);
        ray.transform.localPosition = Vector3.zero;

        float angle = Random.Range(-180f, 180f);
        ray.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        float length = averageLength + Random.Range(-lengthVariance, lengthVariance);
        float width  = averageWidth  + Random.Range(-widthVariance,  widthVariance);
        float alpha  = averageAlpha  + Random.Range(-alphaVariance,  alphaVariance);
        float life   = averageLifeTime + Random.Range(-lifeTimeVariance, lifeTimeVariance);

        aliveCount++;
        ray.Init(length, width, Mathf.Clamp01(alpha), Mathf.Max(0.1f, life));

        ray.gameObject.AddComponent<RayDeathHook>().Init(this);
    }

    public void StartEmit()
    {
        emitting = true;
    }

    public void StopEmit()
    {
        emitting = false;
    }

    public void NotifyRayDeath()
    {
        aliveCount--;
    }
}
