using System.Collections.Generic;
using UnityEngine;


public class CloudManager : MonoBehaviour {
    public Transform container;
    public Sprite[] sprites;
    public Cloud cloudPrefab;
    public Bound bound;
    public Direction direction;
    public int initialCloudCount;
    public float delayBetweenCloud;
    public float cloudSpeed;
    Timer delayTimer;
    void Awake() {
        delayTimer = new Timer(delayBetweenCloud, reset:true);
        for(int i = 0; i < initialCloudCount; ++i) {
            CreateCloud();
        }
    }
    void Update() {
        if(delayTimer.Count()) {
            CreateCloud();
        }
    }
    void CreateCloud() {
        Cloud gObj = Instantiate(cloudPrefab, bound.RandomPosInBound(),
                Quaternion.identity,
                container);
        gObj.Initialize(GetRandomCloud(), direction, cloudSpeed * Random.Range(0.7f, 1.3f),
             Random.Range(0.5f, 1f), bound);
    }
    Sprite GetRandomCloud() {
        return sprites[Random.Range(0, sprites.Length)];
    }
}