// [Wrapper] Phase 4: OnApply should delegate to wrapper for Instantiate/transform
public class PowerOfDollar : Skill, IOnApply {
    public override bool CanActive() {
        return false;
    }
    public override void OnApply() {
        base.OnApply();
    }  
}