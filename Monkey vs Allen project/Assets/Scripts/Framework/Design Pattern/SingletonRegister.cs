using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A centralized registry for managing and retrieving Singleton instances.
/// This prevents multiple instances of the same Type being active at once,
/// enforcing the Singleton pattern without requiring inheritance.
/// </summary>
public static class SingletonRegister
{
    // Dictionary to hold instances: Key is the Type, Value is the instance object.
    private static readonly Dictionary<Type, object> _instanceMap = new Dictionary<Type, object>();

    /// <summary>
    /// Registers an instance of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the instance being registered.</typeparam>
    /// <param name="instance">The actual instance to register (e.g., 'this').</param>
    public static void Register<T>(T instance) where T : class
    {
        Type type = typeof(T);
        if (instance == null) {
            Debug.LogError($"[SingletonRegister] instance of {type} is null!");
            return;
        }
        object obj = _instanceMap.TryGetValue(type, out object value) ? value : null;
        if(obj != null) {
            Debug.Log($"[SingletonRegister] Found old instance of {type}, replaced old instance with new one");
            // Debug.LogError($"[SingletonRegister] Attempted to register Type '{type.Name}' twice. " +
            //                "The previous instance will remain active. Check for duplicate Awake/Start calls.");
        }
        else {
            Debug.Log($"[SingletonRegister] Registered: {type.Name}");
        }
        _instanceMap[type] = instance;
        
    }

    /// <summary>
    /// Retrieves the registered instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the instance to retrieve.</typeparam>
    /// <returns>The registered instance, or null if not found.</returns>
    public static T Get<T>() where T : class {
        Type type = typeof(T);

        if(_instanceMap.TryGetValue(type, out object instance)) {
            return instance as T;
        }

        Debug.LogError($"[SingletonRegister] Attempted to get unregistered Type: '{type.Name}'. " +
                       "Ensure the instance is registered in its Awake method.");
        return null;
    }
    public static void Reset()
    {
        Debug.Log("[SingletonRegister] Reset dictionary");
        _instanceMap.Clear();
    }
}