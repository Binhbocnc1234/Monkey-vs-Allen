using UnityEngine;


public class CombatComponent : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 1f;
    public float attackSpeed = 1f;
    public Timer timer;
    public bool isAttacking = false;
    void Start(){
        timer = new Timer(attackSpeed, true);
    }
    void Update(){
        if (isAttacking && timer.Count()){
            Attack();
        }
    }
    protected virtual void Attack() {
        //should be overwrite by child class;
    }

}
