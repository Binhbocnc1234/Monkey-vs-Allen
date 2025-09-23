using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyLevelUI : MonoBehaviour
{
    public Button button;
    public Image thumbnail, mask;
    public Image background, underbackground;
    public TMP_Text levelName;
    public Color completeColor, darkCompleteColor, inCompleteColor, darkInCompleteColor;
    private LevelSO so;
    void Awake(){
        button.onClick.AddListener(PrepareForFighting);
        // returnButton.onClick.AddListener(Return);
    }
    public void ApplyLevelSO(LevelSO so){
        this.so = so;
        thumbnail.sprite = so.thumbnail;
        levelName.text = $"{so.place.ToString()} - Level {so.index}";
        if (so.isVisible == false){
            mask.gameObject.SetActive(true);
        }
        else{
            mask.gameObject.SetActive(false);
        }
        if (so.isCompleted){
            background.color = completeColor;
            underbackground.color = darkCompleteColor;
        }
        else{
            background.color = inCompleteColor;
            underbackground.color = darkInCompleteColor;
        }
    }   
    public void PrepareForFighting(){
        Debug.Log("Prepare for fighting");
        BattleInfo.levelSO = so;
        SceneManager.LoadScene("Battlefield");
    }


}
