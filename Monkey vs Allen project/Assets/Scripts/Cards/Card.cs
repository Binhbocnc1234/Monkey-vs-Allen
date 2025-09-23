using UnityEngine;

//Card không nên là một thực thể
public class Card : ICard
{
    void Update(){
        cooldownTimer.Count(false);
    }
    public override void ApplyCardSO(CardSO so){
        this.so = so;
        this.cost = so.cost;
        SetCoolDown(so.cooldown);
    }
    public override bool CanUseCard(Vector2Int gridPosition){
        if (IGrid.Instance.IsValidGridPosition(gridPosition.x, gridPosition.y) == false){
            return false;
        }
        ICell cell = IGrid.Instance.GetCell(gridPosition);
        if (so.condition == CardCondition.Summon){
            return cell.hasBlock && HaveEnoughBanana();
        }
        else if (so.condition == CardCondition.Buff){

        }
        return false;
    }
    public override void UseCard(Vector2Int gridPosition) {
        if (so.condition == CardCondition.Summon){
            Entity e = so.prefab.GetComponent<Entity>();
            if (e != null && e.entityType == EntityType.Monkey){
                gridPosition.x = 0;
            }
            IGrid.Instance.GetCell(gridPosition).PlaceObject(so.prefab).GetComponent<Entity>().Initialize();
            
        }
        BattleInfo.ChangeBananaCnt(-cost);
        cooldownTimer.Reset();
    }
    public float GetCurrentCoolDown(){
        return cooldownTimer.curTime;
    }
    public void SetCoolDown(float newCoolDown){
        cooldown = newCoolDown;
        cooldownTimer = new Timer(newCoolDown);
    }
    public override float GetCooldownPercent() {
        return cooldownTimer.curTime / cooldownTimer.totalTime;
    }
    public override bool HaveEnoughBanana(){
        return BattleInfo.BananaCnt >= cost;
    }
}
