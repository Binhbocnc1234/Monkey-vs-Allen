using UnityEngine;


public class Idle : IBehaviour, IInterruptable {
    public override bool CanActive() {
        return true;
    }
    public override void UpdateBehaviour(float deltaTime) {
        
    }
    public override int GetPriority() {
        return 0;
    }
    public override string GetAnimatorStateName() => "Idle";
}