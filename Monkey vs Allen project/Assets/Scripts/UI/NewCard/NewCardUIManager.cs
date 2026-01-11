using UnityEngine;
using UnityEngine.SceneManagement;

public class NewCardUIManager : Singleton<NewCardUIManager>{
    public CardSO newCardSO;
    public CardUIAppearance cardUI;
    private CardDescriptionUI description;
    void Start() {
        if (CustomSceneManager.newCardSO != null) {
            Initialize(CustomSceneManager.newCardSO);
        }
        else {
            Initialize(newCardSO);
        }
    }
    public void Initialize(CardSO so) {
        description = CardDescriptionUI.Ins;
        newCardSO = so;
        cardUI.Initialize(so);
    }
    public void ReturnToLobby() {
        CustomSceneManager.ToLobby();
    }
}