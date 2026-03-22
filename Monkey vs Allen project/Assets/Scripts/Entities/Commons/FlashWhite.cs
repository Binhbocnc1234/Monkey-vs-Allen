using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class FlashWhite : MonoBehaviour
{
    SpriteRenderer[] sprites;
    public Material material;
    public Color flashColor = Color.white, healColor = Color.green;
    public float flashDuration = 0.1f;
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
        if (diff > 0){
            StartCoroutine(FlashRoutine(healColor));
        }
        else{
            StartCoroutine(FlashRoutine(flashColor));
        }
    }
    IEnumerator FlashRoutine(Color color)
    {
        sprites = e.model.GetComponentsInChildren<SpriteRenderer>();
        foreach (var s in sprites)
            s.color = color;

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < sprites.Length; i++)
            sprites[i].color = new Color(0, 0, 0, 0);
    }
}
