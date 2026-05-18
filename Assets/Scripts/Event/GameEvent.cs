using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType{
    Costly,
    
}
public interface IEventCaller{
    public void CallEvent(EventType eventType, IEventCaller caller, IEventCaller receiver);
}

//How to create GameEvent? Call constructor then don't forget to add it to GameEvent.container
public class GameEvent 
{
    public EventType type;
    public GameObject caller;
    public static List<GameEvent> container;
    public GameEvent(EventType type, GameObject caller){
        this.type = type;
        this.caller = caller;
    }
    // public static void CreateNewEvent(EventType )
}
