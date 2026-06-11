using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class CellView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blockRenderer;
    
    public ICell logicalCell { get; private set; }

    public void Bind(ICell cell)
    {
        logicalCell = cell;
        
        // Setup visual naming and layering order
        this.name = $"Cell_{cell.gridPosition.x}_{cell.gridPosition.y}";
        GetComponent<SortingGroup>().sortingOrder = IGrid.Ins.height - cell.gridPosition.y;

        // Subscribe to logic changes
        logicalCell.OnStateChanged += HandleStateChanged;
        logicalCell.OnSpriteChanged += HandleSpriteChanged;

        UpdateVisuals();
    }

    private void HandleStateChanged(ICell cell)
    {
        UpdateVisuals();
    }

    private void HandleSpriteChanged(ICell cell, Sprite sprite)
    {
        if (blockRenderer != null)
        {
            blockRenderer.sprite = sprite;
        }
    }

    private void UpdateVisuals()
    {
        // Add any generic cell state updates if needed in the future
    }

    private void OnDestroy()
    {
        if (logicalCell != null)
        {
            logicalCell.OnStateChanged -= HandleStateChanged;
            logicalCell.OnSpriteChanged -= HandleSpriteChanged;
        }
    }
}
