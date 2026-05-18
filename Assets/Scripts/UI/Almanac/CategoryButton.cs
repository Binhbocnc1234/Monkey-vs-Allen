using UnityEngine;

public class CategoryButton : MonoBehaviour {
    [SerializeField] private UIGradient gradient;
    public void Select() {
        gradient.enabled = true;
    }
    public void Deselect() {
        gradient.enabled = false;
    }
}