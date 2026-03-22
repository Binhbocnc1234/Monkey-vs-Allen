using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueOfLiberty : MonoBehaviour, IInitialize
{
    private DoubleDamageEffect globalEffect;
    public void Initialize() {
        globalEffect = new DoubleDamageEffect();
        GlobalEffectManager.Ins.AddEffect(globalEffect);
        GetComponent<IEntity>().OnEntityDeath += () => GlobalEffectManager.Ins.RemoveEffect(globalEffect);
    }
}
