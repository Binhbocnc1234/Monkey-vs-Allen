using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    [HideInInspector] public RectTransform rect;
    public bool haveAppearanceIfEmpty;
    [SerializeField] private bool isGreyOut = false;
    public CardSO so;
    public CardUIAppearance appearance;
    [ShowIf("haveAppearanceIfEmpty")] public RectTransform emptyRect;
    // public TMP_Text cost;
    public event Action<CardUI> OnClickEvent;
    public EventChannel channel = new();
    protected virtual void Start()
    {
        rect = GetComponent<RectTransform>();
        RemoveGreyOut();
    }
    public class OnCardClick : MyEvent<CardUI>{
        public OnCardClick(CardUI caller) : base(caller){}
    }
    public virtual void ApplyCardSO(CardSO cardSO){
        if(cardSO == null) { Debug.LogError("cardSO is null"); }
        this.gameObject.SetActive(true);
        so = cardSO;
        appearance.Initialize(cardSO);
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
    public bool HaveCardAttached(){
        return so != null;
    }
    [ContextMenu("SetGreyOut")]
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
    public Vector2 GetLocalPos(){
        return new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - rect.rect.width / 2);
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

    public void OnPointerClick(PointerEventData eventData) {
        if(isGreyOut || !HaveCardAttached()) { return; }
        OnClickEvent?.Invoke(this);
        channel.Invoke(new OnCardClick(this));
    }
    public void Validate() {
        if(so != null) {
            ApplyCardSO(so);
        }
        else {
            RemoveCardSO();
        }
        if(isGreyOut) {
            SetGreyOut();
        }
        else {
            RemoveGreyOut();
        }
    }
}
