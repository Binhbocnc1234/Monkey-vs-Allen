using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : EntityAppearance{
    public float duration;
    public Material defaultMaterial;
    public override void Initialize(){
        base.Initialize();
        model.e.OnEntityDeath += () => StartCoroutine(DeathCoroutine());
    }
    IEnumerator DeathCoroutine()
    {
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (e.team == Team.Left ? Vector3.left : Vector3.right) * 2f; // move left by 2 units
        foreach(var renderer in model.GetSprites()){
            renderer.material = defaultMaterial;
        }
        Color startColor = Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move left
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            // Fade alpha
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            foreach(var renderer in model.GetSprites()){
                renderer.color = c;
            }

            yield return null;
        }
        Destroy(this.gameObject);
    }

}