using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


public sealed class Entity : IEntity {
    private IBehaviour activeBehavior;
    public string activeBehaviourName;
    private EffectController effectController;
    private float statLerpSpeed = 10f;
    private IBehaviour idleBehaviour;
    private readonly Dictionary<ST, float> statTargets = new();
    private readonly List<ST> completedStatTargets = new();
    private bool isPaused = true;
    public override bool IsDead() => isDead;
    public override EntitySO GetSO() => SORegistry.Get<EntitySO>(soId);
    public override float GetHealthPercentage() => Stats[ST.Health] / Stats[ST.MaxHealth];
    public override float GetRealMoveSpeed() => this[ST.MoveSpeed] / 4;

    // Public value-like data is copied via JSON; reference fields stay shallow via MemberwiseClone.
    public static List<IBehaviour> CloneBehaviourTemplates(List<IBehaviour> templates) {
        if(templates == null || templates.Count == 0) {
            return new List<IBehaviour>();
        }
        List<IBehaviour> clones = new(templates.Count);
        foreach(IBehaviour template in templates) {
            if(template == null) {
                continue;
            }
            clones.Add(template.GetClone());
        }
        return clones;
    }

    public Entity(EntitySO so, Team team, float x, int laneIndex, int level, bool isSimulated = false) {
        this.effectController = new EffectController(this);
        this.level = level;
        this.lane = laneIndex;
        this.team = team;
        this.isSimulated = isSimulated;
        tribes = so.tribes;
        var clonedBehaviours = CloneBehaviourTemplates(so.behaviourTemplates);
        clonedBehaviours.Add(new Idle());
        clonedBehaviours.Add(new InactiveBehaviour());
        SetBehaviours(clonedBehaviours);
        gridPos = new Vector2(x, laneIndex);
        UpdateSO(so);
        isPaused = false;
    }
    public override IEffectable GetEffectable() => effectController;
    public override Team team {
        get { return _team; }
        set {
            if (value != _team){
                OnTeamSwapped?.Invoke(value);
            }
            _team = value;
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
        ctx.attacker.GetEffectable()?.ProcessDamageOutput(ctx);
        effectController?.ProcessDamageInput(ctx);
        if(ctx.amount <= 0) { return; }
        Stats[ST.Health] = Mathf.Max(Stats[ST.Health] - ctx.amount, 0);
        effectController?.ProcessDamageTaken(ctx);
        OnHealthChanged?.Invoke(-ctx.amount);
        if(Stats[ST.Health] == 0) {
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

    public void UpdateBehaviours(float deltaTime) {
        if (BattleInfo.gameState != GameState.Fighting){ return; }
        if(IGrid.Ins != null && ((team == Team.Left && gridPos.x >= IGrid.Ins.width) || (team == Team.Right && gridPos.x <= -1))) {
            Die();
        }
        effectController.Update(deltaTime);
        if(isPaused) { return; }
        if(activeBehavior is IInterruptable || activeBehavior.CanActive() == false) {
            foreach(IBehaviour behav in behaviours) {
                if(activeBehavior == behav || behav.GetPriority() == -1) { break; }
                if(behav.isEnable && behav.CanActive()) {
                    ChangeBehaviour(behav);
                    break;
                }
            }
        }
        foreach(IBehaviour behav in behaviours){
            if (behav is IUpdatePerFrame updatePerFrame){
                updatePerFrame.Update(deltaTime);
            }
        }
        activeBehavior.UpdateBehaviour(deltaTime);
    }
    internal void UpdateSO(EntitySO so) {
        this.so = so;
        this.soId = so.id;
        tribes = so.tribes;
        Stats = new();
        effectController?.Reset();
        foreach(var stat in so.GetEntityStats(level)) {
            Stats.Add(stat.Key, stat.Value);
        }
        Stats.Add(ST.Health, Stats[ST.MaxHealth]);
        foreach(IInitialize init in GetBehaviours<IInitialize>()) {
            init.Initialize();
        }
        effectController?.Flush();
    }
    public override float DistanceTo(IEntity other) {
        return Mathf.Abs(this.gridPos.x - other.gridPos.x);
    }
    public override float DistanceToBase() {
        return DistanceTo(this.team);
    }
    public override float DistanceToOpponentBase() {
        return DistanceTo(EnumConverter.GetOppositeSide(this.team));
    }
    private float DistanceTo(Team baseTeam = Team.Left) {
        if(baseTeam == Team.Left) {
            return gridPos.x + 1;
        }
        else {
            return IGrid.Ins.width - gridPos.x;
        }
    }
    public override void TogglePause(bool toggle) {
        isPaused = toggle;
    }
    public override void BecomeInActive() {
        ChangeBehaviour(GetBehaviour<InactiveBehaviour>());
    }
    [ContextMenu("GetAssessPoint")]
    public void OutputAssessPoint() {
        Debug.Log($"DangerPoint for {GetSO().name} is {GetAssessPoint(APType.Danger)}");
        Debug.Log($"DefendPoint for {GetSO().name} is {GetAssessPoint(APType.Defend)}");
    }
    struct AssessmentData {
        internal float timeStamp;
        internal float value;
    }
    private Dictionary<APType, AssessmentData> assessmentCache = new();
    public override float GetAssessPoint(APType type) {
        if (assessmentCache.ContainsKey(type)) {
            AssessmentData data = assessmentCache[type];
            if(BattleInfo.timeElapsed - data.timeStamp <= 0.5f) {
                return data.value;
            }
        }
        List<APModifier> modifiers = new();
        if(type == APType.Defend) {
            modifiers.Add(new APModifier(Operator.Addition, APType.Defend,
            this[ST.Health] / (1 - this[ST.Armor] / 100f) / 7 + (GetAssessPoint(APType.Danger) * this[ST.LifeSteal] / 100)));
        }
        else if (type == APType.NeedProtection) {
            float baseNP = GetAssessPoint(APType.Danger);
            if (GetSO() != null && GetSO().tribes.Contains(Tribe.Target)) {
                baseNP += Target.NeedProtectionBonus;
            }
            return baseNP;
        }
        foreach(IAssessable behav in GetBehaviours<IAssessable>()) {
            var point = behav.GetAssessPoint().FirstOrDefault(a => a.type == type);
            if(point != null) { modifiers.Add(point); }
        }
        modifiers.Sort((a, b) => a.op.CompareTo(b.op));
        // foreach(GlobalEffect gbEffect in GlobalEffectManager.Ins.GlobalEffects) {
        //     var point = gbEffect.GetAssessPoint().FirstOrDefault(a => a.type == type);
        //     modifiers.Add(point);
        // }
        float finalValue = 0;
        foreach(var mod in modifiers) {
            if(mod.op == Operator.Addition) {
                finalValue += mod.value;
            }
            else if(mod.op == Operator.Multiply) {
                finalValue *= mod.value;
            }
            else {
                Debug.LogError("[Entity] unhandled Operator");
            }
        }
        assessmentCache[type] = new AssessmentData{timeStamp = BattleInfo.timeElapsed, value = finalValue};
        return finalValue;
    }
    public override float GetSkillStat(SkillSO skillSO, string name) {
        return skillSO.GetStat(name, 0);
    }
    public override void ReturnToIdleBehaviour() {
        ChangeBehaviour(idleBehaviour);
    }
    public override IBehaviour GetActiveBehaviour() {
        return activeBehavior;
    }
    public override T GetBehaviour<T>() where T : class {
        foreach(IBehaviour behaviour in behaviours) {
            if(behaviour is T typed) {
                return typed;
            }
        }
        return null;
    }
    public override IEnumerable<T> GetBehaviours<T>() where T : class {
        foreach(IBehaviour behaviour in behaviours) {
            if(behaviour is T typed) {
                yield return typed;
            }
        }
    }
    public override void SetBehaviours(IEnumerable<IBehaviour> behaviourList) {
        behaviours = behaviourList.OrderBy(b => -b.GetPriority()).ToArray();
        foreach(IBehaviour behaviour in behaviours) {
            behaviour.SetEntity(this);
        }
        idleBehaviour = GetBehaviour<Idle>();
        ChangeBehaviour(idleBehaviour);
    }
    private void ChangeBehaviour(IBehaviour behav) {
        if(activeBehavior is IOnDestroy onDestroy) {
            onDestroy.OnDestroy();
        }
        activeBehavior = behav;
        activeBehaviourName = behav != null ? behav.GetType().ToString() : "None";
        if(activeBehavior is IOnApply onApply) {
            onApply.OnApply();
        }
        OnBehaviorActive?.Invoke(activeBehavior);
    }
}
