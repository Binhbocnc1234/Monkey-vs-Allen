using UnityEngine;

public class GeneralPurposeContainer : Singleton<GeneralPurposeContainer> {
    public T CreateInstance<T>(T prefab, Vector2 position) where T : Object{
        return Instantiate(prefab, position, Quaternion.identity, this.transform);
    }
}