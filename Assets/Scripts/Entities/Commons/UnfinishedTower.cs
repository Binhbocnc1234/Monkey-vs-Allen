using UnityEngine;
using System;

public class UnfinishedTower : MonoBehaviour {
    public bool isFirstSlotOccupied;
    public event Action<float> OnProgressChanged;
    public event Action OnConstructionComplete;
    private EntitySetting entitySetting;
    public Vector2 GridPos => entitySetting != null ? new Vector2(entitySetting.x, entitySetting.lane) : Vector2.zero;
    public float progress = 0;
    private float constructionTime = 10;
    public float GetConstructionTime() => constructionTime;
    public void Initialize(EntitySetting setting) {
        entitySetting = setting;
        this.transform.position = IGrid.Ins.GridToWorldPosition(setting.x, setting.lane);
        this.gameObject.SetActive(false);
        TowerSO so = (TowerSO)setting.so;
        if(so.buildTime == BuildTime.Low) {
            constructionTime = 5;
        }
        else {
            constructionTime = 15;
        }
    }
    public bool AddProgress() {
        this.gameObject.SetActive(true);
        progress++;
        float normalized = progress / constructionTime;
        OnProgressChanged?.Invoke(normalized);
        if(progress >= constructionTime) {
            IEntity e = IEntityRegistry.Ins.CreateEntity(entitySetting);
            ICell cell = IGrid.Ins.GetCell((int)entitySetting.x, entitySetting.lane);
            cell.occupiedByTower = true;
            e.OnEntityDeath += () => cell.occupiedByTower = false;
            OnConstructionComplete?.Invoke();
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }
}
