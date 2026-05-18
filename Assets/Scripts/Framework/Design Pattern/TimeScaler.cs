using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour {
    private Dictionary<string, float> modifiers = new();

    public void Set(string key, float value) {
        modifiers[key] = value;
        Recalculate();
    }

    public void Remove(string key) {
        modifiers.Remove(key);
        Recalculate();
    }

    private void Recalculate() {
        float final = 1f;
        foreach(var v in modifiers.Values)
            final *= v;

        Time.timeScale = final;
    }
}