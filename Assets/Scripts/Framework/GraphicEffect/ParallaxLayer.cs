using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f; //0.1 : very far to 0.9 : very near

    Vector3 lastCameraPos;

    public void Init(Vector3 camPos)
    {
        lastCameraPos = camPos;
    }

    public void UpdateLayer(Vector3 camPos)
    {
        Vector3 delta = camPos - lastCameraPos;
        transform.position += new Vector3(
            delta.x * parallaxFactor,
            delta.y * parallaxFactor,
            0f
        );
        lastCameraPos = camPos;
    }
}