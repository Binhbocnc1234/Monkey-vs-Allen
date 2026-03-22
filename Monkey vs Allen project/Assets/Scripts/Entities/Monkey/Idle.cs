using UnityEngine;


public class Idle : IBehaviour, IOnApply, IInterruptable {

    public override void Initialize() {
        base.Initialize();
    }
    public override bool CanActive() {
        return true;
    }
    public override void UpdateBehaviour() {
        
    }
    public void OnApply() {
        e.model.PlayAnimation("Idle");
    }
    public override int GetPriority() {
        return 0;
    }
}