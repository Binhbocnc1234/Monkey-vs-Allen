using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Initializer", menuName = "ScriptableObject/LevelInit/WeirdTatooSO")]
public class WeirdTatooSO : LevelInitializerSO {
    [SerializeField] private Sprite weirdTatooSO;
    public override void Execute(LevelSO so) {
        List<ICell> emptyCells = new();
        foreach(ICell cell in IGrid.Ins.cells) {
            if(cell.hasBlock == false) {
                emptyCells.Add(cell);
            }
        }
        int chosenInd = Random.Range(0, emptyCells.Count);
        emptyCells[chosenInd].blockRenderer.sprite = weirdTatooSO;
    }
}