using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlmanacManager : MonoBehaviour
{
    private enum Category {
        
    }
    [SerializeField] private AlmanacCardContainer cardContainer;
    [SerializeField] private List<CategoryButton> categoryButtons;
    void Start() {
        for (int i = 0; i < categoryButtons.Count; i++) {
            int idx = i; // capture value
            categoryButtons[i].GetComponent<Button>()
                .onClick.AddListener(() => SwitchToCategory(idx));
        }
        SwitchToCategory(0);
    }

    public void SwitchToCategory(int categoryIndex) {
        for(int i = 0; i < categoryButtons.Count; ++i) {
            categoryButtons[i].Deselect();
        }
        categoryButtons[categoryIndex].Select();
        switch(categoryIndex) {
            case 0:
                cardContainer.SetReferencedList(PlayerData.MonkeyCards, true);
                break;
            case 1:
                cardContainer.SetReferencedList(PlayerData.EnemyCards, false);
                break;
        }
    }
}
