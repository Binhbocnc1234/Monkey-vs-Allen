using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RampurVillageInitializerSO", menuName = "ScriptableObject/LevelInit/RampurVillageInitializerSO")]
public class RampurVillageInitializerSO : LevelInitializerSO {
    public Sprite glassTile, glassTile_2;
    public Sprite[] dirtSprite;
    public GameObject enviroment;
    public override void Execute(LevelSO so) {
        foreach(ICell cell in IGrid.Ins.cells) {
            Sprite chosen = (cell.gridPosition.x + cell.gridPosition.y) % 2 == 0 ? glassTile : glassTile_2;
            if(so.openLanes[cell.gridPosition.y]) {
                cell.PlaceBlock(chosen);
            }
            else {
                cell.blockRenderer.sprite = GetRandomDirt();
            }
        }
    }
    Sprite GetRandomDirt() {
        return dirtSprite[Random.Range(0, dirtSprite.Length)];
    }
}
