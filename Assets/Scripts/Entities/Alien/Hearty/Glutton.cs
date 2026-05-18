
public class Glutton : Entity {
    protected override void Awake() {
        
    }
}
public class AbsorbPrey : Effect, IOnAssistOrKill {
    public AbsorbPrey(IEntity owner) : base() {

    }
    public void NotifyAssistOrKill() {
        
    }
}
