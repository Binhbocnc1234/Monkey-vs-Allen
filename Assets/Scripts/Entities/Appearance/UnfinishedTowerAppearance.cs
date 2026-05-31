using UnityEngine;

public class UnfinishedTowerAppearance : MonoBehaviour {
    [SerializeField] private UnfinishedTower tower;
    private FillBar progressBar;

    void Start() {
        if(tower == null) return;
        tower.OnProgressChanged += OnProgress;
        tower.OnConstructionComplete += OnComplete;
    }

    void OnProgress(float normalized) {
        if(progressBar == null) {
            progressBar = FillBarManager.Ins.CreateProgressBar();
            progressBar.transform.position = tower.transform.position - Vector3.down * 2;
        }
        progressBar.gameObject.SetActive(true);
        progressBar.SetValue(normalized);
    }

    void OnComplete() {
        if(progressBar != null) {
            Destroy(progressBar.gameObject);
        }
        Destroy(gameObject);
    }
}
