using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "NewEntitySO", menuName = "ScriptableObject/Entity")]
public class EntitySO : MySO {
    public GameClass gameClass;
    public List<Tribe> tribes;
    public int health = 10;
    public bool canAttack = true;
    [ShowIf("canAttack")] public int attackRange = 1; //how many cells ahead an entity can control?
    [ShowIf("canAttack")] public int damage = 1;
    [ShowIf("canAttack")] public float attackSpeed = 1f; //hits per second
    public float moveSpeed = 1;
    [SerializeField] private List<EntityStat> otherStats;
    public List<EffectType> traits;
    public GameObject prefab;
    public List<SkillSO> unlockedSkillInFirstLevel;
    [SubclassSelector, SerializeReference] private List<Upgrade> upgrades;
    [Header("Obsolete Upgrade System")]
    public bool usingOldUpgradeSystem = true;
    public StatUpgrade level_2;
    public SkillSO unlockedSkillInLevel3;
    public StatUpgrade level_4;
    public SkillSO upgradedSkillAtLv5;
    public bool IsContainTribes(List<Tribe> requiredTribes) {
        foreach(Tribe tribe in requiredTribes) {
            if(tribes.Contains(tribe) == false) {
                return false;
            }
        }
        return true;
    }
    public bool AnyTribes(List<Tribe> lst) {
        foreach(Tribe tribe in lst) {
            if(tribes.Contains(tribe)) {
                return true;
            }
        }
        return false;
    }
    public UDictionary<ST, float> GetEntityStats(int level) {
        UDictionary<ST, float> total = new();
        foreach(EntityStat stat in otherStats) {
            total.Add(stat.GetStatType(), stat.amount);
        }
        total.Add(ST.MaxHealth, health);
        if(canAttack) {
            total.Add(ST.Strength, damage);
            total.Add(ST.AttackSpeed, attackSpeed);
            total.Add(ST.Range, attackRange);
        }
        if(moveSpeed > 0) {
            total.Add(ST.MoveSpeed, moveSpeed);
        }
        if(level >= 2) {
            total[level_2.stat] += level_2.amount;
        }
        if(level >= 4) {
            total[level_4.stat] += level_4.amount;
        }
        return total;
    }
    public List<Upgrade> GetUpgrades() {
        if(usingOldUpgradeSystem) {
            List<Upgrade> oldUpgrades = new List<Upgrade>() {
                level_2,
                new UnlockSkill{skillSO = unlockedSkillInLevel3},
                level_4,
                new SkillUpgrade{skillSO = upgradedSkillAtLv5}
            };
            return oldUpgrades;
        }
        else {
            return upgrades;
        }
    }
    public SkillSO GetSkillSOByName(string name) {
        foreach(SkillSO so in unlockedSkillInFirstLevel) {
            if(so.skillName == name) {
                return so;
            }
        }
        Debug.LogError($"[EntitySO] Cannot find SkillSO with name :{name}");
        return null;
    }
    public static EntitySO GetSOByName(string name) {
        foreach(EntitySO so in SORegistry.Get<EntitySO>()) {
            if(so.name == name) {
                return so;
            }
        }
        Debug.LogError($"[EntitySO] Cannot find EntitySO with name : {name}");
        return null;
    }
}


// Khoong dung enum cho Stat nua vi no qua dai va kho nhin o inspector
// Save Effect vaf Behaviour su dung polymophism cua NewtonsoftJson
// var settings = new JsonSerializerSettings
// {
//     TypeNameHandling = TypeNameHandling.Auto // or TypeNameHandling.Objects
// };