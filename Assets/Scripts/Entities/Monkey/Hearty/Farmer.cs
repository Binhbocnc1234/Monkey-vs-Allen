public class Farmer : EntityBehaviour, IInitialize {
    public void Initialize() {
        e.GetEffectable().ApplyEffect(new BountifulHarvest(e));
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