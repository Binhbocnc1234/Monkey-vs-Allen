using UnityEngine;

public class Rotater : IBehaviour {
    protected override void UpdateBehaviour() {
        e.model.localScale = e.team == Team.Player ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
    }
}