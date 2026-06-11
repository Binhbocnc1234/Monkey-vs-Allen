using UnityEngine;

public class LogicalCell : ICell
{
    public LogicalCell(int x, int y)
    {
        this.gridPosition = new Vector2Int(x, y);
    }

    public override void PlaceBlock(Sprite blockSprite)
    {
        hasBlock = true;
        NotifyStateChanged();
        NotifySpriteChanged(blockSprite);
    }

    public override void SetSpriteOnly(Sprite sprite)
    {
        NotifySpriteChanged(sprite);
    }
}
