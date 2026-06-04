using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

/*

    Why in general structure of Entity, there is seperate gameObject called Model?
    Effect like airbone can adjust Entity's appearance position without changing its logic position 
    Bạn hãy ghi comment C# vào biến #sym:offset , tiếng anh với ý nghĩa như sau: "Một số model có vị trí của các bộ phận cơ thể(children gameobject) hơi lệch, ví dụ như Model - Basic Monkey có 
*/
/// <summary>
/// Position of model is adjusted so that it align well with grid system. Do not change model's position at runtime <br/>
/// </summary>
[RequireComponent(typeof(EntityDebugView)), RequireComponent(typeof(FlashWhite))]
public class EntityModel : Model {
    public IEntity e { get; private set; }
    public Transform firePoint;
    protected override void Awake() {
        base.Awake();
        sortingGroup.sortingLayerName = "Entities";
    }
    public void AssignEntity(IEntity e) {
        this.e = e;
        this.sortingGroup.sortingOrder = 1 - e.lane;
        foreach(EntityAppearance appearance in GetComponents<EntityAppearance>()) {
            appearance.Initialize(this);
        }
        if(firePoint != null && e.GetBehaviour<RangedAttack>() != null) {
            e.GetBehaviour<RangedAttack>().firePoint = firePoint;
        }
        e.OnHealthChanged += (changedAmount) => {
            if(changedAmount < 0) {
                AnimatorStateInfo stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
                float weight = 0;
                if(stateInfo.IsName("Attack")) {
                    weight = 0.5f;
                }
                else if(stateInfo.IsName("Idle") || stateInfo.IsName("Walk")) {
                    weight = 1f;
                }
                this.animator.SetLayerWeight(1, weight);
                this.PlayAnimation("Hurt Layer.Hurt");
            }
        };
        e.OnBehaviorActive += (behav) => {
            this.PlayAnimation(behav.GetAnimatorStateName());
            if(behav.GetAnimatorStateName() == "Attack") {
                this.animator.SetFloat("AttackSpeed", e[ST.AttackSpeed] / e.GetSO().attackSpeed);
            }
        };
        this.PlayAnimation(e.GetActiveBehaviour().GetAnimatorStateName());
    }
    void Update() {
        if (animator.applyRootMotion == false) {
            transform.position = IGrid.Ins.GridToWorldPosition(e.gridPos);
        }
    }
}