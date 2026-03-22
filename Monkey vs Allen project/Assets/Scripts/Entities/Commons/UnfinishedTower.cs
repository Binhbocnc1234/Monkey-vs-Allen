using System.Collections.Generic;
using UnityEngine;

public class UnfinishedTower : MonoBehaviour {
    public bool isFirstSlotOccupied;
    private EntitySetting entitySetting;
    private EntitySO so;
    private BuildBehaviour firstBuilder, secondBuilder;
    private int lane, x;
    private Team team;
    public float progress = 0;
    public float constructionTime = 10;
    public void Initialize(EntitySetting setting, BuildBehaviour builder_1, BuildBehaviour builder_2) {
        firstBuilder = builder_1;
        secondBuilder = builder_2;
        entitySetting = setting;
    }
    public void AddProgress() {
        progress++;
        if (progress >= constructionTime) {
            firstBuilder.ReturnToHome();
            secondBuilder.ReturnToHome();
            IEntityRegistry.Ins.CreateEntity(entitySetting);
            Destroy(this.gameObject);
        }
    }
}