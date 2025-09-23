using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerDataRaw{
    public List<string> ownedCardIDs = new List<string>();
    public List<string> rawDecks = new List<string>();
    public string playerName = "User2175162#";
    public int completedLevel = 0;
}
[System.Serializable]
public static class PlayerData{
    static string path = Path.Combine(Application.persistentDataPath, "playerdata.json");
    public static string playerName = "User2175162#";
    public static int completedLevel = 0;
    public static List<CardSO> ownedCards = new List<CardSO>();
    public static List<string> deckNames = new List<string>();
    public static List<List<CardSO>> decks = new List<List<CardSO>>();
    static PlayerData(){
        Load();
    }
    public static void AddCardToDeck(string deckName, CardSO cardSO){
        GetDeckByName(deckName).Add(cardSO);
    }
    public static void CreateDeck(string deckName){
        deckNames.Add(deckName);
        decks.Add(new List<CardSO>());
    }
    public static List<CardSO> GetDeckByName(string name){
        for(int i = 0; i < deckNames.Count; ++i){
            if (deckNames[i] == name){
                return decks[i];
            }
        }
        return null;
    }
    /// <summary>
    /// Should be called every times player's data is changed. For example: player acquire a new card, complete a level...
    /// </summary>
    public static void Save(){ 
        //Create PlayerDataRaw instance: contain infomation that can be saved
        PlayerDataRaw rawData = new PlayerDataRaw();
        //Convert from complex data to prime data types: string, int, list
        rawData.completedLevel = completedLevel;
        rawData.playerName = playerName;
        int ind = 0;
        foreach(var lst in decks){
            rawData.rawDecks.Add('#' + deckNames[ind]);
            foreach(CardSO so in lst){
                rawData.rawDecks.Add(so.id);
            }
        }
        foreach(CardSO so in ownedCards) { rawData.ownedCardIDs.Add(so.id); }
        //Write json file to hard memory
        string json = JsonUtility.ToJson(rawData, true);
        File.WriteAllText(path, json);
        Debug.Log("Save successful!");
    }
    /// <summary>
    /// Should be called only one time when players open the game, this function load data from system storage to the game
    /// 
    /// </summary>
    public static void Load(){
        if(!File.Exists(path)) {
            if (BattleInfo.isDebugMode){
                ownedCards = new List<CardSO>();
                foreach(CardSO so in CardSO.container){
                    if (so.isPlayerCard){
                        ownedCards.Add(so);
                    }
                }
            }
            Debug.Log("Not found player data"); return;
        }
        string json = File.ReadAllText(path);
        PlayerDataRaw rawData = JsonUtility.FromJson<PlayerDataRaw>(json);
        foreach(string id in rawData.ownedCardIDs){
            ownedCards.Add(CardSO.RetrieveAsset(id));
        }

        foreach(string id in rawData.rawDecks){
            if (id[0] == '#'){
                deckNames.Add(id.Substring(1));
                decks.Add(new List<CardSO>());
            }
            else{
                decks[deckNames.Count - 1].Add(CardSO.RetrieveAsset(id));
            }
        }
        Debug.Log("Load data success!");
    }
}
