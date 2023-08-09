using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingScript : MonoBehaviour
{
    public List<GameObject> pooledObjects = new List<GameObject>();
    public GameObject objectToPool;
    public int amountToPool;
    public float timedelayCreatPool = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (amountToPool > 0)
            Invoke("createPool", timedelayCreatPool);
    }

    public void createPool()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            addNewPool();
        }
    }

    public GameObject addNewPool()
    {
        GameObject tmp = null;
        if (objectToPool == null) objectToPool = new GameObject(gameObject.name + "_Temp");
        if (objectToPool != null)
        {
            tmp = Instantiate(objectToPool, transform);
            tmp.transform.localPosition = Vector3.zero;
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
        return tmp;
    }

    public GameObject getPoolGameObject()
    {
        foreach (GameObject item in pooledObjects)
            if (item != null && !item.activeSelf) return item;
        return addNewPool();
    }

    public GameObject getPoolGameObject(List<Transform> list)
    {
        foreach (GameObject item in pooledObjects)
            if (!item.activeSelf && list.IndexOf(item.transform) < 0) return item;
        return addNewPool();
    }
}
