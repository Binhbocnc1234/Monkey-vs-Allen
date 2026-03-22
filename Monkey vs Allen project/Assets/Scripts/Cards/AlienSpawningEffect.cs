using UnityEngine;

public class AlienSpawningEffect : MonoBehaviour
{
    [Header("Timing")]
    public float lifeTime = 2f;
    public float scaleUpTime = 0.3f;
    public float scaleDownTime = 0.3f;

    [Header("Rotation")]
    public float rotateSpeed = 180f;

    private float timer;
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        timer += Time.deltaTime;

        Rotate();
        HandleScaling();

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void Rotate()
    {
        transform.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
    }

    void HandleScaling()
    {
        // Scale up
        if (timer <= scaleUpTime)
        {
            float t = timer / scaleUpTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
        }
        // Scale down
        else if (timer >= lifeTime - scaleDownTime)
        {
            float t = (timer - (lifeTime - scaleDownTime)) / scaleDownTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
        }
        // Normal phase
        else
        {
            transform.localScale = originalScale;
        }
    }
}