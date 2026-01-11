using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAllenMove : Move {
    public Transform targetAllen;
    public float hopHeight = 0.5f;
    [ReadOnly] public float hopDistance = 1f;
    public float hopDuration = 0.5f;
    public float pauseBetweenHops = 0.5f;
    float jumpTimer = 0f;
    bool jumping = false;
    float groundY;
    Vector2 startPos;
    Timer pauseTimer;

    public override void Initialize() {
        base.Initialize();
        hopDistance = moveSpeed/2;
        startPos = targetAllen.position;
        groundY = startPos.y;
        pauseTimer = new Timer(pauseBetweenHops, true);
        pauseTimer.Reset(); // start initial pause
        jumping = false;
    }

    protected override void UpdateBehaviour() { //pauseTimer null reference vi initialize() chua duoc goi
        // waiting (pause) state
        if (!jumping) {
            if (!pauseTimer.Count()) return; // still pausing
            // pause finished -> begin jump
            jumping = true;
            jumpTimer = 0f;
            startPos = targetAllen.position; //startPos become starting point for new jump
        }

        // jumping state
        jumpTimer += Time.deltaTime;
        float t = jumpTimer / hopDuration;
        
        if (t >= 1f) { //The moment Allen landed on the ground was also the time parabolic path is ended
            targetAllen.position = new Vector2(startPos.x + hopDistance * GetNormalizedDirection(), groundY);
            jumping = false;
            pauseTimer.totalTime = pauseBetweenHops; // allow runtime changes
            pauseTimer.Reset(); // start next pause
            return;
        }

        float x = startPos.x + hopDistance * GetNormalizedDirection() * t;
        float y = groundY + hopHeight * 4f * t * (1f - t); //parabolic jump path
        targetAllen.position = new Vector2(x, y);
    }
}
