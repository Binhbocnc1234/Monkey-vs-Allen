using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerSO", menuName = "ScriptableObject/Tower")]
public class TowerSO : EntitySO {
    [Header("Tower specific fields")]
    public BuildTime buildTime = BuildTime.Low;
}