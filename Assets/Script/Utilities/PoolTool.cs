using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PoolTool : MonoBehaviour
{
    public GameObject objPrefab;
    public ObjectPool<GameObject> pool;
    private GameObject[] preFillArray;
    private List<GameObject> poolObjectList = new List<GameObject>();
    private void Awake()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var obj = Instantiate(objPrefab, transform);
                poolObjectList.Add(obj);
                return obj;
            },
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) =>
            {
                poolObjectList.Remove(obj);
                Destroy(obj);
            },
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );
        PreFillPool(10);
    }
    //预先生成对象
    private void PreFillPool(int count)
    {
        preFillArray = new GameObject[count];
        for (int i = 0; i < count; i++)
        {

            preFillArray[i] = pool.Get();

        }
        foreach (var obj in preFillArray)
        {
            pool.Release(obj);
        }

    }
    //获取对象
    public GameObject GetGameObjectFromPool()
    {
        return pool.Get();
    }
    //释放对象
    public void ReleaseGameObjectToPool(GameObject obj)
    {
        pool.Release(obj);
    }
    //销毁所有对象池子物体
    [ContextMenu("刷新对象池")]
    public void DestroyAllObjects()
    {
        foreach (var obj in poolObjectList)
        {
            Destroy(obj);
        }
        pool.Clear();
        poolObjectList.Clear();
        PreFillPool(10);
    }
}
