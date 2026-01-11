using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EnumAndIcon {
    public TraitType trait;
    public Sprite icon;

}
[CreateAssetMenu(fileName = "NewTraitIconMap", menuName = "ScriptableObject/TraitIconMapSO")]
public class CardIconMapSO : ScriptableObject {
    public Sprite healthIcon, damageIcon, rangeIcon, attackSpeedIcon, moveSpeedIcon;
    public Sprite attackRangeIcon;
    public Sprite costIcon;
    [SerializeField] private List<EnumAndIcon> iconMap;
    public Sprite GetIconByEnum(TraitType trait) {
        foreach(EnumAndIcon ei in iconMap) {
            if(ei.trait == trait) {
                return ei.icon;
            }
        }
        Debug.LogError($"[TraitIconMapSO] Cannot find sprite for trait {trait}");
        return null;
    }
}