using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//Xử lý toàn bộ những tác vụ liên quan đến UI, ví dụ:
//Xử lý hoạt ảnh phóng to, thu nhỏ khi hover, xử lý việc gắn các sprite với Image trong canvas
//không xử lý logic kiểm tra xem người dùng liệu có dùng được lá bài ở vị trí này hay không
//không xử lý hành động mà lá bài thực hiện, chỉ gọi làm về Card
[RequireComponent(typeof(RectTransform))]
public class CardUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    [HideInInspector] public int index;
    public bool haveAppearanceIfEmpty;
    private bool _haveCooldown;
    public bool haveCooldown{
        get => _haveCooldown; 
        set{
            _haveCooldown = value;
            cooldownMaskRect.gameObject.SetActive(true);
        }
    }
    [ReadOnly] public bool isGreyOut = false;
    public CardSO so;
    public ICard card;
    public CardUIAppearance appearance;
    public Image cooldownMask;
    private float initialCooldownHeight;
    [ShowIf("haveAppearanceIfEmpty")] public RectTransform emptyRect;
    // public TMP_Text cost;
    private RectTransform cooldownMaskRect;
    public event Action<CardUI> OnClickEvent;
    void Start()
    {
        cooldownMask.gameObject.SetActive(haveCooldown);
        cooldownMaskRect = cooldownMask.rectTransform;
        initialCooldownHeight = cooldownMaskRect.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        if(so == null) { return; }
        if (BattleInfo.state == GameState.Fighting){
            if (haveCooldown){
                RectTransformExtensions.SetTop(cooldownMaskRect, initialCooldownHeight * card.GetCooldownPercent()); //problem
            }
            if (card.HaveEnoughBanana()){
                appearance.cost.color = Color.white;
            }
            else{
                appearance.cost.color = Color.red;
            }
        }
        else if (BattleInfo.state == GameState.ChoosingCard){
            if (haveAppearanceIfEmpty){
                
            }
        }
        
    }
    public void ApplyCardSO(CardSO cardSO){
        if(cardSO == null) { Debug.Log("true"); }
        this.gameObject.SetActive(true);
        so = cardSO;
        CardFrameSO cardFrameSO = CardFrameSO.GetObjectByRarity(cardSO.cardRarity);
        appearance.frame.sprite = cardFrameSO.frame;
        appearance.background.sprite = cardFrameSO.background;
        appearance.model.sprite = cardSO.sprite;
        appearance.cost.text = cardSO.cost.ToString();
        appearance.gameObject.SetActive(true);
        emptyRect.gameObject.SetActive(false);
    }
    public void RemoveCardSO(){
        so = null;
        if (haveAppearanceIfEmpty){
            emptyRect.gameObject.SetActive(true);
            appearance.gameObject.SetActive(false);
        }
        else{
            this.gameObject.SetActive(false);
        }
        
    }
    public void SetGreyOut(){
        appearance.model.color = Color.gray;
        appearance.background.color = Color.gray;
        appearance.frame.color = Color.gray;
        isGreyOut = true;
    }
    public void RemoveGreyOut(){
        appearance.model.color = Color.white;
        appearance.background.color = Color.white;
        appearance.frame.color = Color.white;
        isGreyOut = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // LeanTween.scale(this.gameObject, Vector3.one * 1.1f, 0.5f).setEaseOutBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // LeanTween.scale(this.gameObject, Vector3.one, 0.5f).setEaseOutBack();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isGreyOut) { return; }
        OnClickEvent?.Invoke(this);
    }
}
