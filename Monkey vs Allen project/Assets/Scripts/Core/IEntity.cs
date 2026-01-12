using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class IEntity : MonoBehaviour
{
    public List<Tribe> tribes { get; protected set; }
    public Team team = Team.Default;
    public Animator animator;
    public abstract IEffectable GetEffectController();
    public abstract void Initialize(EntitySO so, Team team);
    public abstract bool IsDead();
    public abstract void Die();
    public abstract void TakeDamage(DamageContext damageContext);
    public abstract void SetEntityState(EntityState state);
    public abstract Vector2 GetWorldPosition();
    public abstract EntityState GetEntityState();
}
