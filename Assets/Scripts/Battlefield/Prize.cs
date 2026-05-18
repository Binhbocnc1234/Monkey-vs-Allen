using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum PrizeState{
    None,
    Waiting,
    Animating,
    Ending,
}
[RequireComponent(typeof(BoxCollider2D))]
public class Prize : Singleton<Prize>{
    [ReadOnly] public PrizeState state = PrizeState.None;
    public Transform arrow;
    public FlashPanel flashPanel;
    [SerializeField] private GameObject prize, card;
    private bool isCardAnim = false;
    public float animatingTime = 3f;
    private Animator animator;
    private List<Rewardable> rewardables;
    private CardSO cardSO;
    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
    }
    void Start() {
        this.gameObject.SetActive(false);
    }
    public void Initialize(Vector2 position) {
        state = PrizeState.Waiting;
        animator.enabled = false;
        this.gameObject.SetActive(true);
        transform.position = position;
        rewardables = BattleInfo.levelSO.rewardables;
        if(rewardables.Count == 1 && rewardables[0] is CardReward cardReward) {
            isCardAnim = true;
            prize.gameObject.SetActive(false);
            card.gameObject.SetActive(true);
            card.GetComponent<CardUIAppearance>().Initialize(cardReward.so);
            cardSO = cardReward.so;
        }
        else {
            prize.gameObject.SetActive(false);
            card.gameObject.SetActive(true);
        }
    }
    public void OnMouseDown() {
        state = PrizeState.Animating;
        arrow.gameObject.SetActive(false);
        animator.enabled = true;
        if(isCardAnim) {
            animator.Play("Card");
        }
        else {
            animator.Play("Chest");
        }
        GetComponent<LightRayManager>().StartEmit();
        this.transform.LeanMove(Camera.main.transform.position, animatingTime)
            .setEase(LeanTweenType.easeInOutSine);
        // gg
        this.transform.LeanScale(this.transform.localScale * 1.5f, animatingTime)
            .setEase(LeanTweenType.easeInOutSine);
        StartCoroutine(Victory());
    }
    void Update(){

    }
    IEnumerator Victory(){
        yield return new WaitForSeconds(animatingTime * 0.75f);
        state = PrizeState.Ending;
        yield return StartCoroutine(UIManager.Ins.flashPanel.FlashCorou());
        if(isCardAnim) {
            CustomSceneManager.ToNewCard(cardSO);
        }
        else {
            CustomSceneManager.ToPrize(rewardables);
        }
    }

}