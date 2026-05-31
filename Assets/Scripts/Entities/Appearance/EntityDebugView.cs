using UnityEngine;

public class EntityDebugView : EntityAppearance {
    public IEntity associatedEntity;
    public string entityName;
    public float health;
    public float maxHealth;
    public float healthPercentage;
    public Team team;
    public int lane;
    public float gridPosX;
    public float gridPosY;
    public string activeBehaviourName;
    public bool isDead;
    public void BindEntity(IEntity e) {
        associatedEntity = e;
        if(e.team == Team.Right) {
            transform.FlipLocalScaleX();
        }
        e.OnHealthChanged += (changedAmount) => {
            if(changedAmount < 0) {
                AnimatorStateInfo stateInfo = model.animator.GetCurrentAnimatorStateInfo(0);
                float weight = 0;
                if(stateInfo.IsName("Attack")) {
                    weight = 0.5f;
                }
                else if(stateInfo.IsName("Idle") || stateInfo.IsName("Walk")) {
                    weight = 1f;
                }
                model.animator.SetLayerWeight(1, weight);
                model.PlayAnimation("Hurt Layer.Hurt");
            }
        };
        e.OnBehaviorActive += (behav) => {
            model.PlayAnimation(behav.GetAnimatorStateName());
            if(behav.GetAnimatorStateName() == "Attack") {
                model.animator.SetFloat("AttackSpeed", e[ST.AttackSpeed] / e.GetSO().attackSpeed);
            }
        };
        Update();
    }
    void Update() {
        if(e == null) return;
        entityName = e.GetSO()?.name ?? "N/A";
        health = e[ST.Health];
        maxHealth = e[ST.MaxHealth];
        healthPercentage = e.GetHealthPercentage();
        team = e.team;
        lane = e.lane;
        gridPosX = e.gridPos.x;
        gridPosY = e.gridPos.y;
        activeBehaviourName = e.GetActiveBehaviour()?.GetType().ToString() ?? "None";
        isDead = e.IsDead();
    }
}