using UnityEngine;
using System.Collections;

//Tương lai: sửa lỗi không ấn được vào CardUI do Monobehaviour bị disabled
public class SelectBananaTreeTutorial : Tutorial{
    public ArrowUI arrowUIPrefab;
    public Arrow arrowPrefab;
    public GameObject cellHinterPrefab;
    public MonkeyCardSO bananaTreeSO;
    private ArrowUI arrowUI;
    private GameObject cellHinter;
    private Arrow arrow;
    BattleCardUI bananaTreeCardUI;
    public override void Initialize(){
        base.Initialize();
        bool foundBananaCard = false;
        foreach(BattleCard card in BattleInfo.chosenAllies){
            BattleCardUI cardUI = card.cardUI;
            if (card.GetSO() == bananaTreeSO){
                bananaTreeCardUI = cardUI;
                card.cooldownTimer.SetCurTime(card.cooldownTimer.totalTime);
                card.Update();
                cardUI.OnClickEvent += NextInstruction;
                foundBananaCard = true;
            }
        }
        if(foundBananaCard == false) { Debug.LogError("SelectBananaTreeTutorial: Not found BananaTree card"); }

        PointerUI.Ins.OnReleaseCard += CompleteTutorial;
        StartTutorial();
    }
    public override void StartTutorial() {
        Debug.Log("SelectBananaTree: StartTutorial");
        base.StartTutorial();
        ArrowUI.Instantiate(Direction.Left, bananaTreeCardUI.rect, bananaTreeCardUI.transform);
        PauseManager.Ins.Pause();
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        PauseManager.Ins.DePause();
        PointerUI.Ins.OnReleaseCard -= CompleteTutorial;
        bananaTreeCardUI.OnClickEvent -= NextInstruction;
        Destroy(arrow.gameObject);
        Destroy(cellHinter);
    }
    /// <summary>
    /// Place card at designated cell
    /// </summary>
    /// <param name="cardUI"></param>
    void NextInstruction(CardUI cardUI){
        arrowUI.gameObject.SetActive(false);
        Vector3 cellPos = GridSystem.Ins.GridToWorldPosition(2, 2);
        arrow = Instantiate(arrowPrefab, cellPos, Quaternion.identity, this.transform);
        cellHinter = Instantiate(cellHinterPrefab, cellPos, Quaternion.identity, this.transform);
        arrow.pointingDirection = Direction.Down;
        Debug.Log("Next instruction");
    }

}