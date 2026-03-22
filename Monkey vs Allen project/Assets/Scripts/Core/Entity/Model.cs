using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

/*
    Why in general structure of Entity, there is seperate gameObject called Model?
    Effect like airbone can adjust Entity's appearance position without changing its logic position 
*/
/// <summary>
/// Position of model is adjusted so that it align well with grid system. Do not change model's position at runtime <br/>
/// </summary>
public class Model : MonoBehaviour {
    [Range(0f, 1f)]
    public float transitionTime;
    public IEntity entity;
    public SortingGroup sortingGroup;
    public Animator animator;
    public AnimatorEvent Event;
    public BoxCollider2D boxCollider;
    private List<SpriteRenderer> sprites;
    public float speedMultiplier = -1;
    public bool isFadeOut = false;
    public string currentStateName { get; private set; }
    void Awake() {
        sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
    }
    public void SetColor(Color color) {
        foreach(var s in sprites) {
            s.color = color;
        }
    }
    void Update() {
        if(isFadeOut) {
            bool isCompletelyFadeOut = true;
            foreach(SpriteRenderer renderer in sprites) {
                Color c = renderer.color;
                c.a -= Time.deltaTime * speedMultiplier;
                if(c.a > 0) {
                    isCompletelyFadeOut = false;
                }
            }
            if(isCompletelyFadeOut) {
                Destroy(this.gameObject);
            }
        }
    }
    public void PlayAnimation(string name) {
        currentStateName = name;
        animator.CrossFade(name, transitionTime, 0, 0);
    }
    public void StartFadeOut(float speedMultiplier, Action afterFadeOutCompleted) {
        this.speedMultiplier = speedMultiplier;
        isFadeOut = true;
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
}