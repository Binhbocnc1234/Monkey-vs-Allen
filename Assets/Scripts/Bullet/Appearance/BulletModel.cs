using UnityEngine;

public class BulletModel : Model {
    protected override void Awake() {
        base.Awake();
        sortingGroup.sortingLayerName = "Projectile";
    }
    public void Initialize(Bullet bullet) {
        foreach(BulletAppearance appearance in GetComponents<BulletAppearance>()) {
            appearance.Initialize(bullet);
        }
    }
}
