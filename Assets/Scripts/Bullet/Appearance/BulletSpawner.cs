using UnityEngine;

public class BulletSpawner : MonoBehaviour, IBulletSpawner
{
    public Transform holder;

    void Awake()
    {
        IBulletSpawner.Ins = this;
        if (holder == null) Debug.LogError("[BulletSpawner] Unassigned holder");
    }

    public IBullet Spawn(BulletSpawnRequest req)
    {
        if(req.owner.isSimulated) {
            Debug.LogError("[BulletSpawner] If the owner is a `Simulated Entity`, they cannot spawn bullets.");
            return null;
        }
        if (req.prefab == null) return null;
        
        GameObject go = Object.Instantiate(req.prefab, Vector3.zero, req.rotation, holder);
        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Initialize(req);
        ((BulletModel)bullet.model).Initialize(bullet);
        return bullet;
    }
}
