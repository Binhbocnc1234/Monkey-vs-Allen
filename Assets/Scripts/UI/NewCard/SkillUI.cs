using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour {
    public Transform header;
    public Image icon;
    public TMP_Text skillName;
    public TMP_Text skillDescription;
    public void UpdateInfo(SkillSO so, bool unlocked, int skillLevel) {
        if(so.haveNameAndIcon) {
            icon.sprite = so.thumbnail;
            skillName.text = (so.isActiveSkill ? "Active Skill - " : "") + so.skillName;
        }
        else {
            header.gameObject.SetActive(false);
        }

        if(!unlocked) {
            icon.color = Color.gray;
            skillName.color = Color.gray;
            skillDescription.color = Color.gray;
            skillDescription.text = "[Upgrade to unlock]\n" + so.GetDescription(skillLevel);
        }
        else {
            skillDescription.text = so.GetDescription(skillLevel);
        }
    }
    // public void UpdateInfo(SkillSO so, bool unlockedAtLv3, bool )
}