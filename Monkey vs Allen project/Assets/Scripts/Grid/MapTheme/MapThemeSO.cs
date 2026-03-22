using UnityEngine;

[CreateAssetMenu(fileName = "NewMapTheme", menuName = "ScriptableObject/MapTheme")]
public class MapThemeSO : ScriptableObject {
    public Sprite[] dirtTiles;
    public Sprite dirtWithDickTexture;
    public Sprite[] glassTiles;
    public Sprite pool;
} 