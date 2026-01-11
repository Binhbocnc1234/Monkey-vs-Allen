using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum CardAction{
    Summon
}
public enum CardCondition{ //Nếu vị trí đặt thẻ bài là hợp lệ thì sẽ hiển thị ảnh model mờ của card tại vị trí đó
    Summon,
    Buff,
}

[CreateAssetMenu(fileName = "NewMonkeyCardSO", menuName = "ScriptableObject/MonkeyCardSO")]
public class MonkeyCardSO : CardSO{
    
}
#if UNITY_EDITOR

[CustomEditor(typeof(MonkeyCardSO), true)]
public class MonkeyCardSOEditor : Editor {
    Texture2D preview;

    void OnEnable() {
        if(preview == null)
            preview = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/MonkeyCardSO Icon.png");
        if (preview == null) {
            Debug.LogError("Cannot find .png file");
        }
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
        if(preview == null)
            return base.RenderStaticPreview(assetPath, subAssets, width, height);

        Texture2D tex = new Texture2D(width, height);
        EditorUtility.CopySerialized(preview, tex);
        return tex;
    }
}

#endif
