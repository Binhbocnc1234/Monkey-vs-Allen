using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PointerState {
    InActive,
    HoldingCard,
}
public class PointerUI : Singleton<PointerUI>
{
    // Start is called before the first frame update
    [ReadOnly] public PointerState state;
    public IBattleCard card;
    [ReadOnly] public Vector2Int pointedGridPosition;
    public SpriteRenderer transparentModel;
    private SpriteRenderer spriteRenderer;
    public Action OnReleaseCard;
    [HideInInspector] public bool isTutorial = false;
    private IGrid grid;
    private Timer delayAction = new Timer(0.25f, false);
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        grid = IGrid.Ins;
        HidePointer();
        transparentModel.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Make object stick to pointer
        Vector3 mousePos = Pointer.current.position.ReadValue();
        Vector2 tmp = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = tmp;
        //Get pointed cell in grid
        pointedGridPosition = grid.WorldToGridPosition(transform.position);
        delayAction.Count();
        if (card.CanUseCard(pointedGridPosition)){
            transparentModel.enabled = true;
            transparentModel.transform.position = grid.GridToWorldPosition(pointedGridPosition);
            if (Input.GetMouseButtonDown(0) && delayAction.isEnd){
                OnReleaseCard?.Invoke();
                card.UseCard(pointedGridPosition);
                HidePointer();
                delayAction.Reset();
            }
        }
        else{ // Ẩn pointer đi nếu người chơi chọn vị trí thực thi lá bài không hợp lệ
            transparentModel.enabled = false;
            if (Input.GetMouseButtonDown(0) && delayAction.isEnd && TechnicalInfo.isTutorial == false){
                HidePointer();
                delayAction.Reset();
                
            }
        }
        
    }

    public void SetHoldingCard(IBattleCard monkeyCard){
        this.gameObject.SetActive(true);
        this.card = monkeyCard;
        spriteRenderer.sprite = monkeyCard.GetSO().sprite;
        transparentModel.sprite = monkeyCard.GetSO().sprite;
        delayAction.Reset();
        state = PointerState.HoldingCard;
    }
    public void HidePointer(){
        this.gameObject.SetActive(false);
        transparentModel.enabled = false;
        state = PointerState.InActive;
    }
    // void SetPointerState(PointerState state){
    //     this.state = state;
    // }

    

}
