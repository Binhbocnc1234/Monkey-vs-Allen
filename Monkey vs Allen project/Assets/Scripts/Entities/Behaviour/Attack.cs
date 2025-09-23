using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Attack : IBehaviour
{
    public int damage;
    public float attackSpeed;
    public int attackRange;
    protected Timer attackTimer;
    public override void Initialize(){
        base.Initialize();
        e = GetComponent<Entity>();
        var so = e.so;
        damage = so.damage;
        attackRange = so.attackRange;
        attackSpeed = so.attackSpeed;
        dangerPoint = damage + attackRange;
        
        attackTimer = new Timer(1 / attackSpeed * 1.35f);
        
        if (e.animatorEvent != null){
            e.animatorEvent.OnMakeDamage += MakeDamageInstantly;
        }
        attackTimer = new Timer(attackSpeed);
        attackTimer.Count(false);
    }
    protected virtual void Update(){
        
        if (attackTimer.Count(false)){
            if (IsTherePreyNearby()){
                Debug.Log("Attack");
                attackTimer.Reset();
                e.SetEntityState(EntityState.Attacking);
            }
            else{
                e.SetEntityState(EntityState.Walk);
            }
        }
    }
    protected virtual void MakeDamageInstantly(){
        Debug.Log("Make damage instantly");
        var preys = GetPreyNearby();
        foreach(var prey in preys){
            prey.TakeDamage(damage);
        }
    }
    protected bool IsTherePreyNearby(){ //Problem This function is duplicated in Monkey, Tower, 
        List<Entity> entitiesInLane = EContainer.GetEntitiesByLane(e.laneIndex);
        foreach(Entity entity in entitiesInLane){
            if (IsEnemyInRange(entity, attackRange + 0.4f)){
                return true;
            }
        }
        return false;
    }
    protected List<Entity> GetPreyNearby(){
        List<Entity> entitiesInLane = EContainer.GetEntitiesByLane(e.laneIndex);
        List<Entity> result = new List<Entity>();
        foreach(Entity entity in entitiesInLane){
            if (IsEnemyInRange(entity, attackRange + 0.2f)){
                result.Add(entity);
            }
        }
        if(result.Count == 0) { Debug.LogWarning("No prey nearby"); }
        return result;
    }
    protected bool IsEnemyInRange(Entity entity, float attackRange){
        float entityX = entity.GetWorldPosition().x;
        if (entity.team != e.team && Mathf.Abs(entityX - e.GetWorldPosition().x) <= attackRange){
            return true;
        }
        return false;
    }
    
}
