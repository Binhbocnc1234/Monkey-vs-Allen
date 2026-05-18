public class DeadlyMark : Effect, IStackable {
    public DeadlyMark() : base(-1, 1) {
        isDebuff = true;
    }
    public void Stack(int amount) {
        strength += amount;
        if (strength == 5) {
            owner.Die();
        }
    }
}