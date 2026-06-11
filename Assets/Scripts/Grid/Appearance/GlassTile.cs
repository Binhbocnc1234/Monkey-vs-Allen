using System.Collections.Generic;
using UnityEngine;

public class GlassTile : CellView {
    [Range(0, 2)]
    public float flowerFrequency;
    public GridFlower flowerPrefab;
    public List<Sprite> flowerSprites;
    
    public void Start() {
        if (flowerSprites == null || flowerSprites.Count == 0 || flowerPrefab == null) return;
        
        Sprite chosenSprite = flowerSprites[Random.Range(0, flowerSprites.Count)];
        Vector2 blockPos = this.transform.position;
        float x = Random.Range(blockPos.x - IGrid.CELL_SIZE / 2, blockPos.x + IGrid.CELL_SIZE / 2);
        float y = Random.Range(blockPos.y - IGrid.CELL_SIZE / 2, blockPos.y + IGrid.CELL_SIZE / 2);
        Instantiate(flowerPrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform).Initialize(chosenSprite);
    }
}
