using UnityEngine;

public class PrimalBreachEnviroment : MonoBehaviour {
    public Transform topSide, bottomSide, rightSide, exampleGrid;
    public void Initialize(int gridW) {
        Destroy(exampleGrid.gameObject);
        float bound = IGrid.Ins.GridToWorldPosition(gridW - 1, 2).x;
        foreach(Transform env in topSide) {
            if(env.position.x > bound) {
                Destroy(env.gameObject);
            }
        }
        foreach(Transform env in bottomSide) {
            if(env.position.x > bound) {
                Destroy(env.gameObject);
            }
        }

        rightSide.transform.AssignXPos(bound);
    }
}