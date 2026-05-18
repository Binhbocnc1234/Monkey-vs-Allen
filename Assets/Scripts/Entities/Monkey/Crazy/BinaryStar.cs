using UnityEngine;


public class BinaryStar : MonoBehaviour, IInitialize {
    public void Initialize() {
        IEntity e = GetComponent<IEntity>();
        e.GetEffectable().ApplyEffect(new DoubleDamageEffect());
    }
}