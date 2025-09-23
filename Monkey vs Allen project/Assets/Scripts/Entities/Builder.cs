using UnityEngine;

public class Builder : IBehaviour{
    protected override void Awake() {
        base.Awake();
        dangerPoint = 15;
    }
}