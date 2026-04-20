using System.Collections.Generic;
using UnityEngine;
using System;

public class UnfinishedTower : MonoBehaviour {
    public bool isFirstSlotOccupied;
    private EntitySetting entitySetting;
    public float progress = 0;
    private float constructionTime = 10;
    private FillBar progressBar;
    public void Initialize(EntitySetting setting) {
        entitySetting = setting;
        progressBar = FillBarManager.Ins.CreateProgressBar();
        this.transform.position = IGrid.Ins.GridToWorldPosition(setting.x, setting.lane);
        progressBar.transform.position = this.transform.position - Vector3.down * 2;
        progressBar.gameObject.SetActive(false);
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
        progressBar.gameObject.SetActive(true);
        progress++;
        if(progress >= constructionTime) {
            IEntity e = IEntityRegistry.Ins.CreateEntity(entitySetting);
            ICell cell = IGrid.Ins.GetCell((int)entitySetting.x, entitySetting.lane);
            cell.occupiedByTower = true;
            e.OnEntityDeath += () => cell.occupiedByTower = false;
            Destroy(this.gameObject);
            Destroy(progressBar.gameObject);
            return true;
        }
        else {
            progressBar.SetValue(progress / constructionTime);
            return false;
        }
    }
}