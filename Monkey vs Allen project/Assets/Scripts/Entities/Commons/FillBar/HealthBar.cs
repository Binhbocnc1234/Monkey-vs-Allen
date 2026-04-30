using UnityEngine;
public class HealthBar : FillBar {
    private IEntity e;
    public void BindEntity(IEntity e) {
        this.e = e;
        e.OnEntityDeath += () => Destroy(this.gameObject);
        
    }
    void Update() {
        SetValue(e.GetHealthPercentage());
        this.transform.position = new Vector2(e.model.transform.position.x,
            e.model.boxCollider.bounds.max.y + 0.25f);
    }
}