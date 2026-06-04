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
    public override void Initialize(EntityModel model) {
        base.Initialize(model);
        associatedEntity = e;
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