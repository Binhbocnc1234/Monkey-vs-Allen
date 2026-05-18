public class StunImmunity : Effect, IOnOtherEffectApply {
    public void OnApply(Effect otherEff) {
        if(otherEff is Stun or Slow or Hypnotized or Frozen) {
            otherEff.DestroyThis();
        }
    }
}

public class StunImmunityBySkill : Effect, IOnOtherEffectApply {
    public StunImmunityBySkill() : base(-1, 1) {
        
    }
    public void OnApply(Effect otherEff) {
        if(otherEff is Stun or Slow or Hypnotized or Frozen) {
            otherEff.DestroyThis();
        }
    }
}