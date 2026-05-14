using UnityEngine;


public class InactiveBehaviour : IBehaviour {
    public override bool CanActive() {
        return true;
    }
    public override void UpdateBehaviour(float deltaTime) {
        
    }
    public override int GetPriority() {
        return -1;
    }
    public override string GetAnimatorStateName() => "Idle";
}