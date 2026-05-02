using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour {
    public Image progressBar;
    void Awake() {
        StartCoroutine(LoadSceneAsync(CustomSceneManager.targetScene));
    }
    IEnumerator LoadSceneAsync(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while(op.progress < 0.9f) {
            progressBar.fillAmount = op.progress / 0.9f;
            yield return null;
        }
        // optional: wait for animation / minimum time
        progressBar.fillAmount = 1;
        op.allowSceneActivation = true;
    }
}