using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FreePlayManager : Singleton<FreePlayManager>{
    public GameObject freePlayUI;
    public LevelSO levelSO;
    public void Initialize() {
        BattleInfo.Initialize(levelSO);

        GridSystem grid = GridSystem.Ins;
        grid.Clear();
        grid.Initialize(levelSO.gridWidth, levelSO.openLanes);

        EContainer.Ins.Initialize();
        EContainer.Ins.ClearEntity();

        // Initialize enviroment
        PlaceInitializerMapSO placeMap = SORegistry.Get<PlaceInitializerMapSO>()[0];
        placeMap.initializers[Place.PrimalBreach].Execute(levelSO);

        foreach(LevelInitializerSO initializerSO in levelSO.levelInitializerSOs) {
            initializerSO.Execute(levelSO);
        }

        Prize.Ins.gameObject.SetActive(false);
        EnemyManager.Ins.gameObject.SetActive(false);
        SingletonRegister.Get<OwnedCardManager>().SetReferencedList(PlayerData.GetOwnedCard().ToList<CardSO>());
        // SingletonRegister.Get<ChosenCardManager>().SetReferencedList
        // UI
        PrepareUI.Ins.gameObject.SetActive(false);
        UIManager.Ins.freePlayUI.gameObject.SetActive(true);
        UIManager.Ins.levelUI.InitializeFreePlay();
        UIManager.Ins.letsRockUI.gameObject.SetActive(false);
        var hideShowManager = UIManager.Ins.hideShowManager;
        hideShowManager.ShowAll();
        hideShowManager.Disable("seeBattlefield");
        hideShowManager.Hide("level");
        BattleInfo.ChangeState(GameState.Fighting);
    }
}