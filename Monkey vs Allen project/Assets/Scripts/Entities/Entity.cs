using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;

//Huướng thiết kế tương lai
//Entity không reference đến Behaviour
//Behaviour reference đến Entity
//Entity vẫn sử dụng event để thông báo cho các Behaviour
public class Entity : MonoBehaviourWithContainer<Entity>
{
    public static List<Entity> targetMonkeys = new List<Entity>(), targetEnemies = new List<Entity>();
    [Header("Statistics")]
    public Team team;
    public string entityName;
    public EntityType entityType;
    public int laneIndex;
    public int maxHealth;
    public int health;
    public int width = 1, height = 1;
    public bool canAttack, canMove;
    [ReadOnly] public EntityState state;
    [Header("Others")]
    [HideInInspector] public List<IBehaviour> behaviours;
    protected EntityState entityState;
    public Animator animator;
    public SortingGroup sortingGroup;
    public AnimatorEvent animatorEvent;
    public SpriteRenderer[] sprites;
    public EntitySO so;
    public Action<Entity> OnEntityDeath;
    public Action<int> OnHealthChanged;
    public Action<EntityState> OnStateChanged;
    private bool isInited = false;
    private Vector2 realPosition;
    public bool IsDead() => health == 0;
    public float GetHealthPercentage() => (float)health / maxHealth;
    protected override void Awake(){ //Không gọi khi chưa vào màn chơi
        base.Awake();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        behaviours = new List<IBehaviour>(GetComponents<IBehaviour>());
        SetEntityState(EntityState.Idle);
        // if(!isInited) { Initialize(); isInited = true; }
    }
    protected virtual void Update(){
        IGrid grid = GridSystem.Instance;
        Vector2Int gridPos = grid.WorldToGridPosition(GetWorldPosition());
        if (!grid.IsValidGridPosition(gridPos.x, gridPos.y)){
            Die();
        }
    }
    public Vector2 GetWorldPosition(){
        if (entityState == EntityState.Attacking){
            return realPosition;
        }
        else{
            return transform.position;
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (IsDead()) return;
        health -= damage;
        OnHealthChanged?.Invoke(-damage);
        if (health <= 0)
            Die();
    }

    public void Heal(int healAmount)
    {
        if (IsDead()) return;
        health = Mathf.Min(health + healAmount, maxHealth);
        OnHealthChanged?.Invoke(healAmount);
    }

    public void Die()
    {
        OnEntityDeath?.Invoke(this);
        SetEntityState(EntityState.Death);
    }
    public virtual void SelfDestroy(){
        if (!Application.isPlaying)
        {
            GameObject.DestroyImmediate(this.gameObject);
        }
        else
        {
            EContainer.entities[laneIndex].Remove(this);
            GameObject.Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// Tranfer data from SO to this Entity
    /// </summary>
    // [ContextMenu("Add data from SO to Entity")]
    public void Initialize(){
        isInited = true;
        team = so.team; 
        entityType = so.entityType;
        maxHealth = so.health;
        health = so.health;
        canAttack = so.canAttack;
        canMove = so.canMove;
        width = so.width; height = so.height;

        Vector2Int gridPos = IGrid.Instance.WorldToGridPosition(this.transform.position);
        
        foreach(IBehaviour behav in behaviours){
            behav.Initialize();
        }
        if (Application.isPlaying){
            laneIndex = gridPos.y; sortingGroup.sortingOrder = height - laneIndex;
            IGrid grid = GridSystem.Instance;
            if (width != 1 || height != 1){
                //Adjust transform.position so the tower appear behind 
                transform.position = grid.GridToWorldPosition(gridPos) + (new Vector2(width/2-grid.cellSize/2, height/2-grid.cellSize/2));
                //Add this entity to every lanes it occupy
                for(int i = laneIndex; i < laneIndex + width; ++i){
                    EContainer.AddEntity(this, laneIndex);
                }
            }
            EContainer.AddEntity(this, laneIndex);
            this.transform.SetParent(EntityContainer.Instance.transform);
        }
    }
    public float Distance(Entity other) {
        return Mathf.Abs(this.GetWorldPosition().x - other.GetWorldPosition().x);
    }
    public void SetEntityState(EntityState state){
        this.state = state;
        OnStateChanged?.Invoke(state);
        if (state == EntityState.Idle){
            animator.Play("Idle");
            foreach(IBehaviour behav in behaviours){
                behav.enabled = false;
            }
            this.enabled = false;
        }
        else if (state == EntityState.Walk){
            animator.Play("Walk");
        }
        else if (state == EntityState.Attacking){
            animator.Play("Attack", 0, 0f);
            realPosition = transform.position;
        }
        else if (state == EntityState.Death){
            foreach(IBehaviour behav in behaviours){
                behav.enabled = false;
            }
            this.enabled = false;
        }
        else if(state == EntityState.Frozen){
            foreach(IBehaviour behav in behaviours){
                behav.enabled = false;
            }
            this.enabled = false;
            animator.enabled = false;
        }
    }

}
