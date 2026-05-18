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
public class Model : MonoBehaviour, IModel {
    [Range(0f, 1f)]
    public float transitionTime;
    [ReadOnly] public IEntity e;
    public SortingGroup sortingGroup;
    public Animator animator;
    public AnimatorEvent Event;
    public BoxCollider2D boxCollider;
    private List<SpriteRenderer> sprites;
    public float speedMultiplier = -1;
    public bool isFadeOut = false;
    public string currentStateName { get; private set; }
    private Vector3 initPosition;
    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        initPosition = transform.position;
    }
    public void AssignEntity(IEntity e) {
        this.e = e;
        this.sortingGroup.sortingOrder = 1 - e.lane;
        if(e.team == Team.Right) {
            transform.FlipLocalScaleX();
        }
        e.OnHealthChanged += (changedAmount) => {
            if(changedAmount < 0) {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float weight = 0;
                if(stateInfo.IsName("Attack")) {
                    weight = 0.5f;
                }
                else if(stateInfo.IsName("Idle") || stateInfo.IsName("Walk")) {
                    weight = 1f;
                }
                animator.SetLayerWeight(1, weight);
                PlayAnimation("Hurt Layer.Hurt");
            }
        };
        e.OnBehaviorActive += (behav) => {
            PlayAnimation(behav.GetAnimatorStateName());
            if(behav.GetAnimatorStateName() == "Attack") {
                animator.SetFloat("AttackSpeed", e[ST.AttackSpeed] / e.GetSO().attackSpeed);
            }
        };
        foreach(EntityAppearance appearance in GetComponents<EntityAppearance>()) {
            appearance.Initialize();
        }
    }
    public void SetColor(Color color) {
        foreach(var s in sprites) {
            s.color = color;
        }
        if(transform.hasChanged) {
            if (transform.position != initPosition) {
                Debug.LogError("[Model] Do not change transform.position at runtime");
            }
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
    public SpriteRenderer[] GetSprites()
    {
        sprites.RemoveAll(s => s == null);
        return sprites.ToArray();
    }
    public Vector2 GetPosition() => transform.position;
    public void SetPosition(Vector2 pos){ transform.position = pos; }
    public Bounds GetBound() => boxCollider.bounds;
    void OnValidate() {
        
    }
}