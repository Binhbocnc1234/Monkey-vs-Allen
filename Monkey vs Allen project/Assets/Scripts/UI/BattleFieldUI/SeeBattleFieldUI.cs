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
    }
    public void OnClick() {
        if(!isSeeingBattleField) {
            hideShowManager.Show("level");
            hideShowManager.Hide("ownedCardContainer");
            hideShowManager.Hide("chosenCardContainer");
            hideShowManager.Hide("letsrock");
            GridCamera.Ins.MoveTowardPlayerHouse();
            GridCamera.Ins.OnFinishedMoving += () => {
                GridCamera.Ins.canDraging = true;
            };
        }
        else {
            hideShowManager.Hide("level");
            hideShowManager.Show("ownedCardContainer");
            hideShowManager.Show("chosenCardContainer");
            hideShowManager.Show("letsrock");
            GridCamera.Ins.MoveTowardEnemyHouse();
            GridCamera.Ins.canDraging = false;
        }
    }
    void Update() {
        button.interactable = !GridCamera.Ins.isMoving;
    }
}
