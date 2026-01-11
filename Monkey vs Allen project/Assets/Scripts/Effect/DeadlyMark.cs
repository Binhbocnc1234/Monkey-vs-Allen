public class DeadlyMark : IEffect, IStackable {
    public DeadlyMark(IEntity owner) : base(owner, -1, 1) {
        isDebuff = true;
    }
    public void Stack(int amount) {
        strength += amount;
        if (strength == 5) {
            owner.Die();
        }
    }
}