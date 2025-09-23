using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Team
{
    Player,
    Enemy
}

public enum EntityType{
    None,
    TargetMonkey,
    TargetAllen,
    Tower,
    Monkey,
    Enemy,
    
}

[CreateAssetMenu(fileName = "NewEntitySO", menuName = "ScriptableObject/Entity")]
public class EntitySO : ScriptableObject
{
    [Header("Metadatas")]
    public Team team;
    public EntityType entityType;
    [Header("ToggleAction")]
    public bool canAttack = true;
    public bool canMove = true;
    [Header("Statistics")]
    public int health = 10;
    [ShowIf("canAttack")] public int attackRange = 1; //how many cells ahead an entity can control?
    [ShowIf("canAttack")] public int damage = 1;
    [ShowIf("canAttack")] public float attackSpeed = 1f; //hits per second
    [ShowIf("canMove")] public float moveSpeed = 1;
    public int width = 1, height = 1;
    [Header("Other")]
    public GameObject prefab;

}
