using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[CreateAssetMenu(fileName = "NewEntitySO", menuName = "ScriptableObject/Entity")]
public class EntitySO : MySO {
    public GameClass gameClass = GameClass.None;
    public List<Tribe> tribes = new();
    public int health = 10;
    public bool canAttack = true;
    [ShowIf("canAttack")] public int attackRange = 1; //how many cells ahead an entity can control?
    [ShowIf("canAttack")] public int damage = 1;
    [ShowIf("canAttack")] public float attackSpeed = 1f; //hits per second
    public float moveSpeed = 1;
    [SerializeField] private List<EntityStat> otherStats = new();
    public List<EffectType> traits = new();
    public GameObject prefab;
    [SubclassSelector, SerializeReference] public List<IBehaviour> behaviourTemplates = new();
    public List<SkillSO> unlockedSkillInFirstLevel = new();
    [SubclassSelector, SerializeReference] private List<Upgrade> upgrades = new();
    [Header("Obsolete Upgrade System")]
    public bool usingOldUpgradeSystem = true;
    public StatUpgrade level_2;
    public SkillSO unlockedSkillInLevel3;
    public StatUpgrade level_4;
    public SkillSO upgradedSkillAtLv5;
    public override List<string> GetCompletionMessage() {
        List<string> messages = new();
        if(prefab == null) { messages.Add("Missing prefab"); }
        if(gameClass == GameClass.None) { messages.Add("gameClass is None"); }
        if(tribes == null || tribes.Count == 0) { messages.Add("tribes is empty"); }
        return messages;
    }
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
        if (otherStats != null) {
            foreach(EntityStat stat in otherStats) {
                total.Add(stat.GetStatType(), stat.amount);
            }
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
    void OnValidate() {
        if(prefab != null && prefab.GetComponent<Model>() == null) {
            Debug.LogError("[EntitySO] Prefab root doesn't have Model component");
            prefab = null;
        }
        if(health < 0) { health = 0; }
        if(damage < 0) { damage = 0; }
        if(attackSpeed < 0) { attackSpeed = 0; }
        if(attackRange < 1) { attackRange = 1; }
        if(moveSpeed < 0) { moveSpeed = 0; }
    }
    
}


// Khoong dung enum cho Stat nua vi no qua dai va kho nhin o inspector
// Save Effect vaf Behaviour su dung polymophism cua NewtonsoftJson
// var settings = new JsonSerializerSettings
// {
//     TypeNameHandling = TypeNameHandling.Auto // or TypeNameHandling.Objects
// };