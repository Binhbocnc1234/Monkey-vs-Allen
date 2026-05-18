using UnityEngine;
using UnityEngine.Rendering;

public class Cell : ICell
{
    public void Initialize(int x, int y) {
        this.gridPosition = new Vector2Int(x, y);
        GetComponent<SortingGroup>().sortingOrder = IGrid.Ins.height - y;
        this.name = $"Cell_{x}_{y}";
    }
    public override void PlaceBlock(Sprite blockSprite) {
        hasBlock = true;
        blockRenderer.sprite = blockSprite;
    }
}
