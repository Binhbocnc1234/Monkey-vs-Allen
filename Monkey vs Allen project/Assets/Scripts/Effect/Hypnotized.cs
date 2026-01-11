public class Hypnotized : IEffect, IOnDestroy {
    public Hypnotized(IEntity owner, int duration) : base(owner, duration) {
        owner.team = owner.team == Team.Player ? Team.Enemy : Team.Player;
    }
    public void OnDestroy() {
        owner.team = owner.team == Team.Player ? Team.Enemy : Team.Player;
    }
}
