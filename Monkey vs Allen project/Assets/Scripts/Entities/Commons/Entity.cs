using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


public class Entity : IEntity {
    private IBehaviour activeBehavior;
    [ReadOnly] public string activeBehaviourName;
    [Header("References")]
    private IBehaviour[] behaviours;
    private IInitialize[] initializes;
    [SerializeField] private EffectController effectController;
    private IBehaviour idleBehaviour;
    private bool isPaused = true;
    public override bool IsDead() => isDead;
    public override Animator GetAnimator() => model.animator;
    public override AnimatorEvent GetAnimatorEvent() => model.Event;
    public override EntitySO GetSO() => SORegistry.Get<EntitySO>(soId);
    public override float GetHealthPercentage() => Stats[ST.Health] / Stats[ST.MaxHealth];
    public override float GetRealMoveSpeed() => Stats[ST.MoveSpeed] * IGrid.CELL_SIZE / 3;
    protected virtual void Awake() { //Không gọi khi chưa vào màn chơi
        tribes = new();
        model.entity = this;
        initializes = GetComponents<IInitialize>();
        behaviours = GetComponents<IBehaviour>().OrderBy(b => -b.GetPriority()).ToArray();
        idleBehaviour = GetComponent<Idle>();
        OnEntityDeath = null;
        OnHealthChanged = null;
    }
    protected virtual void Update() {
        if (BattleInfo.gameState != GameState.Fighting){ return; }
        if((team == Team.Player && gridPos.x >= IGrid.Ins.width) || (team == Team.Enemy && gridPos.x <= -1)) {
            Die();
        }
        if(isPaused) { return; }
        if(activeBehavior is IInterruptable) {
            foreach(IBehaviour behav in behaviours) {
                if(activeBehavior == behav) { break; }
                if(behav.enabled && behav.CanActive()) {
                    activeBehavior = behav;
                    activeBehaviourName = behav.GetType().ToString();
                    if(activeBehavior is IOnApply onApply) {
                        onApply.OnApply();
                    }
                    break;
                }
            }
        }
        activeBehavior.UpdateBehaviour();
    }
    public override IEffectable GetEffectable() => effectController;
    public override Team team {
        get { return _team; }
        set {
            _team = value;
            if(value == Team.Enemy) model.GetComponent<Rotater>().FlipX();
        }
    }
    public override float this[ST st] {
        get {
            if(Stats.ContainsKey(st)) {
                float adjustedValue = Mathf.Max(0, effectController.GetFinalStat(st));
                if(st is ST.Armor or ST.MagicResistance or ST.Penetration or ST.MagicPenetration) {
                    adjustedValue = Mathf.Min(adjustedValue, 100f);
                }
                else if (st is ST.AttackSpeed) {
                    adjustedValue = Mathf.Max(adjustedValue, 0.25f);
                }
                return adjustedValue;
            }
            else {
                return 0;
            }
        }
        set {
            Stats[st] = value;
        }
    }
    public override void TakeDamage(DamageContext ctx) {
        if(IsDead()) return;
        ctx.attacker.GetComponent<EffectController>().ProcessDamageOutput(ctx);
        effectController.ProcessDamageInput(ctx);
        if(ctx.amount <= 0) { return; }
        AnimatorStateInfo stateInfo = GetAnimator().GetCurrentAnimatorStateInfo(0);
        float weight = 0;
        if(stateInfo.IsName("Attack")) {
            weight = 0.5f;
        }
        else if(stateInfo.IsName("Idle") || stateInfo.IsName("Walk")) {
            weight = 1f;
        }
        model.animator.SetLayerWeight(1, weight);
        model.animator.Play("Hurt Layer.Hurt");
        Stats[ST.Health] -= ctx.amount;
        effectController.ProcessDamageTaken(ctx);
        OnHealthChanged?.Invoke(-ctx.amount);
        if(Stats[ST.Health] <= 0) {
            Stats[ST.Health] = 0;
            Die();
        }
    }
    public override void Heal(float healAmount) {
        if(IsDead()) return;
        Stats[ST.Health] = Mathf.Min(Stats[ST.Health] + healAmount, Stats[ST.MaxHealth]);
        OnHealthChanged?.Invoke(healAmount);
    }

    public override void Die() {
        if(IsDead()) { return; }
        isDead = true;
        OnEntityDeath?.Invoke();
    }

    /// <summary>
    /// Tranfer data from SO to this Entity
    /// </summary>
    public virtual void Initialize(EntitySO so, Team team, float x, int laneIndex, int level) {
        this.soId = so.id;
        this.level = level;
        this.team = team;
        tribes = so.tribes;
        this.lane = laneIndex;

        foreach(var stat in so.GetEntityStats(level)) {
            Stats.Add(stat.Key, stat.Value);
        }
        Stats.Add(ST.Health, this[ST.MaxHealth]);
        
        foreach(IBehaviour bev in behaviours) {
            bev.Initialize();
        }
        model.sortingGroup.sortingOrder = 1 - laneIndex;
        if(team == Team.Enemy) {
            model.transform.FlipLocalScaleX();
        }
        transform.Translate(new Vector2(0, yAxisAdjustment + UnityEngine.Random.Range(-0.2f, 0.2f)));
        if(width != 1 || height != 1) {
            //Adjust transform.position so the tower appear behind 
            // transform.position += (new Vector2(width / 2 - grid.cellSize / 2, height / 2 - grid.cellSize / 2));
        }
        isPaused = false;
        ChangeBehaviour(idleBehaviour);
        foreach(IInitialize init in initializes) {
            init.Initialize();
        }
    }
    public override float DistanceTo(IEntity other) {
        return Mathf.Abs(this.gridPos.x - other.gridPos.x);
    }
    public override float DistanceToBase() {
        if(team == Team.Player) {
            return gridPos.x + 1;
        }
        else {
            return IGrid.Ins.height - gridPos.x;
        }
    }
    public override void TogglePause(bool toggle) {
        isPaused = toggle;
    }
    public override void BecomeInActive() {
        model.PlayAnimation("Idle");
        foreach(IBehaviour behav in behaviours) {
            behav.enabled = false;
        }
    }
    public override float GetDangerPoint() {
        float total = Stats[ST.Health] / 6;
        foreach(IBehaviour behav in behaviours) {
            total += behav.GetDangerPoint();
        }
        total += effectController.GetDangerPoint();
        return total;
    }
    public override float GetSkillStat(SkillSO skillSO, string name) {

        return skillSO.GetStat(name, 0);
    }
    public override void ReturnToIdleBehaviour() {
        ChangeBehaviour(idleBehaviour);
    }
    public IBehaviour GetActiveBehaviour() {
        return activeBehavior;
    }
    private void ChangeBehaviour(IBehaviour behav) {
        if(activeBehavior is IOnDestroy onDestroy) {
            onDestroy.OnDestroy();
        }
        activeBehavior = behav;
        activeBehaviourName = behav.GetType().ToString();
        if(activeBehavior is IOnApply onApply) {
            onApply.OnApply();
        }
    }
}
