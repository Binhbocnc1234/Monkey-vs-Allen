using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private IEntity linkedEntity;
    private int lane;
    public void Initialize(IEntity e){
        this.gameObject.SetActive(true);
        e.OnEntityDeath += () => SingletonRegister.Get<ShadowContainer>().Release(this);
        this.linkedEntity = e;
        this.lane = e.lane;
        transform.position = e.model != null ? e.model.GetPosition() : Vector2.zero;
    }

    // Di chuy?n Shadow b�m theo Entity, ch? thay d?i t?a d? x, t?a d? y gi? nguy�n
    void Update()
    {
        transform.position = new Vector2(linkedEntity.model.GetPosition().x, transform.position.y);
    }
}