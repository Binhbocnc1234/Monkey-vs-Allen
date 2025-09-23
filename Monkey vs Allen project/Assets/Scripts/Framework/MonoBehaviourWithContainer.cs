using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourWithContainer<T> : MonoBehaviour where T : MonoBehaviour
{
    public static List<T> _container = new List<T>();
    public static List<T> container{
        get{
            ResetContainer();
            return _container;
        }
    }
    static MonoBehaviourWithContainer(){
        ResetContainer();
        Debug.Log($"Reset container of class {typeof(T).ToString()}, now size of container is {container.Count}");
    }
    public static void ResetContainer(){
        _container.RemoveAll(item => item == null);
    }
    protected virtual void Awake(){
        if (!_container.Contains(this as T)){
            _container.Add(this as T);
        }
    }
}
