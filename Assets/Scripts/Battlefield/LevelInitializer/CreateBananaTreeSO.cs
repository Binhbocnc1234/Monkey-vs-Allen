using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/LevelInit/CreateBananaTreeSO")]
public class CreateBananaTreeSO : LevelInitializerSO {
    [SerializeField] private EntitySO bananaTreeSO;
    public override void Execute(LevelSO so) {
        IEntity entity = EContainer.Ins.CreateEntity(new EntitySetting { so = bananaTreeSO, x = 2, lane = 2, team = Team.Left, level = 1 });
        BananaTree tree = entity.GetBehaviour<BananaTree>();
        if(tree != null) {
            tree.bananaCount = BattleInfo.resourcePerGeneration * 2;
        }
    }
}