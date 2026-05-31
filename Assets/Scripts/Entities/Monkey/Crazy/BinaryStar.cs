public class BinaryStar : IInitialize {
    private readonly IEntity entity;
    public BinaryStar(IEntity entity) {
        this.entity = entity;
    }
    public void Initialize() {
        entity.GetEffectable().ApplyEffect(new DoubleDamageEffect());
    }
}
