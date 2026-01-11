using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private Entity linkedEntity;
    private int lane;
    public void Initialize(Entity e, int lane){
        this.gameObject.SetActive(true);
        e.OnEntityDeath += (e) => {
            SingletonRegister.Get<ShadowContainer>().Release(this);
        };
        this.linkedEntity = e;
        this.lane = lane;
        transform.position = GridSystem.Ins.GridToWorldPosition(2, lane) - new Vector2(0, 0.5f);
    }

    // Di chuyển Shadow bám theo Entity, chỉ thay đổi tọa độ x, tọa độ y giữ nguyên
    void Update()
    {
        transform.position = new Vector2(linkedEntity.GetWorldPosition().x, transform.position.y);
    }
}
