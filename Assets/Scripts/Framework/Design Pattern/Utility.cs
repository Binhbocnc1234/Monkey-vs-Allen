using System.Collections.Generic;
using UnityEngine;


public static class Utilities{
    public static Vector3 RandomOffset(float magtitude){
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1.0f, 1f))*magtitude;
    }
    public static bool RollAndGetResult(float upperBound) {
        return Random.Range(1, 101) <= upperBound;
    }
    public static T RandomElement<T>(List<T> lst) {
        return lst[Random.Range(0, lst.Count)];
    }
}