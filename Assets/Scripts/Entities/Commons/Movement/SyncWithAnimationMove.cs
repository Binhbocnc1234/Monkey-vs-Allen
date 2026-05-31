[System.Serializable]
public class SyncWithAnimationMove : StraightMove {
    public override void UpdateBehaviour(float deltaTime) {
        if(e.isSimulated) {
            // Behave like StraightMove
            base.UpdateBehaviour(deltaTime);
        }
        else {
            // Sync with model position
            e.gridPos = IGrid.Ins.WorldToGridPos(e.model.GetPosition());
        }

    }
}