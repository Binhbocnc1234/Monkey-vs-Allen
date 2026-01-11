using UnityEngine;
using System;
using System.Collections.Generic;


// public class UIAnimationManager : MonoBehaviour {
//     public enum Direction {
//         Up,
//         Down,
//         Left,
//         Right,
//         None
//     }
//     [Serializable]
//     public class UIAction {
//         public ActionType actionType;
//         public RectTransform target;
//     }
//     public enum ActionType {
//         [InspectorName("Show/FloatInFromTop")] FloatInTop,
//         [InspectorName("Show/FloatInFromBottom")] FloatInBottom,
//         [InspectorName("Show/FloatInFromLeft")] FloatInLeft,
//         [InspectorName("Show/FloatInFromRight")] FloatInRight,
//         [InspectorName("Show/Zoom")] Zoom,
//         [InspectorName("Hover/Enlarge")] Enlarge,
//         [InspectorName("Hide/FloatOutFromTop")] FloatOutTop,
//         [InspectorName("Hide/FloatOutFromBottom")] FloatOutBottom,
//         [InspectorName("Hide/FloatOutFromLeft")] FloatOutLeft,
//         [InspectorName("Hide/FloatOutFromRight")] FloatOutRight,
//         [InspectorName("Hide/Shrink")] Shrink,
//     }
//     public List<UIAction> actions;
//     public void ExecuteActions() {
//         foreach(UIAction action in actions) {
            
//         }
//     }
//     public void ShowAll() {
//         foreach(RectTransform ui in dict.Values) {
//             IShow ishow = ui.GetComponent<IShow>();
//             if(ishow != null) {
//                 ishow.Show();
//             }
//         }
//     }
//     public void HideAll() {
//         foreach(RectTransform ui in dict.Values) {
//             IHide ishow = ui.GetComponent<IHide>();
//             if(ishow != null) {
//                 ishow.Hide();
//             }
//         }
//     }
//     public void HideAllImmediately() {
//         foreach(RectTransform ui in dict.Values) {
//             if()
//                 ui.HideImmediately();
//         }
//     }
// }