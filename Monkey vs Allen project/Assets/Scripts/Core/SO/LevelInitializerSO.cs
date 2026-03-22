using UnityEngine;

public abstract class LevelInitializerSO : ScriptableObject {
    public abstract void Execute(LevelSO so);
}