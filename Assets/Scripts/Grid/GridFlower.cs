using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class GridFlower : MonoBehaviour{
    public float targetSize = 1f;
    public float animatingTime = 1f;
    private bool isAnimating = false;
    private new SpriteRenderer renderer;
    public void Initialize(Sprite sprite){
        Wilt();
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
    }
    void Wilt(){
        
    }
    void Bloom(){
        StartCoroutine(BloomCoroutine());
    }
    IEnumerator BloomCoroutine(){
        transform.localScale = Vector3.zero;
        float currentSize = 0;
        while(transform.localScale.magnitude < targetSize){
            currentSize += Time.deltaTime;
            yield return null;
        }
    }
    public bool IsAnimating(){
        return isAnimating;
    }
}