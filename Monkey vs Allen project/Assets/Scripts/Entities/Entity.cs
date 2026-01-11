using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;


[System.Serializable]
public struct EntityData {
    public string soId;
    public EntityState state;
    public float actionElapsedTime;
    public int lane;
    public float x;
    public List<IEffect> effects;
}
//Huướng thiết kế tương lai
//Entity không reference đến Behaviour
//Behaviour reference đến Entity
//Entity vẫn sử dụng event để thông báo cho các Behaviour
public class Entity : IEntity {
    [Header("Statistics")]
    [ReadOnly] public string entityName;
    [ReadOnly] public int laneIndex;
    [ReadOnly] public int maxHealth;
    [ReadOnly] public int health;
    public int width = 1, height = 1;
    [ReadOnly] public EntityState state;
    public EntityState defaultState = EntityState.Idle;
    [Header("References")]
    public List<IBehaviour> behaviours;
    public EffectController effectController;
    public SortingGroup sortingGroup;
    public AnimatorEvent animatorEvent;
    public Transform model;
    public SpriteRenderer[] sprites;
    public EntitySO so;
    public Action<Entity> OnEntityDeath;
    public Action<int> OnHealthChanged;
    public Action<EntityState> OnStateChanged;
    private Vector2 realPosition;
    // Kiểm tra chết dựa trên máu để giữ nhất quán với TakeDamage/Heal
    private bool isDead = false;
    public override bool IsDead() => isDead;
    public float GetHealthPercentage() => (float)health / maxHealth;

    protected virtual void Awake() { //Không gọi khi chưa vào màn chơi
        sprites = model.GetComponentsInChildren<SpriteRenderer>();
        behaviours = new List<IBehaviour>();
        tribes = new();
        foreach(IBehaviour behav in GetComponents<IBehaviour>()) {
            if(behav.enabled == true) {
                behaviours.Add(behav);
            }
        }
    }
    protected virtual void Update() {
        IGrid grid = GridSystem.Ins;
        Vector2Int gridPos = grid.WorldToGridPosition(GetWorldPosition());
        if(!grid.IsValidGridPosition(gridPos.x, gridPos.y)) {
            Die();
        }
    }
    public override Vector2 GetWorldPosition() {
        if(state == EntityState.Attacking) {
            return realPosition;
        }
        else {
            return transform.position;
        }
    }
    public override IEffectable GetEffectController() => effectController;
    public override void TakeDamage(DamageContext ctx) {
        if(IsDead()) return;
        effectController.ProcessDamageInput(ctx);
        if(ctx.amount <= 0) { return; }
        health -= ctx.amount;
        effectController.ProcessDamageTaken(ctx);
        OnHealthChanged?.Invoke(-ctx.amount);
        if(health <= 0) {
            health = 0;
            Die();
        }
    }
    public void Heal(int healAmount) {
        if(IsDead()) return;
        health = Mathf.Min(health + healAmount, maxHealth);
        OnHealthChanged?.Invoke(healAmount);
    }

    public override void Die() {
        if(IsDead()) { return; }
        isDead = true;
        SetEntityState(EntityState.Death);
        OnEntityDeath?.Invoke(this);
    }

    /// <summary>
    /// Tranfer data from SO to this Entity
    /// </summary>
    public override void Initialize(EntitySO so, Team team) {
        this.so = so;
        this.team = team;
        this.tribes = so.tribes;
        maxHealth = so.health;
        health = so.health;
        width = so.width; height = so.height;

        Vector2Int gridPos = IGrid.Ins.WorldToGridPosition(this.transform.position);
        foreach(IBehaviour bev in behaviours) {
            bev.Initialize();
        }
        foreach(TraitType trait in so.traits) {

        }

        if(Application.isPlaying) {
            laneIndex = gridPos.y; sortingGroup.sortingOrder = height - laneIndex;
            IGrid grid = GridSystem.Ins;
            if(width != 1 || height != 1) {
                //Adjust transform.position so the tower appear behind 
                transform.position = grid.GridToWorldPosition(gridPos) + (new Vector2(width / 2 - grid.cellSize / 2, height / 2 - grid.cellSize / 2));
                //Add this entity to every lanes it occupy
                // for(int i = laneIndex; i < laneIndex + width; ++i){
                //     EContainer.AddEntity(this, laneIndex);
                // }
            }
        }
    }
    public float Distance(Entity other) {
        return Mathf.Abs(this.GetWorldPosition().x - other.GetWorldPosition().x);
    }
    public override void SetEntityState(EntityState state) {
        if(this.state == state) { return; }
        this.state = state;
        OnStateChanged?.Invoke(state);
        if(state == EntityState.Idle) {
            animator.Play("Idle");
            foreach(IBehaviour behav in behaviours) {
                behav.SetBehaviourEnable(true);
            }
        }
        else if(state == EntityState.Walk) {
            animator.Play("Walk");
        }
        else if(state == EntityState.Attacking) {
            animator.Play("Attack", 0, 0f);
            realPosition = transform.position;
        }
        else if(state == EntityState.Death) {
            foreach(IBehaviour behav in behaviours) {
                behav.SetBehaviourEnable(false);
            }
        }
        else if(state == EntityState.InActive) {
            animator.Play("Idle");
            foreach(IBehaviour behav in behaviours) {
                behav.SetBehaviourEnable(false);
            }
        }
        else if(state == EntityState.Frozen) {
            foreach(IBehaviour behav in behaviours) {
                behav.SetBehaviourEnable(false);
            }
            animator.enabled = false;
        }
    }
    public void ReturnToDefaultState() {
        SetEntityState(defaultState);
    }
    public override EntityState GetEntityState() {
        return state;
    }
    public int GetDangerPoint() {
        int total = health / 6;
        foreach(IBehaviour behav in behaviours) {
            total += behav.dangerPoint;
        }
        return total;
    }

}
