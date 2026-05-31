using UnityEngine;

public class BananaTreeAppearance : EntityAppearance
{
    public Transform bananaBunch;
    public float growProgress;
    public override void Initialize(EntityModel model) {
        base.Initialize(model);
        BananaTree behav = e.GetBehaviour<BananaTree>();
        if(behav == null) {
            Debug.LogError("[BananaTreeAppearance] No BananaTree behaviour founded");
        }
        behav.OnBananaGenerated += () => {
            Banana bananaBunch = GeneralPurposeContainer.Ins.CreateInstance(SingletonRegister.Get<PrefabRegisterSO>().bananaBunch.GetComponent<Banana>(), e.model.GetPosition());
            bananaBunch.Initialize(e.lane);
            bananaBunch.SetBananaCount(behav.bananaCount);
        };
    }
    void Update() {
        if (e.GetActiveBehaviour() is BananaTree bananaTree) {
            growProgress = bananaTree.growProgress;
            bananaBunch.localScale = Vector3.one * bananaTree.growProgress;
        }
    }
}