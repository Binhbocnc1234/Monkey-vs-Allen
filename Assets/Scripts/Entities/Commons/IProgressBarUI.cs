using UnityEngine;

public interface IProgressBarUI {
    void InitializeProgressBar(Vector3 worldPos);
    void UpdateProgress(float normalizedValue);
    void ShowProgressBar(bool show);
    void DestroyProgressBar();
}
