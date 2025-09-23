// using UnityEngine;
// using System;
// //Entity is a gameobject that has health and can TakeDamage from other Entities
// public abstract class IEntity : MonoBehaviour, IInitialize{
//     [Header("Statistics")]
//     public Team team;
//     public string entityName;
//     public EntityType entityType;
//     public int laneIndex;
//     public int maxHealth;
//     public int health;
//     public int width = 1, height = 1;
//     public bool canAttack, canMove;
    
//     [ReadOnly] public EntityState state;
//     public abstract void Initialize();
//     public abstract bool IsDead();
//     public abstract void SetEntityState(EntityState state);
//     public abstract Vector2 GetWorldPosition();
//     public abstract float Distance(IEntity other);
//     public abstract void Heal(int amount);
//     public abstract void TakeDamage(int damage);
//     public abstract void Die();
//     public Action<IEntity> OnEntityDeath;
//     public Action<int> OnHealthChanged;
//     public Action<EntityState> OnStateChanged;
// }
