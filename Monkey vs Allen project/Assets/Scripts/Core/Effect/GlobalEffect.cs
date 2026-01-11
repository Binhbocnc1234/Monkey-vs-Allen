using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class GlobalEffect : IUpdatePerFrame, IDestroyable {
    public bool isDebuff { get; protected set; }
    protected readonly int duration;
    public int strength{ get; protected set; }
    protected Timer lifeTimer;
    private bool isDead;
    public GlobalEffect(int duration = -1, int strength = 1) {
        this.duration = duration;
        this.strength = strength;
        isDebuff = true;
        if(duration != -1) {
            lifeTimer = new Timer(duration, false);
        }
    }
    public virtual void Update() {
        if(lifeTimer.Count()) {
            DestroyThis();
        }
    }
    public void DestroyThis() {
        isDead = true;
    }
    public bool IsDead() => isDead;
    public virtual bool IsIdentical(IEffect effect) {
        return this.GetType() == effect.GetType();
    }
    public void ResetDuration() {
        if(duration != -1) {
            lifeTimer.Reset();
        }
    }
}
public class GlobalEffectManager : UpdateManager<GlobalEffect>{
    public ReadOnlyCollection<GlobalEffect> GlobalEffects => container.AsReadOnly();
    public void AddEffect(GlobalEffect addedGle) {
        foreach(GlobalEffect gle in container) {
            if(gle.GetType() == addedGle.GetType()) {
                gle.ResetDuration();
                if(gle is IStackable stack) {
                    stack.Stack(addedGle.strength);
                }
                return;
            }
        }
        AddElement(addedGle);
    }
    public void RemoveEffect(GlobalEffect removedGle) {
        RemoveElement(removedGle);
    }
}