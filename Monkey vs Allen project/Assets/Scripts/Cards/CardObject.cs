using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Có nhiều CardObject cùng xuất hiện khi màn chơi rơi ra nhiều CardObject
public class CardObject : MonoBehaviour
{
    public static float fallingSpeed = 5f;
    public int landingLane;
    public float finalY;
    public MonkeyCardSO so;
    public Transform model;
    public SpriteRenderer rarityFrame, rarityBackground, cardImage;
    private MonkeyCardSO card;

    void Update(){
        if (Mathf.Abs(finalY - transform.position.y) >= 0.2f){
            transform.Translate(new Vector2(0, -fallingSpeed * Time.deltaTime));
        }
    }
    void OnMouseDown(){
        Debug.Log("Card clicked!");
        BattleInfo.playerHand.Add(card);
        Destroy(this.gameObject);
    }
    public void Initialize(MonkeyCardSO card, int landingLane){
        so = card;
        this.landingLane = landingLane;
        this.finalY = landingLane * BattleInfo.CELL_SIZE;
        this.card = card;
        CardFrameSO cardFrameSO = CardFrameSO.GetObjectByRarity(card.cardRarity);
        this.rarityFrame.sprite = cardFrameSO.frame;
        this.rarityBackground.sprite = cardFrameSO.background;
    }

}

