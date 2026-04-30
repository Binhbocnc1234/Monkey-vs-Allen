public class Hypnotized : Effect, IOnDestroy {
    public Hypnotized(int duration) : base(duration) {
        owner.team = owner.team == Team.Left ? Team.Right : Team.Left;
    }
    public void OnDestroy() {
        owner.team = owner.team == Team.Left ? Team.Right : Team.Left;
    }
}
