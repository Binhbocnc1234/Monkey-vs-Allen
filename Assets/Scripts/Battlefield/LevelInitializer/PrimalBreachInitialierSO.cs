using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GardenFieldInitializerSO", menuName = "ScriptableObject/LevelInit/GardenFieldInitializer")]
public class PrimalBreachInitializerSO : LevelInitializerSO {
    public Sprite glassTile, glassTile_2;
    public Sprite[] dirtSprite;
    public PrimalBreachEnviroment enviroment;
    public override void Execute(LevelSO so) {
        foreach(ICell cell in GridSystem.Ins.cells) {
            Sprite chosen = (cell.gridPosition.x + cell.gridPosition.y) % 2 == 0 ? glassTile : glassTile_2;
            if(so.openLanes[cell.gridPosition.y]) {
                cell.PlaceBlock(chosen);
            }
            else {
                cell.blockRenderer.sprite = GetRandomDirt();
            }
        }
        SlidingCamera.
        Instantiate(enviroment).Initialize(IGrid.Ins.width);
    }
    Sprite GetRandomDirt() {
        return dirtSprite[Random.Range(0, dirtSprite.Length)];
    }
}
