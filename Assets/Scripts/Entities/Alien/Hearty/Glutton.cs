
// public class Glutton : Entity {
//     // [Wrapper] If Unity-specific initialization is needed, add it in EntityWrapper subclass
// }
public class AbsorbPrey : Effect, IOnAssistOrKill {
    public AbsorbPrey(IEntity owner) : base() {

    }
    public void NotifyAssistOrKill() {
        
    }
}
