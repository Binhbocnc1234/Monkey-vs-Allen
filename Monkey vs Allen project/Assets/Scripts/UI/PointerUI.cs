using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerUI : Singleton<PointerUI>
{
    // Start is called before the first frame update
    public ICard card;
    [ReadOnly] public Vector2Int pointedGridPosition;
    public SpriteRenderer transparentModel;
    private SpriteRenderer spriteRenderer;
    public Action OnReleaseCard;
    
    private IGrid grid;
    private Timer delayAction = new Timer(0.25f);
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        grid = IGrid.Instance;
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
        delayAction.Count(false);
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
        else{
            transparentModel.enabled = false;
            if (Input.GetMouseButtonDown(0) && delayAction.isEnd){
                HidePointer();
                delayAction.Reset();
            }
        }
        
    }
    void FollowCursor()
    {
        // Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        // Vector2 mousePos = Mouse.current.position.ReadValue(); // always updates

        // // normalize to 0–1
        // Vector2 normalized = new Vector2(mousePos.x / screenSize.x,
        //                                 mousePos.y / screenSize.y);

        // // scale to canvas rect
        // RectTransform canvasRect = UIManager.Instance.GetComponent<Canvas>().transform as RectTransform;
        // Vector2 localPos = new Vector2(
        //     (normalized.x - 0.5f) * canvasRect.sizeDelta.x,
        //     (normalized.y - 0.5f) * canvasRect.sizeDelta.y
        // );

        // transform.localPosition = localPos;
    }

    public void UpdateHoldingCard(ICard card){
        this.gameObject.SetActive(true);
        spriteRenderer.sprite = card.so.sprite;
        transparentModel.sprite = card.so.sprite;
        this.card = card;
        delayAction.Reset();
    }
    public void HidePointer(){
        this.gameObject.SetActive(false);
        transparentModel.enabled = false;
    }

    

}
