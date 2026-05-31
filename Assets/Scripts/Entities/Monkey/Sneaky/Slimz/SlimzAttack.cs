// [Wrapper] Phase 4: wrapper handles Instantiate and prefab references
public class SlimzAttack : RangedAttack {
    public bool isNextTimeIsPoke = false;
    protected override void WhenAttackReady() {
        if(isNextTimeIsPoke) {
            // [Wrapper] Phase 4: wrapper creates Poke bullet
            isNextTimeIsPoke = false;
        }
        else {
            base.WhenAttackReady();
        }
    }
    public void NextAttackBecomePoke() {
        isNextTimeIsPoke = true;
    }
}