
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class HideAndShowUI : MonoBehaviourWithContainer<HideAndShowUI>  {
    public enum State { Show, Hide}
    [ReadOnly] public bool isAnimating = false;
    [ReadOnly] public State state = State.Show;
    public Direction direction;
    private Vector2 originalPos, hiddenPos;
    private RectTransform rectTrans;
    protected override void Awake(){
        base.Awake();
        rectTrans = GetComponent<RectTransform>();
        Vector2 dir = EnumConverter.Convert(direction);
        originalPos = rectTrans.anchoredPosition;
        hiddenPos = originalPos + new Vector2(rectTrans.rect.width * dir.x, rectTrans.rect.height * dir.y) * 1.2f;
    }
    void Start(){
        
    }
    public void Show(){
        if(state == State.Show) { return; }
        gameObject.SetActive(true);
        StartCoroutine(ShowCoroutine());
    }
    public void Hide(){
        if(state == State.Hide) { return; }
        StartCoroutine(HideCoroutine());
    }
    public IEnumerator HideCoroutine(){
        state = State.Hide;
        isAnimating = true;
        yield return StartCoroutine(LeanTweenFake.MoveUI(rectTrans, hiddenPos, 0.4f));
        gameObject.SetActive(false);
        isAnimating = false;
    }
    public void HideImmediately(){
        this.gameObject.SetActive(false);
        state = State.Hide;
        rectTrans.anchoredPosition = hiddenPos;
    }
    public IEnumerator ShowCoroutine(){
        state = State.Show;
        isAnimating = true;
        yield return StartCoroutine(LeanTweenFake.MoveUI(rectTrans, originalPos, 0.4f));
        isAnimating = false;
    }
    public static void ShowAll(){
        foreach(HideAndShowUI ui in container){
            ui.Show();
        }
    }
    public static void HideAll(){
        foreach(HideAndShowUI ui in container){
            ui.Hide();
        }
    }
    public static void HideAllImmediately(){
        foreach(HideAndShowUI ui in container){
            ui.HideImmediately();
        }
    }
}
