using UnityEngine;

public abstract class IEntityRegistry : Singleton<IEntityRegistry> {
    public abstract void CreateBuilder(EntitySetting towerSetting);
    public abstract IEntity CreateEntity(EntitySetting setting);
    public abstract IEntity[] GetEntitiesByLane(int lane, bool includeUntargetable = false);
    public abstract IEntity[] GetEntities();
    public abstract int GetTargetCount(Team team);
}
