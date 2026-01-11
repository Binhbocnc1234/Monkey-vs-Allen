
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(RectTransform), typeof(IShow), typeof(IHide))]
public class HideAndShowUI : MonoBehaviour, IShow, IHide  {
    public enum State { Show, Hide}
    [ReadOnly] public bool isAnimating = false;
    [ReadOnly] public State state = State.Show;
    public Direction appearFrom;
    private Vector2 originalPos, hiddenPos;
    private RectTransform rectTrans;
    protected virtual void Awake(){
        rectTrans = GetComponent<RectTransform>();
        Vector2 dir = DirectionConverter.Convert(appearFrom);
        originalPos = rectTrans.anchoredPosition;
        hiddenPos = originalPos + new Vector2(rectTrans.rect.width * dir.x, rectTrans.rect.height * dir.y) * 1.2f;
    }
    void Start(){
        
    }
    public void Show(){
        if(state == State.Show) { return; }
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ShowCoroutine());
    }
    public void Hide(){
        if(state == State.Hide) { return; }
        StopAllCoroutines();
        StartCoroutine(HideCoroutine());
    }
    protected IEnumerator HideCoroutine(){
        state = State.Hide;
        isAnimating = true;
        yield return StartCoroutine(LeanTweenFake.MoveUI(rectTrans, hiddenPos, 0.25f));
        gameObject.SetActive(false);
        isAnimating = false;
    }
    public void HideImmediately(){
        this.gameObject.SetActive(false);
        state = State.Hide;
        rectTrans.anchoredPosition = hiddenPos;
    }
    protected IEnumerator ShowCoroutine(){
        state = State.Show;
        isAnimating = true;
        yield return StartCoroutine(LeanTweenFake.MoveUI(rectTrans, originalPos, 0.25f));
        isAnimating = false;
    }
    public static void HideAllImmediately(){
        // foreach(HideAndShowUI ui in container){
        //     ui.HideImmediately();
        // }
    }
}
