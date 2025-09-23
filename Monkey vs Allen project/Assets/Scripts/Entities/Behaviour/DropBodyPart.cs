using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DropBodyPart : MonoBehaviour{
    public SpriteRenderer spriteRenderer;
    public Vector2 velocity;
    public float groundPos;
    public bool isFadeOut = false;
    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enabled = false;
    }

    public void Initialize(int laneIndex){
        groundPos = IGrid.Instance.GridToWorldPosition(1, laneIndex).y;
        enabled = true;
        velocity = new Vector2(Random.Range(-3, 3), Random.Range(0, 3));
        this.transform.SetParent(null);
    }
    void Update(){
        
        if (transform.position.y >= groundPos){
            velocity.y -= TechnicalInfo.gravity * Time.deltaTime;
            transform.Translate(velocity);
        }
        else if (isFadeOut == false){
            StartCoroutine(FadeOut(1));
            isFadeOut = true;
        }
    }
    IEnumerator FadeOut(float duration) {
        Color c = spriteRenderer.color;
        float startAlpha = c.a;
        float t = 0f;
        while (t < duration) {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, t / duration);
            spriteRenderer.color = c;
            yield return null;
        }
        c.a = 0f;
        spriteRenderer.color = c;
    }

}