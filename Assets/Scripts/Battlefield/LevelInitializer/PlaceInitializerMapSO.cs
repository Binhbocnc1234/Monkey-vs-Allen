using UnityEngine;

[CreateAssetMenu(fileName = "PlaceInitializerMap", menuName = "ScriptableObject/Singleton/PlaceInitializerMapSO")]
public class PlaceInitializerMapSO : MySO {
    public UDictionary<Place, LevelInitializerSO> initializers;
}