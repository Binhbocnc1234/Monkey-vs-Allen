using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyLevelUI : MonoBehaviour {
    public enum State {
        Completed,
        Incompleted,
        Visible,
    }
    private State state;
    public Image thumbnail, mask;
    public Image background, underbackground;
    public TMP_Text levelName;
    public Color completeColor, darkCompleteColor, inCompleteColor, darkInCompleteColor;
    public LevelSO so { get; private set; }
    public void ApplyLevelSO(LevelSO so, State state) {
        this.so = so;
        this.state = state;
        thumbnail.sprite = so.thumbnail;
        levelName.text = $"{so.place} - Level {so.number}";
        // if (so.isVisible == false){
        //     mask.gameObject.SetActive(true);
        // }
        // else{
        //     mask.gameObject.SetActive(false);
        // }
        if(state == State.Completed) {
            background.color = completeColor;
            underbackground.color = darkCompleteColor;
        }
        else if(state == State.Incompleted) {
            background.color = inCompleteColor;
            underbackground.color = darkInCompleteColor;
        }
        else if (state == State.Visible){
            background.color = inCompleteColor;
            underbackground.color = darkInCompleteColor;
            mask.gameObject.SetActive(true);
        }
    }

    public void PrepareForFighting() {
        CustomSceneManager.ToBattleField(so);
    }

}
