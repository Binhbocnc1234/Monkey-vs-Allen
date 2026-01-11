using UnityEngine;
using System.Collections.Generic;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null){
                    Debug.LogError($"Not found singleton of {typeof(T).ToString()}");
                }
               
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
        SingletonRegister.Register(this as T);
    }
}

