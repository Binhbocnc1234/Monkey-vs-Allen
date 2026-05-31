using UnityEngine;

public class BulletSpawnRequest {
	public GameObject prefab;
	public Vector3 position;
	public Quaternion rotation = Quaternion.identity;
	public IEntity owner;
	public IEntity target; // optional
	public float damage;
	public int lane;
}

public interface IBullet {
	void Initialize(BulletSpawnRequest request);
}

public interface IBulletSpawner {
	IBullet Spawn(BulletSpawnRequest req);
	public static IBulletSpawner Ins { get; set; }
}
