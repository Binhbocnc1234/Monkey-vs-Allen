using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


public class HideAndShowUIManager : MonoBehaviour {
    [SerializeField] private UDictionary<string, HideAndShowUI> container;
    void Awake() {
        UDictionary<string, HideAndShowUI> new_container = new();
        foreach(var pair in container) {
            if(pair.Key != string.Empty && pair.Value != null) {
                new_container.Add(pair);
            }
        }
        container = new_container;
    }
    public void Show(string name) {
        if(container.TryGetValue(name, out var value)) {
            value.Show();
        }
        else {
            Debug.LogError($"[HideAndShowUIManager] Cannot find HideAndShowUI named {name}");
        }
    }
    public void Hide(string name) {
        if(container.TryGetValue(name, out var value)) {
            value.Hide();
        }
        else {
            Debug.LogError($"[HideAndShowUIManager] Cannot find HideAndShowUI named {name}");
        }
    }
    public void ShowAll(){
        foreach(HideAndShowUI ui in container.Values){
            ui.Show();
        }
    }
    public void HideAll(){
        foreach(HideAndShowUI ui in container.Values){
            ui.Hide();
        }
    }
    public void HideAllImmediately(){
        foreach(HideAndShowUI ui in container.Values){
            ui.HideImmediately();
        }
    }
}