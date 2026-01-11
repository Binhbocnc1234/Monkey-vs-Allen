using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.Linq;

public class DeckUI : Singleton<DeckUI> {
    public enum State {
        DeckPreview,
        DeckEditor,
    }
    [ReadOnly] public State state;
    public Deck openedDeck;
    public TMP_InputField deckName;
    [SerializeField] private RectTransform header_1, header_2;
    public CardUIManager<CardUI> deckManager, ownedCardManager;
    [SerializeField] private RectTransform deckPreviewsContainer;
    [SerializeField] private DeckDisplay deckDisplayPrefab;
    public event Action OnDeckNameDeselected;
    void Start() {
        ownedCardManager.SetReferencedList(PlayerData.GetOwnedCard().ToList<CardSO>());
        SetState(State.DeckPreview);
        foreach(Deck deck in PlayerData.decks) {
            DeckDisplay newDisplay = Instantiate(deckDisplayPrefab, deckPreviewsContainer);
            newDisplay.Initialize(deck);
            newDisplay.button.onClick.AddListener(() => OnDeckDisplayClicked(deck));
            newDisplay.gameObject.SetActive(true);
        }
        deckName.onValueChanged.AddListener((string t) => { openedDeck.deckName = t; });
    }
    void OnDeckDisplayClicked(Deck chosenDeck) {
        SetState(State.DeckEditor);
        openedDeck = chosenDeck;
        deckManager.SetReferencedList(chosenDeck.cardList.ToList<CardSO>());
    }
    void SetState(State state) {
        bool value = state == State.DeckPreview;
        deckManager.container.gameObject.SetActive(!value);
        header_1.gameObject.SetActive(value);
        header_2.gameObject.SetActive(!value);
        deckPreviewsContainer.gameObject.SetActive(value);
    }
    public void OnRenameButtonClicked(){
        deckName.ActivateInputField();
    }

    public void ReturnToLobby(){
        if (state == State.DeckPreview) {
            SceneManager.LoadScene("Lobby");
        }
        else {
            SetState(State.DeckPreview);
        }
    }
    public void HandleDeselect(){
        OnDeckNameDeselected?.Invoke();
    }

}
