using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitUI : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private TMP_Asset textAsset;
    public void Initialize(Sprite icon, string text) {
        this.icon.sprite = icon;
        tmp.text = text;
    }
}