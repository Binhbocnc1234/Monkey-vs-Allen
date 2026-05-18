using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class LightRay : MonoBehaviour
{
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Init(
        float length,
        float width,
        float alpha,
        float lifeTime
    )
    {
        transform.localScale = new Vector3(width, length, 1f);

        Color c = sr.color;
        c.a = alpha;
        sr.color = c;

        StartCoroutine(LifeRoutine(lifeTime));
    }

    IEnumerator LifeRoutine(float lifeTime)
    {
        float t = 0f;
        float startAlpha = sr.color.a;

        while (t < lifeTime)
        {
            t += Time.deltaTime;
            float k = 1f - t / lifeTime;

            Color c = sr.color;
            c.a = startAlpha * k;
            sr.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}
