using UnityEngine;
using UnityEngine.UI;

public class BattleModifier : MonoBehaviour{
    public InputField inputField;
    void Awake() {
        inputField.gameObject.SetActive(false);
    }
    void Update() {
        if(Input.GetKeyUp(KeyCode.Slash)) {
            inputField.gameObject.SetActive(true);
            inputField.text = "/";
        }
    }
    public void ModifyTimeScale(float x) {
        Time.timeScale = Mathf.Clamp(x, 0.25f, 3);
    }
    public void SpawnEntity(string entityName, int x, int y) {
        
    }
}