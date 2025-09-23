
using UnityEngine;
using System;
using System.Collections;

public static class LeanTweenFake{
    public static IEnumerator MoveUI(RectTransform target, Vector2 end, float duration){
        Vector2 start = target.anchoredPosition;
        float t = 0f;
        while(t < 1f){
            t += Time.unscaledDeltaTime / duration; // ignore timescale like LeanTween
            target.anchoredPosition = Vector2.Lerp(start, end, Mathf.Clamp01(t));
            yield return null;
        }
        target.anchoredPosition = end;
    }
    // public static IEnumerator MoveUICoroutine(RectTransform target, Vector2 end, float duration){
        
    // }
    public static IEnumerator Move(Transform target, Vector2 end, float duration){
        Vector2 start = target.position;
        float t = 0f;
        while(t < 1f){
            t += Time.deltaTime / duration; // ignore timescale like LeanTween
            target.position = Vector2.Lerp(start, end, Mathf.Clamp01(t));
            yield return null;
        }
        target.position = end;
    }
}