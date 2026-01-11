using UnityEngine;


public static class Utility{
    public static Vector3 RandomOffset(float magtitude){
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1.0f, 1f))*magtitude;
    }

}