public class Stun : Effect, IOnApply, IOnDestroy {
    public Stun(float duration) : base(duration) {

    }
    public void OnApply() {
        owner.ReturnToIdleBehaviour();
        owner.TogglePause(true);
    }
    public void OnDestroy() {
        owner.TogglePause(false);
    }
}