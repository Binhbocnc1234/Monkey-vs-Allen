using UnityEngine;

public class Cloud : MonoBehaviour {
    [SerializeField] private new SpriteRenderer renderer;
    Timer timer;
    enum State {
        Appearing,
        Moving,
        Disappearing,
    }
    State state;
    Direction direction;
    private float movingSpeed;
    private float highestAlpha;
    private Bound bound;
    public void Initialize(Sprite sprite, Direction direction, float movingSpeed, float alpha, Bound bound) {
        GetComponent<SpriteRenderer>().sprite = sprite;
        this.direction = direction;
        this.movingSpeed = movingSpeed;
        highestAlpha = alpha;
        this.bound = bound;
        renderer.AssignRendererAlpha(0);
    }
    void Update() {
        if(state == State.Appearing) {
            if(renderer.color.a >= highestAlpha) {
                state = State.Moving;
            }
            else {
                renderer.AdjustRendererAlpha(Time.deltaTime);
            }
        }
        else if(state == State.Moving) {
            if(bound.IsOutsideBound(this.transform.position)) {
                state = State.Disappearing;
            }
            transform.Translate(EnumConverter.Convert(direction) * movingSpeed * Time.deltaTime);
        }
        else {
            renderer.AdjustRendererAlpha(-Time.deltaTime);
            if (renderer.color.a <= 0) {
                Destroy(this.gameObject);
            }
        }
    }
}