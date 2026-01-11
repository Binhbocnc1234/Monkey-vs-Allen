using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DropPartState{
    InActive,
    Falling,
    Idle,
    FadeOut,
}
[RequireComponent(typeof(SpriteRenderer))]
public class DropBodyPart : MonoBehaviour{
    [ReadOnly] protected DropPartState state = DropPartState.InActive;
    public SpriteRenderer spriteRenderer;
    public Vector2 velocity;
    public float groundPos = 5;
    public bool canFadeOut = false;
    private float idleDuration = -1;
    private Timer idleTimer;
    void Awake(){
        enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // public static DropBodyPart Instantiate(int initX, int laneIndex, float duration){

    // }
    public void Initialize(int laneIndex){
        state = DropPartState.Falling;
        groundPos = IGrid.Ins.GridToWorldPosition(1, laneIndex).y;
        velocity = new Vector2(Random.Range(-3, 3), Random.Range(0, 3));
        this.transform.SetParent(null);
        this.enabled = true;
    }
    public void Initialize(int laneIndex, float duration){
        Initialize(laneIndex);
        SetIdleTime(duration);
    }
    protected void SetIdleTime(float duration){
        canFadeOut = true;
        idleDuration = duration;
        idleTimer = new Timer(duration, true);
    }
    protected virtual void Update(){
        if (state == DropPartState.Falling){
            velocity.y -= TechnicalInfo.gravity * Time.deltaTime;
            transform.Translate(velocity*Time.deltaTime);
            if (transform.position.y < groundPos){
                state = DropPartState.Idle;
            }
        }
        else if (state == DropPartState.Idle){
            if (canFadeOut && idleTimer.Count()){
                state = DropPartState.FadeOut;
            }
        }
        else if (state == DropPartState.FadeOut){
            Color c = spriteRenderer.color;
            c.a -= Time.deltaTime;
            spriteRenderer.color = c;
            if (c.a <= 0){
                Destroy(this.gameObject);
            }
        }
    }
    IEnumerator FadeOut(float duration) {
        Color c = spriteRenderer.color;
        float startAlpha = c.a;
        float t = 0f;
        while (t < duration) {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, t / duration);
            spriteRenderer.color = c;
            yield return null;
        }
        c.a = 0f;
        spriteRenderer.color = c;
    }


}