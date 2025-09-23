using System.Collections;
using UnityEngine;
using TMPro;

public class BananaUI : HideAndShowUI
{
    public static BananaUI Instance;
    public TMP_Text tmp;
    private new Coroutine animation = null;

    protected override void Awake() {
        base.Awake();
        Instance = this;
    }
    public void Initialize(){
        tmp.text = BattleInfo.BananaCnt.ToString();
        BattleInfo.OnBananaChange += BananaUpdate;
        Debug.Log("Initialize BananaUI");
        animation = null;
    }

    void BananaUpdate() {
        tmp.text = BattleInfo.BananaCnt.ToString();

        if (animation != null && gameObject != null && this != null) {
            StopCoroutine(animation);
        }
        animation = StartCoroutine(Animate());
    }

    IEnumerator Animate() {
        // instant color change
        tmp.color = Color.yellow;

        float duration = 0.3f;       // time to grow
        float shrinkDuration = 0.3f; // time to shrink
        float targetScale = 1.5f;

        float t = 0f;
        Vector3 baseScale = Vector3.one;

        // scale up
        while (t < duration) {
            t += Time.deltaTime;
            float progress = t / duration;
            tmp.rectTransform.localScale = Vector3.Lerp(baseScale, baseScale * targetScale, progress);
            yield return null;
        }

        // scale back
        t = 0f;
        while (t < shrinkDuration) {
            t += Time.deltaTime;
            float progress = t / shrinkDuration;
            tmp.rectTransform.localScale = Vector3.Lerp(baseScale * targetScale, baseScale, progress);
            yield return null;
        }

        tmp.rectTransform.localScale = baseScale;
        tmp.color = Color.white; // back to normal
    }
    // void OnDestroy(){
    //     if (animation != null){
    //         animation = null;
    //         BattleInfo.OnBananaChange -= BananaUpdate;
    //     }
    // }
}
