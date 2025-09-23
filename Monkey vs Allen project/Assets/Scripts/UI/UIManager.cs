using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//UI assembly contains: bananaUI --> core, CardUI --> CardSO -> core
//UIManager born to solve UI's reference all 
public class UIManager : Singleton<UIManager>
{

    private CardUIManager ownedCards, chosenCards;
    public HideAndShowUI levelUI, letsRockUI;
    public HideAndShowUI ownedCardsTrans, chosenCardsTrans;
    public Scrollbar selectedCardScrollbar;
    public Image flashPanel;
    public GameObject loseText;
    public RectTransform tryAgainUI;

    void Start(){
        ownedCards = OwnedCardManager.Instance;
        chosenCards = ChosenCardManager.Instance;
        loseText.gameObject.SetActive(false);
        tryAgainUI.gameObject.SetActive(false);
    }
    public void InitChoosingCard(){
        BananaUI.Instance.Initialize();
        HideAndShowUI.HideAllImmediately();
        levelUI.ShowCoroutine();
        selectedCardScrollbar.value = 0;
        // chosenCards.SetReferencedList()
        PrepareUI.Instance.gameObject.SetActive(false);
    }
    public void Prepare(){
        levelUI.Hide();
        letsRockUI.Hide();
        StartCoroutine(ownedCardsTrans.HideCoroutine());
        StartCoroutine(chosenCardsTrans.ShowCoroutine());
        foreach(CardUI cardUI in chosenCards.cardUIs){
            cardUI.haveCooldown = true;
        }
        
    }
    public IEnumerator Lose(){
        loseText.transform.localScale = Vector3.zero;
        float t = 0;
        while(true){
            t += Time.deltaTime;
            if (t >= 2){
                tryAgainUI.gameObject.SetActive(true);
                break;
            }
            else{
                yield return null;
            }
        }

    }
    public IEnumerator Flash(){
        Color white = Color.white;
        while(true){
            white.a += Time.deltaTime;
            if (white.a >= 1){
                break;
            }
            else{
                flashPanel.color = white;
                yield return null;
            }
            
        }   
    }
    // public IEnumerator DeFlash(){
    //     Color white = Color.white;
    //     while(true){
    //         white.a -= Time.deltaTime;
    //         if (white.a <= 0){
    //             break;
    //         }
    //         else{
    //             flashPanel.color = white;
    //             yield return null;
    //         }
            
    //     }  
    // }
}
