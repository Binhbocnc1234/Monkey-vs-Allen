using UnityEngine;


/// <summary>
/// Đây là class chứa tham chiếu đến các prefab, chỉ nên tạo một file trong Asset
/// </summary>
[CreateAssetMenu(fileName = "PrefabRegister", menuName = "ScriptableObject/Singleton/PrefabRegister")]
public class PrefabRegisterSO : MySO {
    public RectTransform arrowUI;
    public GameObject arrow;
    public EntitySO builder;
    public GameObject unfinishedTower;
    public GameObject loadingUI;
    public GameObject dollarGameObject;
    public GameObject bananaBunch;
    public GameObject emptyGameObject;
    public GameObject alienSpawningEffect;
    public UDictionary<ST, Sprite> statIconMap;
}