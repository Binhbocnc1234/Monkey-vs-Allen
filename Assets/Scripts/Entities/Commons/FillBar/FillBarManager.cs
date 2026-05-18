using System;
using UnityEngine;

public class FillBarManager : Singleton<FillBarManager> {
    [SerializeField] private HealthBar healthbarPrefab;
    [SerializeField] private FillBar prefab;
    public void CreateHealthBar(IEntity e) {
        var newHealthBar = Instantiate(healthbarPrefab, this.transform);
        newHealthBar.BindEntity(e);
    }
    public FillBar CreateProgressBar() {
        return Instantiate(prefab, this.transform);
    }
}