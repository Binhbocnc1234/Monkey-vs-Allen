using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class IEntity {
    public Action<IBehaviour> OnBehaviorActive;
    public EntitySO so;
    public int lane;
    public bool isTargetable = true;
    public float width = 1, height = 1;
    public List<Tribe> tribes { get; protected set; }
    public string soId;
    public int level;
    protected Team _team;
    public Action<Team> OnTeamSwapped;
    public abstract Team team {
        get; set;
    }
    public bool isDead;
    public float actionElapsedTime;
    public Vector2 gridPos;
    public UDictionary<ST, float> Stats = new();
    public Action OnEntityDeath;
    public Action<float> OnHealthChanged;
    public Model model;
    public IBehaviour[] behaviours;
    public bool isSimulated{ get; protected set; }
    public abstract float this[ST st] { get; set; }
    public abstract IEffectable GetEffectable();
    /// <summary>
    /// return Health/MaxHealth, the return value inside [0f, 1f]
    /// </summary>
    public abstract float GetHealthPercentage();
    public abstract bool IsDead();
    /// <summary>
    /// Make the entity die immediately eventhough its Health > 0
    /// </summary>
    public abstract void Die();
    public abstract void TakeDamage(DamageContext damageContext);
    public abstract void Heal(float amount);
    public abstract float DistanceTo(IEntity other);
    public abstract float DistanceToBase();
    public abstract float DistanceToOpponentBase();
    public abstract void TogglePause(bool toggle);
    public abstract void BecomeInActive();
    /// <summary>
    /// Ví dụ, với BasicMonkey thì Danger là 30, Defend cũng xấp xỉ 30
    /// Danger là 30 có nghĩa là Entity này có thể gây 30 sát thương trong mỗi 1.35 giây
    /// Defend là 30 có nghĩa là nó có thể sống sót được trong 20/3, hoặc 6.6 giây 
    /// khi đấu với đội hình có tổng Danger là 30
    /// </summary>
    public abstract float GetAssessPoint(APType type);
    public abstract EntitySO GetSO();
    /// <summary>
    /// Trả về tốc độ theo đơn vị Cell unit, được sử dụng để di chuyển Entity
    /// </summary>
    /// <returns></returns>
    public abstract float GetRealMoveSpeed();
    public abstract void ReturnToIdleBehaviour();
    public abstract IBehaviour GetActiveBehaviour();
    public abstract float GetSkillStat(SkillSO so, string name);
    public abstract T GetBehaviour<T>() where T : class;
    public abstract IEnumerable<T> GetBehaviours<T>() where T : class;
    public abstract void SetBehaviours(IEnumerable<IBehaviour> behaviourList);
}
