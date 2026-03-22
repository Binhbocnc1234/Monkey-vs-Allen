using UnityEngine;

[System.Serializable]
public class LocalizedDescription
{
    [TextArea(3, 8)]
    public string English;

    [TextArea(3, 8)]
    public string Vietnamese;

    [TextArea(3, 8)]
    public string Poturgese;

    public string Get(Language language) {
        switch(language) {
            case Language.English: return English;
            case Language.Vietnamese: return Vietnamese;
            case Language.Portuguese: return Poturgese;
            default: return "empty";
        }
    }
    public string GetString() => Get(PlayerData.UserLanguage);
}