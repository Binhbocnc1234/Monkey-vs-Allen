using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HinterManager : UpdateManager<HinterManager.Hinter>
{
    public class Hinter : IUpdatePerFrame, IOnDestroy {
        public GameObject target { get; private set; }
        private SpriteRenderer[] renderers;
        private Color color = Color.white;
        private HarmonicOscillation harmonicOscillation = new HarmonicOscillation(30, 0.75f, 220);
        public Hinter(GameObject target) {
            this.target = target;
            renderers = target.GetComponentsInChildren<SpriteRenderer>();
        }
        public void Update() {
            harmonicOscillation.Update();
            float rgbVal = harmonicOscillation.GetValue() / 255f;
            color.r = rgbVal;
            color.g = rgbVal;
            color.b = rgbVal;
            foreach(var ren in renderers) {
                ren.color = color;
            }
        }
        public void OnDestroy() {
            foreach(var ren in renderers) {
                ren.color = Color.white;
            }
        }
    }
    void Awake() {
        SingletonRegister.Register(this);
    }
    public void NewHinter(GameObject target){
        AddElement(new Hinter(target));
    }
    public void RemoveHinter(GameObject target){
        foreach(Hinter hinter in container){
            if (hinter != null && hinter.target == target){
                RemoveElement(hinter);
                break;
            }
        }
    }
}
