public class Hypnotized : Effect, IOnDestroy {
    public Hypnotized(int duration) : base(duration) {
        owner.team = owner.team == Team.Player ? Team.Enemy : Team.Player;
    }
    public void OnDestroy() {
        owner.team = owner.team == Team.Player ? Team.Enemy : Team.Player;
    }
}
