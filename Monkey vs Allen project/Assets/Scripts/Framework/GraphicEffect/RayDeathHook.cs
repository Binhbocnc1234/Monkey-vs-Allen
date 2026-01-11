using UnityEngine;

public class RayDeathHook : MonoBehaviour
{
    LightRayManager manager;

    public void Init(LightRayManager m)
    {
        manager = m;
    }

    void OnDestroy()
    {
        if (manager != null)
            manager.NotifyRayDeath();
    }
}
