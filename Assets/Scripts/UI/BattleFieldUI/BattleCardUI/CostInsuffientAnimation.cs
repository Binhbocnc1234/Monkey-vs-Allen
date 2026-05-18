using UnityEngine;
public class CostInsuffientAnimation : IUpdatePerFrame {
    private TMPro.TMP_Text text;
    private Timer amplifyingTimer, shrinkingTimer;
    private float sizeMultiplier = 15f;
    private float duration = 0.35f;
    public static void Instantiate(BattleCardUI cardUI) {
        GeneralUM.Ins.AddElement(new CostInsuffientAnimation(cardUI.appearance.cost));
    }
    private CostInsuffientAnimation(TMPro.TMP_Text text) {
        this.text = text;
        amplifyingTimer = new Timer(duration, false);
        shrinkingTimer = new Timer(duration, false);
        text.color = Color.red;
    }
    public void Update() {
        if(!amplifyingTimer.Count()) {
            text.fontSize += Time.deltaTime * sizeMultiplier;
        }
        else if(!shrinkingTimer.Count()) {
            text.fontSize -= Time.deltaTime * sizeMultiplier;
        }
        else {
            text.color = Color.white;
            GeneralUM.Ins.RemoveElement(this);
        }
    }
}