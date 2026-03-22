using UnityEngine;

public class PowerOfDollar : Skill, IOnApply {
    public override bool CanActive() {
        return false;
    }
    public override void OnApply() {
        base.OnApply();
        e.model.PlayAnimation("SupremeGun");
        for(int i = 0; i < 10; ++i) {
            var thisPos = IGrid.Ins.WorldToGridPosRounded(e.transform.position);
            Vector2Int pos = new Vector2Int(Random.Range(thisPos.x - 3, thisPos.x), Random.Range(thisPos.y - 3, thisPos.y));
            Instantiate(SingletonRegister.Get<PrefabRegisterSO>().dollarGameObject, e.transform).GetComponent<Dollar>().Initialize(pos);
        }
    }
}