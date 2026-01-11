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
        attackSpeed = so.attackSpeed*1.35f;
        dangerPoint = damage + attackRange;
        attackTimer = new Timer(1 / attackSpeed * 1.35f, false);
        e.animatorEvent.OnMakeDamage += MakeDamageInstantly;
    }
    protected override void UpdateBehaviour(){
        if (attackTimer.Count()){
            if (IsTherePreyNearby()){
                attackTimer.Reset();
                e.SetEntityState(EntityState.Attacking);
            }
            else{
                e.ReturnToDefaultState();
            }
        }
    }
    protected virtual void MakeDamageInstantly(){
        Entity defender = GetNearestPrey(); 
        if (defender == null){ return; }
        defender.TakeDamage(new DamageContext(damage, e, defender));
    }
    protected bool IsTherePreyNearby(){ //Problem This function is duplicated in Monkey, Tower, 
        var entitiesInLane = EContainer.Ins.GetEntitiesByLane(e.laneIndex);
        foreach(Entity entity in entitiesInLane){
            if (IsEnemyInRange(entity, attackRange + 0.4f)){
                return true;
            }
        }
        return false;
    }
    protected List<Entity> GetPreyNearby() {
        var entitiesInLane = EContainer.Ins.GetEntitiesByLane(e.laneIndex);
        List<Entity> result = new List<Entity>();
        foreach(Entity entity in entitiesInLane) {
            if(IsEnemyInRange(entity, attackRange + 0.6f)) {
                result.Add(entity);
            }
        }
        // if(result.Count == 0) { Debug.LogWarning("No prey nearby"); }
        return result;
    }
    protected Entity GetNearestPrey() {
        var preys = GetPreyNearby();
        if(preys.Count == 0) {
            return null;
        }
        Entity ans = preys[0];
        foreach(Entity prey in preys) {
            if((prey.GetWorldPosition().x - e.GetWorldPosition().x) <= attackRange) {
                ans = prey;
            }
        }
        return ans;
    }
    protected bool IsEnemyInRange(Entity entity, float attackRange){
        float entityX = entity.GetWorldPosition().x;
        if (entity.team != e.team && Mathf.Abs(entityX - e.GetWorldPosition().x) <= attackRange){
            return true;
        }
        return false;
    }
    
}
