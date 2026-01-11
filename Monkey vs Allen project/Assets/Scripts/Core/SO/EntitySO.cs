using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Team {
    Player,
    Enemy,
    Default
}


[CreateAssetMenu(fileName = "NewEntitySO", menuName = "ScriptableObject/Entity")]
public class EntitySO : ScriptableObject
{
    [Header("Metadatas")]
    public List<Tribe> tribes;
    public List<TraitType> traits;
    [Header("Statistics")]
    public int health = 10;
    public bool canAttack = true; 
    [ShowIf("canAttack")] public int attackRange = 1; //how many cells ahead an entity can control?
    [ShowIf("canAttack")] public int damage = 1;
    [ShowIf("canAttack")] public float attackSpeed = 1f; //hits per second
    public float moveSpeed = 1;
    public int width = 1, height = 1;
    [Header("Other")]
    public GameObject prefab;
    public bool IsContainTribes(List<Tribe> requiredTribes) {
        foreach(Tribe tribe in tribes) {
            if(requiredTribes.Contains(tribe) == false) {
                return false;
            }
        }
        return true;
    }
    // public static EntitySO GetEntitySO(string name) {
    //     foreach(EntitySO so in SORegistry.Get<EntitySO>()) {
            
    //     }
    // }
}
