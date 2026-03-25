using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SeeBattleFieldUI : HideAndShowUI {
    private bool isSeeingBattleField;
    private Button button;
    private HideAndShowUIManager hideShowManager;
    protected override void Awake() {
        base.Awake();
        button = GetComponent<Button>();
    }
    void Start() {
        hideShowManager = SingletonRegister.Get<UIManager>().hideShowManager;
        GridCamera.Ins.OnFinishedMoving += () => SlidingCamera.Ins.enable = isSeeingBattleField;
    }
    public void OnClick() {
        isSeeingBattleField = !isSeeingBattleField;
        if(isSeeingBattleField) {
            GridCamera.Ins.MoveTowardPlayerHouse();
            hideShowManager.Hide("ownedCardContainer");
            hideShowManager.Hide("letsrock");
            hideShowManager.Show("level");
        }
        else {
            GridCamera.Ins.MoveTowardEnemyHouse();
            hideShowManager.Show("ownedCardContainer");
            hideShowManager.Show("letsrock");
            hideShowManager.Hide("level");
        }
        
    }
    // void ToggleSliding(bool value) {
    //     SlidingCamera.Ins.enable = value;
    // }
    void Update() {
        button.interactable = !GridCamera.Ins.isMoving;
    }
}
