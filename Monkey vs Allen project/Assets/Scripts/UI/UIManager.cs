using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//UI assembly contains: BananaCounterUI --> core, CardUI --> CardSO -> core
//UIManager born to solve UI's reference all 
public class UIManager : Singleton<UIManager>
{
    public HideAndShowUIManager hideShowManager;
    public Scrollbar selectedCardScrollbar;
    public FlashPanel flashPanel;
    public GameObject loseText;
    public RectTransform tryAgainUI;

    void Start(){
        loseText.gameObject.SetActive(false);
        tryAgainUI.gameObject.SetActive(false);
    }
    public void InitChoosingCard(){
        StartCoroutine(InitChoosingCardCoroutine());
    }
    public IEnumerator InitChoosingCardCoroutine(){
        BananaCounterUI.Ins.Initialize();
        PrepareUI.Ins.gameObject.SetActive(false);
        hideShowManager.HideAllImmediately();
        hideShowManager.Show("level");
        selectedCardScrollbar.value = 0;
        GridCamera.Ins.canDraging = false;
        yield return new WaitForSeconds(1f);
        GridCamera.Ins.MoveTowardEnemyHouse();
        yield return new WaitWhile(() => GridCamera.Ins.isMoving);
        if (BattleInfo.levelSO.canChooseCard){
            hideShowManager.ShowAll();
        }
        else{
            hideShowManager.Show("chosenCardContainer");
            hideShowManager.Show("pause");
            hideShowManager.Show("letsrock");
        }
    }

    public void PrepareForBattle(){
        StartCoroutine(PrepareForBattleCoroutine());
    }
    public IEnumerator PrepareForBattleCoroutine() {
        hideShowManager.Hide("level");
        hideShowManager.Hide("letsrock");
        hideShowManager.Hide("ownedCardContainer");
        hideShowManager.Show("chosenCardContainer");
        GridCamera.Ins.MoveTowardPlayerHouse();
        yield return new WaitWhile(() => GridCamera.Ins.isMoving);
        yield return StartCoroutine(PrepareUI.Ins.Act());
    }

    public IEnumerator Lose(){
        loseText.transform.localScale = Vector3.zero;
        float t = 0;
        while(true){
            t += Time.deltaTime;
            if (t >= 2){
                tryAgainUI.gameObject.SetActive(true);
                break;
            }
            else{
                yield return null;
            }
        }

    }
    public void AddUI(RectTransform child, RectTransform parent, Vector3 worldPos){
        Canvas canvas = GetComponent<Canvas>();
        Camera cam = canvas.worldCamera; // the camera assigned to the canvas

        // Convert world → screen → canvas local position
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            screenPos,
            cam,
            out Vector2 anchoredPos
        );

        child.SetParent(parent, false);
        child.anchoredPosition = anchoredPos;
    }
    
}
