public class Farmer : Entity {
    protected override void Awake() {
        base.Awake();
        GetEffectable().ApplyEffect(new BountifulHarvest(this));
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