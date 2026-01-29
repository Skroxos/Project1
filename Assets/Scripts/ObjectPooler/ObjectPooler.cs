using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public List<GameObject> objectsToPool;
    public int amountToPool;
    private Dictionary<string, List<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializePool();
    }

    public void InitializePool()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach (GameObject obj in objectsToPool)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < amountToPool; i++)
            {
                GameObject pooledObj = Instantiate(obj);
                pooledObj.SetActive(false);
                objectPool.Add(pooledObj);
            }

            poolDictionary.Add(obj.name, objectPool);
        }
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (poolDictionary != null && poolDictionary.ContainsKey(prefab.name))
        {
            foreach (GameObject obj in poolDictionary[prefab.name])
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
                if (obj == poolDictionary[prefab.name][poolDictionary[prefab.name].Count - 1])
                {
                    GameObject newObj = Instantiate(prefab);
                    newObj.SetActive(false);
                    poolDictionary[prefab.name].Add(newObj);
                    return newObj;
                }
            }
        }
        
        return null;
    }
}
