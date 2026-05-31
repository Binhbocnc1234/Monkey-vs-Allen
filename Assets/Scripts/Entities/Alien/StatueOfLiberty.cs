public class StatueOfLiberty : IInitialize {
    private readonly IEntity entity;
    private DoubleDamageEffect globalEffect;
    public StatueOfLiberty(IEntity entity) {
        this.entity = entity;
    }
    public void Initialize() {
        // [Wrapper] hook global effect visuals in wrapper if needed
        // globalEffect = new DoubleDamageEffect();
        // GlobalEffectManager.Ins.AddEffect(globalEffect);
        // entity.OnEntityDeath += () => GlobalEffectManager.Ins.RemoveEffect(globalEffect);
    }
}
