using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private TMP_Asset textAsset;
    public void Initialize(Sprite icon, float num) {
        this.icon.sprite = icon;
        tmp.text = num.ToString();
    }
}