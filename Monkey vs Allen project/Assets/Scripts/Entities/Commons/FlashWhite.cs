using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Entity))]
public class FlashWhite : MonoBehaviour
{
    SpriteRenderer[] sprites;
    public Material material;
    public static Color flashColor = new Color(1, 1, 1, 0.6f), healColor = new Color(0, 0.7f, 0, 0.6f);
    public const float flashDuration = 0.1f;
    Entity e;
    void Start()
    {
        e = GetComponent<Entity>();
        sprites = e.model.GetSprites();
        for (int i = 0; i < sprites.Length; i++){
            sprites[i].color = new Color(0, 0, 0, 0);
            sprites[i].material = material;
        }
        e.OnHealthChanged += Flash;
    }
    public void Flash(float diff)
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(e.model.GetSprites(), diff));
    }
    public static IEnumerator FlashRoutine(SpriteRenderer[] sprites, float diff)
    {
        if(diff == 0) yield break;
        Color color = Color.green;
        if(diff < 0) {
            color = Color.white;
        }
        color.a = Mathf.Clamp(0.3f + Mathf.Abs(diff)/150, 0, 1);
        foreach (var s in sprites)
            s.color = color;

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < sprites.Length; i++) {
            if(sprites[i] == null) continue;
            sprites[i].color = new Color(0, 0, 0, 0);
        }
    }
}
