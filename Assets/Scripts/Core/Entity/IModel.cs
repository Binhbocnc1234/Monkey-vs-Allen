
using UnityEngine;

public interface IModel
{
    public Vector2 GetPosition();
    public void SetPosition(Vector2 worldPos);
    public Bounds GetBound();
}