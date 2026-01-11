using UnityEngine;

public class HarmonicOscillation 
{
    private float magnitude;
    private float frequency;
    private float elapsedTime;
    private float baseValue;

    public HarmonicOscillation(float magnitude, float frequency, float baseValue = 0f)
    {
        this.magnitude = magnitude;
        this.frequency = frequency;
        this.baseValue = baseValue;
        elapsedTime = 0f;
    }

    // Call every frame
    public void Update()
    {
        // Debug.Log("Update oscillation");
        elapsedTime += Time.deltaTime;
    }

    // Returns the current oscillating value
    public float GetValue()
    {
        return baseValue + Mathf.Sin(elapsedTime * Mathf.PI * 2f * frequency) * magnitude;
    }
}
