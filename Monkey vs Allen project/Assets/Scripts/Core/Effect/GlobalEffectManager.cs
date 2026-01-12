
using System.Collections.ObjectModel;

public class GlobalEffectManager : UpdateManager<GlobalEffect> {
    public ReadOnlyCollection<GlobalEffect> GlobalEffects => container.AsReadOnly();
    void Awake() {
        SingletonRegister.Register(this);
    }
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