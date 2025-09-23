using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class FlashWhite : IBehaviour
{
    SpriteRenderer[] sprites;
    public Material material;
    public Color flashColor = Color.white, healColor = Color.green;
    public float flashDuration = 0.1f;


    public override void Initialize()
    {
        base.Initialize();
        e = GetComponent<Entity>();
        sprites = e.sprites;
        for (int i = 0; i < sprites.Length; i++){
            sprites[i].color = new Color(0, 0, 0, 0);
            sprites[i].material = material;
        }
            
        e.OnHealthChanged += Flash;
    }

    public void Flash(int diff)
    {
        Debug.Log("Flash");
        StopAllCoroutines();
        if (diff > 0){
            StartCoroutine(FlashRoutine(healColor));
        }
        else{
            StartCoroutine(FlashRoutine(flashColor));
        }
        
    }
    public void FlashHealing(){
        StopAllCoroutines();
        // StartCoroutine(Fl)
    }
    IEnumerator FlashRoutine(Color color)
    {
        foreach (var s in sprites)
            s.color = color;

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < sprites.Length; i++)
            sprites[i].color = new Color(0, 0, 0, 0);
    }
}
