using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public Camera cam;
    ParallaxLayer[] layers;
    Vector3 lastCamPos;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        layers = FindObjectsOfType<ParallaxLayer>();
        lastCamPos = cam.transform.position;

        foreach (var l in layers)
            l.Init(lastCamPos);
    }

    void LateUpdate()
    {
        Vector3 camPos = cam.transform.position;
        if (camPos == lastCamPos) return;

        foreach (var l in layers)
            l.UpdateLayer(camPos);

        lastCamPos = camPos;
    }
}