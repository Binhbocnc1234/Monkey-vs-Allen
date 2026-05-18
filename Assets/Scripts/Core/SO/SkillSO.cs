using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SkillSO", menuName = "ScriptableObject/SkillSO")]
public class SkillSO : MySO {
    public bool isActiveSkill;
    public bool haveNameAndIcon = true;
    [ShowIf("haveNameAndIcon")] public string skillName;// Tên dùng để hiển thị với người dùng
    [ShowIf("haveNameAndIcon")] public Sprite thumbnail;
    // Do những con số trong description có thể bị thay đổi bởi Upgrade nên chúng được biểu diễn bằng skillStat
    public UDictionary<string, int> skillStats;
    public LocalizedDescription description;
    public SkillUpgradeInfo skillUp;
    public int GetStat(string name, int upgradeCnt = 0) {
        return skillStats[name] + (skillUp.statName == name ? skillUp.changeAmount*upgradeCnt : 0);
    }
    public string GetDescription(int upgradeCnt) {
        string desc = description.GetString();
        foreach(var stat in skillStats) {
            float finalValue = stat.Value + (skillUp.statName == stat.Key ? skillUp.changeAmount*upgradeCnt : 0);
            desc = desc.Replace($"[{stat.Key}]", finalValue.ToString());
        }
        return desc;
    }
}
[System.Serializable]
public class SkillUpgradeInfo {
    public string statName;
    public int changeAmount; // Giá trị được cộng vào giá trị gốc, chứ không phải giá trị đè lên
}
