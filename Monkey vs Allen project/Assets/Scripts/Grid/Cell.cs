using UnityEngine;

public class Cell : ICell
{
    // public SpriteRenderer blockRenderer;
    // private SpriteRenderer cellRenderer;
    void Awake()
    {
        // cellRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        
    }
    
    public bool CanPlaceTower()
    {
        return hasBlock;
    }
    public override void PlaceBlock(Sprite blockSprite){
        blockObject = Instantiate(GridSystem.Ins.blockPrefab, transform.position, Quaternion.identity, this.transform);
        blockObject.GetComponent<SpriteRenderer>().sprite = blockSprite;
        hasBlock = true;
    }
    // public void Place
    public override GameObject PlaceObject(GameObject prefab){
        entityObject = Instantiate(prefab, transform.position, Quaternion.identity);
        return entityObject;
    }
}
