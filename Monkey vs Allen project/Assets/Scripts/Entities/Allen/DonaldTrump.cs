using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DonaldTrump : Allen
{
    public float gunCooldown, embargoCooldown, summonCooldown;
    public Dollar dollarPrefab;
    public override void Initialize(EntitySO so, Team team) {
        base.Initialize(so, team);
        
    }
    protected override void Update(){
        base.Update();
    }

}

public class PowerOfDollar : Skill{
    public PowerOfDollar(IEntity owner, float cooldown) : base(owner, cooldown) {

    }
    public override void Update() {
        base.Update();
    }
    protected override void ActiveSkill() {
        owner.animator.Play("SupremeGun");
        for(int i = 0; i < 10; ++i){
            var thisPos = IGrid.Ins.WorldToGridPosition(owner.GetWorldPosition());
            Vector2Int pos = new Vector2Int(Random.Range(thisPos.x-3, thisPos.x), Random.Range(thisPos.y-3, thisPos.y));
            Object.Instantiate(SingletonRegister.Get<PrefabRegisterSO>().dollarGameObject, owner.transform).GetComponent<Dollar>().Initialize(pos);
        }
    }
    protected override bool CanActiveSkill() {
        return true;
    }
}