using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


public class Entity : IEntity {
    private IBehaviour activeBehavior;
    private EntitySO previousAssignedSO;
    [ReadOnly] public string activeBehaviourName;
    [Header("References")]
    private IBehaviour[] behaviours;
    private IInitialize[] initializes;
    private IAssessable[] assessables;
    [SerializeField] private EffectController effectController;
    [SerializeField, Min(0f)] private float statLerpSpeed = 10f;
    private IBehaviour idleBehaviour;
    private readonly Dictionary<ST, float> statTargets = new();
    private readonly List<ST> completedStatTargets = new();
    private bool isPaused = true;
    public override bool IsDead() => isDead;
    public override Animator GetAnimator() => model.animator;
    public override AnimatorEvent GetAnimatorEvent() => model.Event;
    public override EntitySO GetSO() => SORegistry.Get<EntitySO>(soId);
    public override float GetHealthPercentage() => Stats[ST.Health] / Stats[ST.MaxHealth];
    public override float GetRealMoveSpeed() => Stats[ST.MoveSpeed] * IGrid.CELL_SIZE / 4;
    protected virtual void Awake() { //Không gọi khi chưa vào màn chơi
        tribes = new();
        model.entity = this;
        initializes = GetComponents<IInitialize>();
        assessables = GetComponents<IAssessable>();
        behaviours = GetComponents<IBehaviour>().OrderBy(b => -b.GetPriority()).ToArray();
        idleBehaviour = GetComponent<Idle>();
    }
    protected virtual void Update() {
        if (BattleInfo.gameState != GameState.Fighting){ return; }
        if((team == Team.Left && gridPos.x >= IGrid.Ins.width) || (team == Team.Right && gridPos.x <= -1)) {
            Die();
        }
        UpdateStatTargets();
        if(isPaused) { return; }
        if(activeBehavior is IInterruptable || activeBehavior.CanActive() == false) {
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
            if(value == Team.Right) model.GetComponent<Rotater>().FlipX();
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
            // if(!Stats.ContainsKey(st)) {
            //     Stats[st] = value;
            //     statTargets.Remove(st);
            //     return;
            // }

            // if(Mathf.Approximately(Stats[st], value)) {
            //     Stats[st] = value;
            //     statTargets.Remove(st);
            //     return;
            // }

            Stats[st] = value;
        }
    }
    private void UpdateStatTargets() {
        return;
        // if(statTargets.Count == 0) {
        //     return;
        // }

        // completedStatTargets.Clear();

        // foreach(var entry in statTargets) {
        //     ST stat = entry.Key;
        //     float target = entry.Value;

        //     if(!Stats.ContainsKey(stat)) {
        //         completedStatTargets.Add(stat);
        //         continue;
        //     }

        //     float current = Stats[stat];
        //     float next = Mathf.Lerp(current, target, statLerpSpeed * Time.deltaTime);

        //     if(Mathf.Abs(target - next) <= 0.01f) {
        //         Stats[stat] = target;
        //         completedStatTargets.Add(stat);
        //     }
        //     else {
        //         Stats[stat] = next;
        //     }
        // }

        // foreach(ST stat in completedStatTargets) {
        //     statTargets.Remove(stat);
        // }
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
        Stats[ST.Health] = Mathf.Max(Stats[ST.Health] - ctx.amount, 0);
        effectController.ProcessDamageTaken(ctx);
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

    /// <summary>
    /// Tranfer data from SO to this Entity
    /// </summary>
    public virtual void Initialize(EntitySO so, Team team, float x, int laneIndex, int level) {
        this.level = level;
        this.lane = laneIndex;
        this.team = team;
        UpdateSO(so);
        model.sortingGroup.sortingOrder = 1 - laneIndex;
        if(team == Team.Right) {
            model.transform.FlipLocalScaleX();
        }
        if(width != 1 || height != 1) {
            //Adjust transform.position so the tower appear behind 
            // transform.position += (new Vector2(width / 2 - grid.cellSize / 2, height / 2 - grid.cellSize / 2));
        }
        if(CustomSceneManager.isFreePlay) {
            FillBarManager.Ins.CreateHealthBar(this);
        }
        isPaused = false;
        ChangeBehaviour(idleBehaviour);
        transform.Translate(new Vector2(0, yAxisAdjustment + UnityEngine.Random.Range(-0.2f, 0.2f)));
    }
    internal void UpdateSO(EntitySO so) {
        this.so = so;
        this.soId = so.id;
        tribes = so.tribes;
        Stats = new();
        effectController.Reset();
        //gg
        foreach(var stat in so.GetEntityStats(level)) {
            Stats.Add(stat.Key, stat.Value);
        }
        Stats.Add(ST.Health, Stats[ST.MaxHealth]);
        foreach(IInitialize init in GetComponents<IInitialize>()) {
            init.Initialize();
        }
        effectController.Flush();
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
        model.PlayAnimation("Idle");
        foreach(IBehaviour behav in behaviours) {
            behav.enabled = false;
        }
    }
    [ContextMenu("GetAssessPoint")]
    public void OutputAssessPoint() {
        Debug.Log($"DangerPoint for {gameObject.name} is {GetAssessPoint(APType.Danger)}");
        Debug.Log($"DefendPoint for {gameObject.name} is {GetAssessPoint(APType.Defend)}");
    }
    struct AssessmentData {
        internal float timeStamp;
        internal float value;
    }
    private UDictionary<APType, AssessmentData> assessmentCache = new();
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
            return GetAssessPoint(APType.Danger);
        }
        foreach(IAssessable behav in GetComponents<IAssessable>()) {
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
