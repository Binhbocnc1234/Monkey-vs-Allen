using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public void UnlimitedBanana(bool toggle) {
        BattleInfo.ToggleUnlimitedBanana(toggle);
    }
    public void NoCooldown(bool toggle) {
        BattleInfo.ToggleCooldown(toggle);
    }
    public void ClearEntity() {
        foreach(IEntity e in EContainer.Ins.GetEntities()) {
            e.Die();
        }
    }
}
