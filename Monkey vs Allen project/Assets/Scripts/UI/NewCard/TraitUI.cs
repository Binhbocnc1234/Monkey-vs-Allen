using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitUI : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private TMP_Asset textAsset;
    public void Initialize(EffectType trait) {
        EffectSO so = SORegistry.GetSOByName<EffectSO>(trait.ToString());
        icon.sprite = so.thumbnail;
        tmp.text = trait.ToString();
    }
}