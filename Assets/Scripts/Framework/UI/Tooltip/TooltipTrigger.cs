using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private string tooltipText;
    [SerializeField] private RectTransform tooltipOffset; // Optional: offset từ cursor
    
    private Tooltip tooltip;
    
    void Start() {
        tooltip = SingletonRegister.Get<Tooltip>();
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        if (string.IsNullOrEmpty(tooltipText)) return;
        
        tooltip.Initialize(tooltipText);
        tooltip.Show(transform as RectTransform, eventData.position);
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        tooltip.Hide();
    }
}