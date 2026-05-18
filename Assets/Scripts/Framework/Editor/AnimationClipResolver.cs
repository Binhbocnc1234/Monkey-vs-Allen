using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationClipResolver : MonoBehaviour
{
    public Vector2 delta;
    public AnimationClip clip;

    [ContextMenu("Adjust keyframes' position")]
    private bool AdjustClip()
    {
        bool changed = false;
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);

        Undo.RecordObject(clip, "Adjust animation position keyframes");

        foreach (EditorCurveBinding binding in curveBindings)
        {
            if (!TryGetOffset(binding.propertyName, out float offset))
            {
                continue;
            }

            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve == null || curve.length == 0)
            {
                continue;
            }

            Keyframe[] keys = curve.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].value += offset;
            }

            curve.keys = keys;
            AnimationUtility.SetEditorCurve(clip, binding, curve);
            changed = true;
        }

        return changed;
    }

    private bool TryGetOffset(string propertyName, out float offset)
    {
        switch (propertyName)
        {
            case "m_LocalPosition.x":
                offset = delta.x;
                return true;
            case "m_LocalPosition.y":
                offset = delta.y;
                return true;
            default:
                offset = 0f;
                return false;
        }
    }
}
