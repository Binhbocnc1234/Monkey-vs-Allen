using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class DeckUI : Singleton<DeckUI> {
    [ReadOnly] public string openedDeckName;
    public TMP_InputField deckName;
    public CardUIManager deckContainer, ownedCardContainer;
    public event Action OnDeckNameDeselected;
    void Start(){
        deckContainer.SetReferencedList(PlayerData.GetDeckByName(openedDeckName));
        ownedCardContainer.SetReferencedList(PlayerData.ownedCards);
    }
    public void OnRenameButtonClicked(){
        deckName.ActivateInputField();
    }
    public void AddCardToDeck(CardSO cardSO){
        PlayerData.AddCardToDeck(openedDeckName, cardSO);
    }
    public void ReturnToLobby(){
        SceneManager.LoadScene("Lobby");
    }
    public void HandleDeselect(){
        OnDeckNameDeselected?.Invoke();
    }

}
