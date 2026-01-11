using System;
using System.Collections.Generic;

public class MyEvent<T> {
    public T caller;
    public MyEvent(T caller) {
        this.caller = caller;
    }
}

/// <summary>
/// EventChannel chứa các loại event, mỗi loại event có 1 hoặc nhiều Subscription
/// </summary>
public class EventChannel {
    private readonly Dictionary<Type, List<Subscription>> _table = new();

    public void Subscribe<T>(Action<T> callback, bool oneTime = false) {
        var type = typeof(T);
        if(!_table.ContainsKey(type))
            _table[type] = new List<Subscription>();
        _table[type].Add(new Subscription(e => callback((T)e), oneTime));
    }
    /// <summary>
    /// Khi muốn Event nào đó thực hiện, hãy gọi hàm Invoke
    /// Tham số evt chứa thông tin về event
    /// Các Subscription sau đó cũng được thông báo
    /// </summary>
    public void Invoke<T>(T evt) {
        var type = typeof(T);
        if(!_table.ContainsKey(type)) return;

        var subs = _table[type];
        var toInvoke = new List<Subscription>(subs);

        foreach(var s in toInvoke) {
            s.Callback?.Invoke(evt);
            if(s.OneTime) subs.Remove(s);
        }

        if(subs.Count == 0)
            _table.Remove(type);
    }

    public void Clear() {
        _table.Clear();
    }

    private class Subscription {
        public Action<object> Callback;
        public bool OneTime;
        public Subscription(Action<object> cb, bool oneTime) {
            Callback = cb;
            OneTime = oneTime;
        }
    }
}
