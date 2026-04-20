using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Kế hoạch: toàn bộ hàm ưới đây sẽ di chuyển lại vào Entity
public class EffectController : UpdateManager<Effect>, IEffectable, IAssessable {
    private Entity _e;
    protected Entity e {
        get {
            if(_e == null) _e = GetComponent<Entity>();
            return _e;
        }
    }
    Dictionary<IEntity, float> damageContributors = new();
    void Awake() {
        e.OnEntityDeath += NotifyAttacker;
    }
    public void ApplyEffect(Effect addedEffect) {
        addedEffect.owner = e;
        foreach(Effect effect in container) {
            if(effect.IsIdentical(addedEffect)) {
                effect.ResetDuration();
                if(effect is IStackable stackable) {
                    stackable.Stack(addedEffect.strength);
                }
                return;
            }
            else if(effect is IOnOtherEffectApply effWithInterface) {
                effWithInterface.OnApply(addedEffect);
            }
        }
        if(addedEffect.IsDead()) return;
        if(addedEffect is IOnApply onApplyEffect) {
            onApplyEffect.OnApply();
        }
        base.AddElement(addedEffect);
        // container.Sort((a, b) => {

        // });
    }
    public void RemoveEffect(Effect element) {
        base.RemoveElement(element);
    }
    public bool HaveEffect(Type type) {
        foreach(Effect eff in container) {
            if(eff.GetType() == type) {
                return true;
            }
        }
        return false;
    }
    public void ProcessDamageOutput(DamageContext ctx) {
        foreach(Effect effect in container) {
            if(effect is IDamageOutputModifier modifier) {
                modifier.ModifyDamage(ctx);
            }
        }
        ctx.Flush();
    }
    public void ProcessDamageInput(DamageContext ctx) {
        foreach(Effect effect in container) {
            if(effect is IDamageInputModifier modifier) {
                modifier.ModifyDamage(ctx);
            }
        }

        if(!ctx.isMagicalDamage) {
            ctx.AddModifier(new DamageModifier(Operator.Multiply, 1 - (e[ST.Armor] - ctx.penetrationAmount) / 100f));
        }
        else {
            ctx.AddModifier(new DamageModifier(Operator.Multiply, 1 - (e[ST.MagicResistance] - ctx.penetrationAmount) / 100f));
        }
        ctx.Flush();
    }
    public void ProcessDamageTaken(DamageContext ctx) {
        damageContributors[ctx.attacker] = damageContributors.GetValueOrDefault(ctx.attacker) + ctx.amount;
        foreach(Effect effect in container) {
            if(effect is IAfterTakenDamage odt) {
                odt.OnDamageTaken(ctx);
            }
            else if(effect is IAfterDefenderTakenDamage oddt) {
                oddt.OnDefenderDamageTaken(ctx);
            }
        }
        ctx.attacker.Heal(e[ST.LifeSteal]/100f*ctx.amount);
    }
    public void NotifyOnAssistOrKill() {
        foreach(Effect effect in container) {
            if(effect is IOnAssistOrKill onAssist) {
                onAssist.NotifyAssistOrKill();
            }
        }
    }
    void NotifyAttacker() {
        float totalDamage = 0;
        foreach(var pair in damageContributors) {
            totalDamage += pair.Value;
        }
        foreach(var pair in damageContributors) {
            if(pair.Key == null) continue;
            if (pair.Value/totalDamage >= 0.7f) {
                pair.Key.GetComponent<EffectController>().NotifyOnAssistOrKill();
            }
        }
    }
    public int GetDangerPoint() {
        int totalDangerPoint = 0;
        foreach(Effect effect in container) {
            totalDangerPoint += effect.GetDangerPoint();
        }
        return totalDangerPoint;
    }
    public float GetFinalStat(ST stat) {
        if(e.Stats == null) {
            Debug.Log("[EffectController] Null Stats");
        }
        float valueAfterAddition = e.Stats[stat];
        float multiplier = 1;
        foreach(Effect effect in container) {
            if(effect is IModifyStat modifyStatEff) {
                StatModifier modifier = modifyStatEff.ModifyStat().FirstOrDefault(e => e.st == stat);
                if(modifier != null) {
                    if(modifier.op == Operator.Addition) {
                        if(stat == ST.AttackSpeed) {
                            Debug.LogError("[EffectController] AttackSpeed should not have additions, multipliers only");
                        }
                        valueAfterAddition += modifier.value;
                    }
                    else {
                        multiplier *= modifier.value;
                    }
                }
            }
        }
        return valueAfterAddition * multiplier;
    }
    public List<APModifier> GetAssessPoint() {
        List<APModifier> ans = new();
        foreach(Effect eff in container) {
            ans.AddRange(eff.GetAssessPoint());
        }
        return ans;
    }
}