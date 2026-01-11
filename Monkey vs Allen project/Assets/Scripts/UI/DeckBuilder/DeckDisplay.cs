using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplay : MonoBehaviour {
    public Deck deck;
    [SerializeField] private CardUI cardUIPrefab;
    [SerializeField] private RectTransform container;
    public Button button;
    [SerializeField] private TMP_Text deckNameText;
    public void Initialize(Deck deck) {
        this.deck = deck;
        deckNameText.text = deck.deckName;
        foreach(MonkeyCardSO so in deck.cardList) {
            CardUI newCardUI = Instantiate(cardUIPrefab, container);
            newCardUI.ApplyCardSO(so);
            newCardUI.gameObject.SetActive(true);
        }
    }
}
