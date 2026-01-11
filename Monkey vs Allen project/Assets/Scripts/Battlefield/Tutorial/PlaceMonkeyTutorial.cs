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
        hinterManager = GetComponent<HinterManager>();
        foreach(BattleCard card in BattleInfo.chosenAllies){
            BattleCardUI cardUI = card.cardUI;
            if (card.GetSO() == basicMonkeySO){
                basicMonkeyCardUI = cardUI;
                card.cooldownTimer.FinishCooldown();
                card.Update();
                Action<CardUI> handler = null;
                handler = (cardUI) => {
                    arrowUI.gameObject.SetActive(false);
                    for(int i = 0; i < GridSystem.Ins.width; ++i) {
                        hinterManager.NewHinter(GridSystem.Ins.cells[i, 2].blockObject);
                    }
                    card.cooldownTimer.FinishCooldown();
                    cardUI.OnClickEvent -= handler;
                };
                cardUI.OnClickEvent += handler;
            }
        }

        PointerUI.Ins.OnReleaseCard += CompleteTutorial;
        StartTutorial();
    }
    public override void StartTutorial() {
        base.StartTutorial();
        arrowUI = ArrowUI.Instantiate(Direction.Left, basicMonkeyCardUI.rect, basicMonkeyCardUI.transform);
        EContainer.Ins.InActiveAll();
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        EContainer.Ins.IdleAll();
        PointerUI.Ins.OnReleaseCard -= CompleteTutorial;
        hinterManager.Reset();
    }

}