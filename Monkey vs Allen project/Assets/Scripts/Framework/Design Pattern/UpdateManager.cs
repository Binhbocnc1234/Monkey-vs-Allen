using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UpdateManager<T> : MonoBehaviour where T : IUpdatePerFrame
{
    protected readonly List<T> container = new();
    protected readonly List<T> pendingRemoved = new(), pendingAdded = new();
    protected virtual void Update(){
        container.RemoveAll(u => u == null || pendingRemoved.Contains(u));
        pendingRemoved.Clear();
        container.AddRange(pendingAdded);
        pendingAdded.Clear();
        foreach(T element in container) {
            element.Update();
            if (element is IDestroyable destroyable && destroyable.IsDead()) {
                pendingRemoved.Add(element);
            }
        }
    }
    protected virtual void AddElement(T element){
        pendingAdded.Add(element);
    }
    protected virtual void RemoveElement(T element) {
        if(element is IOnDestroy destroy) {
            destroy.OnDestroy();
        }
        pendingRemoved.Add(element);
    }
    public ReadOnlyCollection<T> GetContainer() {
        return container.AsReadOnly();
    }
    public void Reset() {
        foreach(T element in container) {
            if (element is IOnDestroy destroy) {
                destroy.OnDestroy();
            }
        }
        container.Clear();
        pendingAdded.Clear();
        pendingRemoved.Clear();
    }
}