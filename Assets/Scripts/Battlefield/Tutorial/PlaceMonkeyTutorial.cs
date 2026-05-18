using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Tutorial hướng dẫn người chơi chọn thẻ bài Monkey và đặt nó vào Grid
/// Không có điều kiện Start()
/// Hoàn thành Tutorial khi người chơi đặt thẻ Monkey thành công
/// </summary>
public class PlaceMonkeyTutorial : Tutorial{
    public ArrowUI arrowUIPrefab;
    public MonkeyCardSO basicMonkeySO;
    private ArrowUI arrowUI;
    BattleCardUI basicMonkeyCardUI;
    HinterManager hinterManager;
    public override void Initialize(){
        base.Initialize();
        hinterManager = SingletonRegister.Get<HinterManager>();
        // Need adjustment
        foreach(BattleCard card in BattleInfo.teamDict[Team.Left].cards){
            if (card.GetSO() == basicMonkeySO){
                basicMonkeyCardUI = SingletonRegister.Get<ChosenCardManager>().FindCardUIBySO(card.GetSO());
                card.cooldownTimer.FinishCooldown();
                card.Update();
                Action handler = null;
                handler = () => {
                    arrowUI.gameObject.SetActive(false);
                    for(int i = 0; i < GridSystem.Ins.width; ++i) {
                        hinterManager.NewHinter(GridSystem.Ins.cells[i, 2].gameObject);
                    }
                    card.cooldownTimer.FinishCooldown();
                    basicMonkeyCardUI.OnClickEvent -= handler;
                };
                basicMonkeyCardUI.OnClickEvent += handler;
            }
        }

        PointerUI.Ins.OnReleaseCard += CompleteTutorial;
        StartTutorial();
    }
    public override void StartTutorial() {
        base.StartTutorial();
        arrowUI = ArrowUI.Instantiate(Direction.Left, basicMonkeyCardUI.rect, basicMonkeyCardUI.transform);
        PauseManager.Ins.Pause();
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        PauseManager.Ins.DePause();
        PointerUI.Ins.OnReleaseCard -= CompleteTutorial;
        hinterManager.Reset();
    }

}