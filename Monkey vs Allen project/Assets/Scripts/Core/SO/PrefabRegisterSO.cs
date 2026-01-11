using UnityEngine;

[CreateAssetMenu(fileName = "PrefabRegister", menuName = "ScriptableObject/Singleton/PrefabRegister")]
public class PrefabRegisterSO : MySO {
    public RectTransform arrowUI;
    public GameObject arrow;
    public EntitySO builder;
    public GameObject loadingUI;
    public GameObject dollarGameObject;
}