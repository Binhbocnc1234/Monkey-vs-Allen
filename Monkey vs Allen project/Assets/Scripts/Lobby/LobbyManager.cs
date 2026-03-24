using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    public Interactable television, collection, freePlayMode;
    public Canvas levelCanvas;
    public Vector3 initialCamPos;
    void Start()
    {
        levelCanvas.gameObject.SetActive(false);
        television.OnClick += () => levelCanvas.gameObject.SetActive(true);;
        collection.OnClick += () => CustomSceneManager.ToCollection();
        freePlayMode.OnClick += () => CustomSceneManager.ToFreePlay();
        initialCamPos = MyCamera.Ins.transform.position;
        Reset();
        // foreach(LobbyLevelUI levelUI in LobbyManagerUI.Ins.levelGrid){
        //     levelUI.button.onClick.AddListener(() => GameManager.Ins.StartBattle(levelUI.so));
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadLevelMenu(){
        levelCanvas.gameObject.SetActive(true);
    }
    public void Reset(){
        MyCamera.Ins.transform.position = initialCamPos;

    }
}
