using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject prefab;
    public TestScriptableObject testSO;
    void Start()
    {
        Instantiate(prefab, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
