using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Những cây sau sẽ có giá tiền tăng thêm 1
public class BananaTree : IBehaviour
{
    public float cooldown = 5f;
    private Timer cooldownTimer;
    protected override void Awake(){
        base.Awake();
        cooldownTimer = new Timer(cooldown);
        dangerPoint = 1;
    }
    protected void Update(){
        if (cooldownTimer.Count()){
            BattleInfo.ChangeBananaCnt(1);
        }
    }
}
