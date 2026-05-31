using UnityEngine;

[System.Serializable]
public class StraightMove : Move {
    public override void UpdateBehaviour(float deltaTime) {
        e.gridPos += new Vector2(GetUnityMoveSpeed(e) * deltaTime * GetNormalizedDirection(), 0);
    }
}
