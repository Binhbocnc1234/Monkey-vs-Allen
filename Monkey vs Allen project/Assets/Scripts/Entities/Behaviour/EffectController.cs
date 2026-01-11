using UnityEngine;
using UnityEngine.PlayerLoop;

// Kế hoạch: toàn bộ hàm ưới đây sẽ di chuyển lại vào Entity
public class EffectController : UpdateManager<IEffect>, IEffectable {
    public void ApplyEffect(IEffect addedEffect) {
        foreach(IEffect effect in container) {
            if(effect.IsIdentical(addedEffect)) {
                effect.ResetDuration();
                if(effect is IStackable stackable) {
                    stackable.Stack(addedEffect.strength);
                }
                return;
            }
        }
        base.AddElement(addedEffect);
    }
    public void RemoveEffect(IEffect element) {
        base.RemoveElement(element);
    }
    public void ProcessDamageOutput(DamageContext ctx) {
        foreach(IEffect effect in container) {
            if(effect is IDamageOutputModifier modifier) {
                modifier.ModifyDamage(ctx);
            }
        }
    }
    public void ProcessDamageInput(DamageContext ctx) {
        foreach(IEffect effect in container) {
            if(effect is IDamageInputModifier modifier) {
                modifier.ModifyDamage(ctx);
            }
        }
    }
    public void ProcessDamageTaken(DamageContext ctx) {
        foreach(IEffect effect in container) {
            if(effect is IOnDamageTaken modifier) {
                modifier.OnDamageTaken(ctx);
            }
        }
    }

}