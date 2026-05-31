public class Farmer : IInitialize {
    private readonly IEntity entity;
    public Farmer(IEntity entity) {
        this.entity = entity;
    }
    public void Initialize() {
        entity.GetEffectable().ApplyEffect(new BountifulHarvest(entity));
    }
}

public class BountifulHarvest : Effect, IOnAssistOrKill {
    private float healPercent = 20;
    public BountifulHarvest(IEntity owner) : base() {

    }
    public void NotifyAssistOrKill() {
        owner.Heal(healPercent/100*(owner.Stats[ST.MaxHealth] - owner.Stats[ST.Health]));
    }
}
