using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    public Interactable television, collection;
    public Canvas levelCanvas;
    private Vector2 initialCamPos;
    void Start()
    {
        levelCanvas.gameObject.SetActive(false);
        television.OnClick += LoadLevelMenu;
        collection.OnClick += LoadCollection;
        initialCamPos = MyCamera.Instance.transform.position;
        // foreach(LobbyLevelUI levelUI in LobbyManagerUI.Instance.levelGrid){
        //     levelUI.button.onClick.AddListener()
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadLevelMenu(){
        levelCanvas.gameObject.SetActive(true);
    }
    public void LoadCollection(){

    }
    public void Reset(){
        MyCamera.Instance.transform.position = initialCamPos;

    }
}
