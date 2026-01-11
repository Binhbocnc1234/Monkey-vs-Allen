using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FlowerGenerator : MonoBehaviour
{
    public int flowerCount;
    public GridFlower flowerPrefab;
    public List<Sprite> flowerSprites;
    public void Start(){
        GridBound bounds = GridSystem.Ins.bounds;
        for(int i = 0; i < flowerCount; ++i){
            float x = Random.Range(bounds.left, bounds.right);
            float y = Random.Range(bounds.bottom, bounds.top);
            Sprite chosenSprite = flowerSprites[Random.Range(0, flowerSprites.Count)];
            Instantiate(flowerPrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform).Initialize(chosenSprite);
        }
    }
}
