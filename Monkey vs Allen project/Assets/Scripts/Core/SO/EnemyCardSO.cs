using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewEnemyCardSO", menuName = "ScriptableObject/EnemyCardSO")]
public class EnemyCardSO : CardSO {

}

#if UNITY_EDITOR

[CustomEditor(typeof(EnemyCardSO), true)]
public class EnemyCardSOEditor : Editor {
    Texture2D preview;

    void OnEnable() {
        if(preview == null)
            preview = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/EnemyCardSO Icon.png");
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