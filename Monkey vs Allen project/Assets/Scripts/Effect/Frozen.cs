using UnityEngine;

public class Frozen : Effect, IOnApply, IOnDestroy {
    public Frozen(float duration) : base(duration) {

    }
    public void OnApply() {
        owner.GetAnimator().speed = 0;
        owner.TogglePause(true);
        // foreach(SpriteRenderer renderer in owner.GetSprites()) {
        //     renderer.color = Color.
        // }
    }
    public void OnDestroy() {
        owner.GetAnimator().speed = 1;
        owner.TogglePause(false);
    }
}