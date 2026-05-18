using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FeetAligner : MonoBehaviour {
    public bool enableSimulation = false;
    public List<Transform> feet;
    public float groundY;
    private Model model;
    void Awake() {
        model = GetComponent<Model>();
    }
    void Update() {
        if(enableSimulation == false) return;
        float lowest_y = 999;
        foreach(Transform foot in feet) {
            lowest_y = Mathf.Min(lowest_y, foot.transform.position.y);
        }
        float diff1 = lowest_y - groundY;
        model.transform.AssignYPos(model.transform.position.y - diff1);
    }
}