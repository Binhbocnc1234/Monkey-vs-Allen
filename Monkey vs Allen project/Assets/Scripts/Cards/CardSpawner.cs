using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardSpawner : Singleton<MonoBehaviour>
{
    [SerializeField] private static CardObject cardObjPrefab;
    public float delayBetweenTwoCards = 7f;
    public CardObject cardObjectPrefab;
    private Timer delayTimer;

    void Initialize()
    {
        delayTimer = new Timer(delayBetweenTwoCards);
        BattleInfo.choosenCardSOs = CardSO.container;
    }

    // Update is called once per frame
    void Update()
    {
        if (delayTimer.Count()){
            SpawnCard();
        }
    }
    void SpawnCard(){
        CardSO randomCard = BattleInfo.choosenCardSOs[Random.Range(0, BattleInfo.choosenCardSOs.Count)];
        IGrid grid = IGrid.Instance;
        int landingLane = Random.Range(0, grid.height - 1);
        var randomPos = grid.GridToWorldPosition(Random.Range(0, grid.width-1), landingLane+5);
        Instantiate(cardObjectPrefab, randomPos, Quaternion.identity, this.transform).Initialize(randomCard, landingLane);
    }
}

