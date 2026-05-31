using System;
public class PokeAppearance : BulletAppearance
{
    private Poke poke;
    public override void Initialize(Bullet bullet) {
        base.Initialize(bullet);
        poke = (Poke)bullet;
    }
    void Update()
    {
        if (poke.nearestTarget != null)
        {
            float targetX = poke.nearestTarget.model.GetPosition().x;
            if (MathF.Abs(targetX - this.transform.position.x) <= IGrid.CELL_SIZE)
            {
                model.animator.Play("Poke Open");
            }
        }
    }
}