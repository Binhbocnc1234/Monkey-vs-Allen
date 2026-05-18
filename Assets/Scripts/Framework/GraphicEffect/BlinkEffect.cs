using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlinkEffect : MonoBehaviour
{
    public float blinkInterval = 0.5f;
    public Color semiBlackColor = new Color(0.8f, 0.8f, 0.8f, 1);
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    void Update()
    {
        if (spriteRenderer == null || blinkInterval <= 0f)
        {
            return;
        }

        float t = Mathf.PingPong(Time.time / blinkInterval, 1f);
        spriteRenderer.color = Color.Lerp(Color.white, semiBlackColor, t);
    }
}
