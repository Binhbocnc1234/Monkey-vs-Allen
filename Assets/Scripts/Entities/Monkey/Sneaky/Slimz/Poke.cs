using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poke : CollisionBullet
{
    [SerializeField] private Animator animator;
    private IEntity nearestTarget;
    private float stunDuration;
    private bool isDebuffApplied;
    public void Initialize(IEntity e, float damage, float stunDuration, bool isDebuffApplied) {
        base.Initialize(damage, e);
        this.isDebuffApplied = isDebuffApplied;
        this.stunDuration = stunDuration;
    }
    protected override void Update() {
        base.Update();
        if(nearestTarget != null && Mathf.Abs(nearestTarget.transform.position.x
            - this.transform.position.x) <= IGrid.CELL_SIZE) {
            animator.Play("Poke Open");
        }
    }
    protected override void OnHit(IEntity target) {
        base.OnHit(target);
        target.GetEffectable().ApplyEffect(new Stun(stunDuration));
        if(isDebuffApplied) {
            target.GetEffectable().ApplyEffect(new SlimzSting(stunDuration));
        }
    }
}
