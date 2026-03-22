using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

// Kế hoạch: toàn bộ hàm ưới đây sẽ di chuyển lại vào Entity
public class EffectController : UpdateManager<Effect>, IEffectable {
    Entity e;
    Dictionary<IEntity, float> damageContributors = new();
    void Awake() {
        e = GetComponent<Entity>();
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
            else if (effect is IOnOtherEffectApply effWithInterface) {
                effWithInterface.OnApply(addedEffect);
            }
        }
        if(addedEffect.IsDead()) return;
        if (addedEffect is IOnApply onApplyEffect) {
            onApplyEffect.OnApply();
        }
        base.AddElement(addedEffect);
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
    }
    public void ProcessDamageInput(DamageContext ctx) {
        foreach(Effect effect in container) {
            if(effect is IDamageInputModifier modifier) {
                modifier.ModifyDamage(ctx);
            }
        }
        
        if(!ctx.isMagicalDamage) {
            ctx.amount -= ctx.amount * Math.Max(0f, e[ST.Armor] - ctx.penetrationAmount) / 100f;
        }
        else {
            ctx.amount -= ctx.amount * Math.Max(0f, e[ST.MagicResistance] - ctx.penetrationAmount) / 100f;
        }
    }
    public void ProcessDamageTaken(DamageContext ctx) {
        damageContributors[ctx.attacker] = damageContributors.GetValueOrDefault(ctx.attacker) + ctx.amount;
        foreach(Effect effect in container) {
            if(effect is IOnDamageTaken odt) {
                odt.OnDamageTaken(ctx);
            }
            else if(effect is IOnDefenderDamageTaken oddt) {
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
        float valueAfterAddition = e.Stats[stat];
        float multiplier = 1;
        foreach(Effect effect in container) {
            if(effect is IModifyStat modifyStatEff) {
                StatModifier modifier = modifyStatEff.ModifyStat().FirstOrDefault(e => e.st == stat);
                if(modifier != null) {
                    if(modifier.op == Operator.Addition) {
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
}