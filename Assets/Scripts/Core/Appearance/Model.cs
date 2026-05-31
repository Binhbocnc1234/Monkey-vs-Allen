using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SortingGroup)), RequireComponent(typeof(Animator))]
public abstract class Model : MonoBehaviour {
    // Thời gian để chuyển từ một state này sang một state khác
    public const float transitionTime = 0.25f;
    [HideInInspector] public SortingGroup sortingGroup;
    [HideInInspector] public Animator animator;
    public BoxCollider2D boxCollider;
    protected List<SpriteRenderer> sprites;
    protected string currentStateName;
    protected virtual void Awake() {
        sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        animator = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
        if(boxCollider == null) {
            Debug.LogError("[Model] No collider assigned");
        }
        else {
            boxCollider.enabled = false;
        }
    }
    public void SetColor(Color color) {
        foreach(var s in sprites) {
            s.color = color;
        }
    }
    void Update() {
        // if(isFadeOut) {
        //     bool isCompletelyFadeOut = true;
        //     foreach(SpriteRenderer renderer in sprites) {
        //         Color c = renderer.color;
        //         c.a -= Time.deltaTime * speedMultiplier;
        //         if(c.a > 0) {
        //             isCompletelyFadeOut = false;
        //         }
        //     }
        //     if(isCompletelyFadeOut) {
        //         Destroy(this.gameObject);
        //     }
        // }
    }
    public void PlayAnimation(string name) {
        currentStateName = name;
        animator.CrossFade(name, transitionTime, 0, 0);
    }
    public void SetMaterial(Material material) {
        foreach(var s in sprites) {
            s.material = material;
        }
    }
    public SpriteRenderer[] GetSprites() {
        sprites.RemoveAll(s => s == null);
        return sprites.ToArray();
    }
    public Vector2 GetPosition() => transform.position;
    public Bounds GetBound() => boxCollider.bounds;
}