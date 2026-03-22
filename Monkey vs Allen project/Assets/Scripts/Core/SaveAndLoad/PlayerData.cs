using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections.ObjectModel;
using System.Linq;
using System;

[System.Serializable]
internal class PlayerDataRaw {
    public List<CardData> monkeyCards = new List<CardData>(), enemyCards = new List<CardData>();
    /* Cấu trúc của rawDecks chứa:
        - Phần tử bắt đầu với kí tự '#' là tên deck
        - Những phần tử sau tên deck là id của các lá bài*/
    public List<string> rawDecks = new List<string>();
    public string playerName = "User2175162#";
    public bool[] campainProgress;
    public List<string> completedLevels;
}
[System.Serializable]
public class Deck {
    public string deckName;
    public List<MonkeyCardSO> cardList = new();
    public Deck(string name) {
        deckName = name;
    }
}
[System.Serializable]
public class CardData {
    public string id{ get; private set; }
    public int level{ get; private set; } //0 is not owned
    public bool discovered;
    public int shards;
    public CardData(string id) {
        this.id = id;
        level = 0;
        shards = 0;
        discovered = false;
    }
    public CardData(CardData other) {
        this.id = other.id;
        this.level = other.level;
        this.shards = other.shards;
    }
    public bool CanUpgrade() {
        return level < CardSO.maxLevel && shards >= CardSO.UPGRADE_THRESHOLD[level];
    }
    public void Upgrade() {
        shards -= CardSO.UPGRADE_THRESHOLD[level];
        level++;
    }
    // private enum UnlockMessage{
    //     Success,
    //     AlreadyUnlock
    // }
    public void Unlock() {
        if(level == 0) {
            level = 1;
        }
        discovered = true;
    }
}
public static class PlayerData{
    static string path = Path.Combine(Application.persistentDataPath, "playerdata.json");
    public static string playerName = "User2175162#";
    private static bool[] campainProgress;
    // private static HashSet<string> completedLevels;
    private static List<CardData> monkeyCards, enemyCards;
    // public static ReadOnlyCollection<string> CompletedLevels => completedLevels.ToList().AsReadOnly();
    // public static ReadOnlyCollection<string> VisibleLevels => visibleLevels.ToList().AsReadOnly();
    public static ReadOnlyCollection<CardData> MonkeyCards => monkeyCards.AsReadOnly();
    public static ReadOnlyCollection<CardData> EnemyCards => enemyCards.AsReadOnly();
    private static int ownedCoins = 0;
    public static Language UserLanguage = Language.English;
    public static List<Deck> decks { get; private set; }
    public static void Initialize() {
        decks = new();
        monkeyCards = new();
        enemyCards = new();
        campainProgress = new bool[100];
        // visibleLevels = new() {
        //     SORegistry.Get<LevelSO>().
        //     First(e => e.place == Place.Garden && e.number == 1).id
        // }; 
        foreach(MonkeyCardSO so in SORegistry.Get<MonkeyCardSO>()) {
            monkeyCards.Add(new CardData(so.id));
        }
        foreach(EnemyCardSO so in SORegistry.Get<EnemyCardSO>()) {
            enemyCards.Add(new CardData(so.id));
        }
        Debug.Log($"[PlayerData] Finish initialization, datapath is {path}//playerdata.json");
    }

    public static List<MonkeyCardSO> GetOwnedCard() {
        List<MonkeyCardSO> monkeyCardSOs = new();
        foreach(CardData data in monkeyCards) {
            if(data.level > 0) {
                monkeyCardSOs.Add(SORegistry.Get<MonkeyCardSO>(data.id));
            }
        }
        return monkeyCardSOs;
    }
    public static CardData GetCardDataById(string id) {
        foreach(CardData data in monkeyCards) {
            if(data.id == id) { return data; }
        }
        foreach(CardData data in enemyCards) {
            if(data.id == id) {
                return data;
            }
        }
        return null;
    }
    public static void AddCoins(int amount) {
        ownedCoins += amount;
    }
    public static bool TryMinusCoins(int amount) {
        if(ownedCoins - amount < 0) {
            return false;
        }
        else {
            ownedCoins -= amount;
            return true;
        }
    }
    public static int GetOwnedCoins() {
        return ownedCoins;
    }
    // public static void CompleteCampainLevel(int difficulty, Place place, int num) {
    //     int index = ((int)place) * LevelSO.COUNT_FOREACH_PLACE + num;
    //     campainProgress[index] = true;
    // }
    public static void CompleteCampainLevel(int index) {
        campainProgress[index] = true;
    }
    public static bool[] GetProgress(Place place) {
        int startIndex = ((int)place) * LevelSO.COUNT_FOREACH_PLACE;
        bool[] answer = new bool[LevelSO.COUNT_FOREACH_PLACE];
        for(int i = 0; i < LevelSO.COUNT_FOREACH_PLACE; ++i) {
            answer[i] = campainProgress[startIndex + i];
        }
        return answer;
    }
    /// <summary>
    /// Move data from PlayerData to hard memory
    /// Should be called every times player's data is changed. For example: player acquire a new card, complete a level...
    /// </summary>
    public static void Save() {
        //Create PlayerDataRaw instance: contain infomation that can be saved
        PlayerDataRaw rawData = new PlayerDataRaw();
        //Convert from complex data to prime data types: string, int, list
        rawData.playerName = playerName;
        rawData.campainProgress = campainProgress;
        // Add created decks
        foreach(Deck deck in decks) {
            rawData.rawDecks.Add('#' + deck.deckName);
            foreach(MonkeyCardSO so in deck.cardList) {
                rawData.rawDecks.Add(so.id);
            }
        }
        // Add owned cards;
        rawData.monkeyCards = monkeyCards;
        //Write json file to hard memory
        string content = JsonUtility.ToJson(rawData, true);
        File.WriteAllText(path, content);
        Debug.Log("Save successful!");
    }
    /// <summary>
    /// Retrieve data from hard memory and assign them to PlayerData's field
    /// Should be called only one time when players open the game, this function load data from system storage to the game
    /// </summary>
    public static void Load(){
        
        if(!File.Exists(path)) {
            Debug.Log("Not found player data");
            return;
        }

        string json = File.ReadAllText(path);
        PlayerDataRaw rawData = JsonUtility.FromJson<PlayerDataRaw>(json);

        monkeyCards = rawData.monkeyCards;
        enemyCards = rawData.enemyCards;
        
        Deck newDeck = new Deck("empty deck");
        foreach(string id in rawData.rawDecks) {
            if(id[0] == '#') {
                if (newDeck.deckName != "empty deck") {
                    decks.Add(newDeck);
                }
                newDeck = new Deck(id.Substring(1));
            }
            else {
                newDeck.cardList.Add(SORegistry.Get<MonkeyCardSO>(id));
            }
        }

        for(int i = 0; i < rawData.campainProgress.Length; ++i) {
            campainProgress[i] = rawData.campainProgress[i];
        }

        PlayerData.playerName = rawData.playerName;
        Debug.Log("Load data success!");
    }
}
