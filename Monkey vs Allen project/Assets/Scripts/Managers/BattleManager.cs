using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum CardSystemType {
    PvZ2,
    PvZHeroes
}
public class Binhfafaf{
    public string name;
    public BattleManager battleManager;
    public Target target;
    public string cc;
}
public class BattleManager : Singleton<BattleManager> {
    public CardSystemType cardSystemType = CardSystemType.PvZ2;
    public Entity targetMonkeyPrefab, targetAllenPrefab;
    public UIManager uiManager;

    public LevelSO levelInfo;
    private CardUIManager chosenCards, ownedCards;
    private IGrid grid;
    private int targetMonkeysLeft = 5, targetEnemiesLeft = 5;
    [ContextMenu("Initialize everythings")]
    void Start() {

        chosenCards = ChosenCardManager.Instance;
        ownedCards = OwnedCardManager.Instance;
        uiManager = UIManager.Instance;

        CardSO.ResetContainer();
        CardFrameSO.ResetContainer();
        LevelSO.ResetContainer(); levelInfo = LevelSO.GetLevelSO(BattleInfo.place, BattleInfo.level);
        HideAndShowUI.ResetContainer();
        EntityContainer.Instance.ClearEntity();
        //BattleInfo's place and level is initialized by Level choosing scene
        
        BattleInfo.Initialize(levelInfo);
        grid = GridSystem.Instance;
        grid.Clear();
        grid.Initialize();
        Prize.Instance.gameObject.SetActive(false);
        PlayerData.Load();

        if (cardSystemType == CardSystemType.PvZ2){
            //OwnedCard initialization
            ownedCards.SetReferencedList(PlayerData.ownedCards);
            foreach(CardUI cardUI in ownedCards.cardUIs){
                cardUI.OnClickEvent += AddCardToOwnedCard;
            }
            //ChoosenCard initialization
            chosenCards.SetReferencedList(BattleInfo.choosenCardSOs);
            foreach(CardUI cardUI in chosenCards.cardUIs){
                cardUI.OnClickEvent += RemoveCardFromOwnedCard;
            }
    
            
        }
        else{
            this.gameObject.AddComponent(typeof(CardSpawner));
        }
        ChangeState(GameState.ChoosingCard);
    }
    void Update() {
        if (BattleInfo.state == GameState.Fighting){
            foreach(Card card in BattleInfo.choosenCards){
                card.cooldownTimer.Count(false);
            }
        }
    }
    void AddCardToOwnedCard(CardUI card){
        BattleInfo.choosenCardSOs.Add(card.so);
    }
    void RemoveCardFromOwnedCard(CardUI card){
        BattleInfo.choosenCardSOs.Remove(card.so);
    }
    public void ChangeState(GameState state){
        BattleInfo.state = state;
        if (state == GameState.ChoosingCard){
            StartCoroutine(InitChoosingCard());
        }
        else if (state == GameState.Prepare){
            StartCoroutine(Prepare());
        }
        else if (state == GameState.Fighting){
            this.gameObject.GetComponent<EnemyManager>().enabled = true;
            EnemyManager.Instance.ClearDemoEnemy();
            foreach(CardSO so in BattleInfo.choosenCardSOs){ 
                Card newCard = new Card();
                newCard.ApplyCardSO(so);
                BattleInfo.choosenCards.Add(newCard);
                chosenCards.FindCardUIBySO(so).card = newCard;
            }
            GridCamera.Instance.canDraging = true;
        }
        else if (state == GameState.Victory){
            for(int i = 0; i < grid.height; ++i){
                List<Entity> entities = EContainer.GetEntitiesByLane(i);
                foreach(Entity e in entities){
                    e.SetEntityState(EntityState.Idle);
                }
            }
            uiManager.gameObject.SetActive(false);
            
        }
        else if(state == GameState.GameOver){
            StartCoroutine(GameOver());
        }
    }
    public void ChangeToPrepare(){
        ChangeState(GameState.Prepare);
    }
    IEnumerator InitChoosingCard(){
        //Create target for both sides
        for(int y = 0; y < grid.height; ++y){
            var targetMonkey = grid.GetCell(1, y).PlaceObject(targetMonkeyPrefab.gameObject).GetComponent<Entity>();
            Entity.targetMonkeys.Add(targetMonkey);
            targetMonkey.Initialize();
            targetMonkey.OnEntityDeath += CheckLose;

            var targetEnemy = grid.GetCell(grid.width - 2, y).PlaceObject(targetAllenPrefab.gameObject).GetComponent<Entity>();
            Entity.targetEnemies.Add(targetEnemy);
            targetEnemy.Initialize();
            targetEnemy.OnEntityDeath += CheckWin;
        }
        uiManager.InitChoosingCard();
        HideAndShowUI.HideAllImmediately();
        GridCamera.Instance.canDraging = false;
        yield return new WaitForSeconds(1f);
        GridCamera.Instance.MoveTowardEnemyHouse();
        yield return new WaitWhile(() => GridCamera.Instance.isMoving);
        GetComponent<EnemyManager>().ShowEnemy();
        if (levelInfo.canChooseCard){
            HideAndShowUI.ShowAll();
        }
        else{
            UIManager.Instance.chosenCardsTrans.Show();
            ChangeState(GameState.Prepare);
        }
    }
    IEnumerator Prepare(){
        uiManager.Prepare();
        GridCamera.Instance.MoveTowardPlayerHouse();
        yield return new WaitWhile(() => GridCamera.Instance.isMoving);
        yield return StartCoroutine(PrepareUI.Instance.Act());
        ChangeState(GameState.Fighting);
    }
    void CheckLose(Entity e){
        targetMonkeysLeft--;
        if (targetMonkeysLeft == 0){
            ChangeState(GameState.GameOver);
        }
    }
    void CheckWin(Entity e){
        targetEnemiesLeft--;
        if(targetEnemiesLeft == 0){
            ChangeState(GameState.Victory);
        }
    }
    IEnumerator GameOver(){
        uiManager.gameObject.SetActive(false);
        for(int i = 0; i < grid.height; ++i){
            List<Entity> entities = EContainer.GetEntitiesByLane(i);
            foreach(Entity e in entities){
                if (e.GetComponent<Target>() == null){
                    e.SetEntityState(EntityState.Frozen);
                }
                
            }
        }
        yield return new WaitForSeconds(0.5f);
        GridCamera.Instance.SetTarget(grid.GetCell(2, 2).transform.position, 1);
        yield return new WaitWhile(() => GridCamera.Instance.isMoving);
        StartCoroutine(uiManager.Lose());
    }
    IEnumerator Victory(){
        yield return new WaitWhile(() => Prize.Instance.isAnimating);
        yield return StartCoroutine(uiManager.Flash());
        SceneManager.LoadScene("Prize");
    }
}
