using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour
    where T : MonoBehaviour //prefab
{
    //Pool là hàng đợi những Object sẵn sàng được lôi ra sử dụng, không tính những Object đang sử dụng
    private Queue<T> pool = new Queue<T>();
    public T prefab;
    public Transform parent;
    public int initialSize = 10;

    protected virtual void Awake()
    {
        // Tạo sẵn vài gameObject
        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public virtual T Get()
    {
        //Trường hợp xảy ra khi Pool cạn kiệt vì toàn bộ Object được lôi ra sử dụng
        if (pool.Count == 0) 
            AddObject();

        T obj = pool.Dequeue(); // Lấy một object có sẵn trong pool để sử dụng
        obj.gameObject.SetActive(true);

        return obj;
    }

    public virtual void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj); // Đưa object vào cuối Pool để sau tái sử dụng
    }

    protected virtual void AddObject() {
        T obj = GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj); // Đưa object vào cuối Pool
    }
    public void Clear() {
        // Reset pool: disable tất cả child dưới parent và rebuild queue từ các child hợp lệ.
        pool.Clear();
        // Duyệt tất cả child của parent, nếu có component T thì set inactive và enqueue.
        for (int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            if (child == null) continue;
            T comp = child.GetComponent<T>();
            if (comp == null) continue;
            Release(comp);
        }
    }
}
