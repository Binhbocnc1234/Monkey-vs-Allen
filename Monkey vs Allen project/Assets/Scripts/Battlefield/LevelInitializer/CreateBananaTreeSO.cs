using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/LevelInit/CreateBananaTreeSO")]
public class CreateBananaTreeSO : LevelInitializerSO {
    [SerializeField] private EntitySO bananaTreeSO;
    public override void Execute(LevelSO so) {
        EContainer.Ins.CreateEntity(bananaTreeSO, 2, 2, Team.Player, 1);
    }
}