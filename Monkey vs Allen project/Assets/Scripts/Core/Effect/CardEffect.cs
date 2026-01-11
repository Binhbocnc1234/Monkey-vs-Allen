public abstract class CardEffect : IUpdatePerFrame, IDestroyable {
    public bool isDebuff{ get; protected set; }
    public readonly int duration;
    public IBattleCard owner;
    protected Timer lifeTimer;
    private bool isDead = false;
    public CardEffect(IBattleCard owner, int duration = -1) {
        this.owner = owner;
        this.duration = duration;
        isDebuff = true;
        if(duration != -1) {
            lifeTimer = new Timer(duration, false);
        }
    }
    public virtual void Update() {
        if(lifeTimer.Count()) {
            DestroyThis();
        }
    }
    public void DestroyThis() {
        isDead = true;
    }
    public bool IsDead() => isDead;
    public void ResetDuration() {
        if (duration != -1) {
            lifeTimer.Reset();
        }
    }
}