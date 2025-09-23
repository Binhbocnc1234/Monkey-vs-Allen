using UnityEngine;

public class Cell : ICell
{
    private SpriteRenderer cellRenderer;
    void Awake()
    {
        cellRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        
    }
    
    public bool CanPlaceBlock()
    {
        return !hasBlock && !hasEntity;
    }
    
    public bool CanPlaceTower()
    {
        return hasBlock && !hasEntity;
    }
    public override void PlaceBlock(){
        blockObject = Instantiate(GridSystem.Instance.blockPrefab, transform.position, Quaternion.identity, this.transform);
        hasBlock = true;
    }
    // public void Place
    public override GameObject PlaceObject(GameObject prefab){
        entityObject = Instantiate(prefab, transform.position, Quaternion.identity);
        hasEntity = true;
        return entityObject;
    }
}
