using UnityEngine;


public class BinaryStar : MonoBehaviour, IInitialize {
    private DoubleDamageEffect globalEffect;
    public void Initialize() {
        IEntity e = GetComponent<IEntity>();
        globalEffect = new DoubleDamageEffect();
        GlobalEffectManager.Ins.AddEffect(globalEffect);
        e.OnEntityDeath += () => GlobalEffectManager.Ins.RemoveEffect(globalEffect);
    }
}