using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/LevelInit/CreateBananaTreeSO")]
public class CreateBananaTreeSO : LevelInitializerSO {
    [SerializeField] private EntitySO bananaTreeSO;
    public override void Execute(LevelSO so) {
        BananaTree tree = EContainer.Ins.CreateEntity(bananaTreeSO, 2, 2, Team.Left, 1).GetComponent<BananaTree>();
        tree.bananaCount = BattleInfo.resourcePerGeneration * 2;
    }
}