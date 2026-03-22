using UnityEngine;

public abstract class IEntityRegistry : Singleton<IEntityRegistry> {
    public abstract IEntity CreateEntity(EntitySetting setting);
    public abstract IEntity CreateEntity(EntitySO so, float x, int lane, Team team, int level = 1);
    public abstract IEntity CreateEntity(EntitySO so, Vector2Int gridPos, Team team, int level = 1);
    public abstract IEntity CreateEntity(EntitySO so, int lane, Team team, int level = 1);
    public abstract IEntity[] GetEntitiesByLane(int lane, bool includeUntargetable = false);
    public abstract IEntity[] GetEntities();
    public abstract int GetTargetCount(Team team);
}
