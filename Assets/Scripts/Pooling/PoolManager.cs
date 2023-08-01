using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    public Transform PoolParent;

    [System.Serializable]
    internal struct Pool
    {
        internal Queue<PoolObject> pooledObjects;
        [SerializeField] internal PoolObject objectPrefab;
        [SerializeField] internal int poolSize;
    }

    [SerializeField]
    private Pool[] pools;
    public bool hasLoading = false;

    [SerializeField]
    internal struct TPool
    {
        [SerializeField] public Type Type;
        [SerializeField] public Queue<IGridEntity> TQueue;
    }
    [SerializeField] private List<TPool> lspooledEntities;

    private Dictionary<string, Queue<IGridEntity>> entityPools;

    private void Start()
    {
        CreatePools();

    }

    void CreatePools()
    {
        lspooledEntities = new List<TPool>();

        entityPools = new();

        for (int i = 0; i < pools.Length; i++)
        {
            pools[i].pooledObjects = new Queue<PoolObject>();

            for (int j = 0; j < pools[i].poolSize; j++)
            {
                PoolObject _po = Instantiate(pools[i].objectPrefab, transform);
                _po.OnCreated();
                pools[i].pooledObjects.Enqueue(_po);

                if (_po is IGridEntity gridEntity)
                {
                    string entityName = (gridEntity as PoolObject).PoolObjectName;

                    if (!entityPools.ContainsKey(entityName))
                    {
                        entityPools[entityName] = new Queue<IGridEntity>(); // Create the queue if it doesn't exist
                    }

                   
                    entityPools[entityName].Enqueue(gridEntity); // Add the gridEntity to the corresponding queue
                    _po.gameObject.SetActive(false);
                }
            }
        }
        GameManager.I.CreateCurrentLevel();
    }

    public T GetObject<T>() where T : PoolObject
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (typeof(T) == pools[i].objectPrefab.GetType())
            {
                if (pools[i].pooledObjects.Count == 0)
                {
                    PoolObject _po = Instantiate(pools[i].objectPrefab, transform);
                    _po.OnCreated();
                    EnqueObject(_po);
                    return GetObject<T>();
                }
                else
                {

                    T t = pools[i].pooledObjects.Dequeue() as T;
                    t.OnSpawn();

                    return t;
                }
            }

        }
        return default;
    }

    public U ResolveFunc<T, U>(string entityName) where T : class, IGridEntity where U : PoolObject, T
    {
        if (entityPools.ContainsKey(entityName) && entityPools[entityName].Count > 0)
        {
            T entity = entityPools[entityName].Dequeue() as T;
            (entity as PoolObject)?.OnSpawn();
            return entity as U;
        }

        Debug.LogWarning("Pool is empty for entity with name: " + entityName);
        return null;
    }

    public TInterface Resolve<TInterface>(string entityName) where TInterface : class, IGridEntity
    {
        if (entityPools.ContainsKey(entityName) && entityPools[entityName].Count > 0)
        {
            TInterface entity = entityPools[entityName].Dequeue() as TInterface;
            (entity as PoolObject)?.OnSpawn();
            return entity;
        }

        Debug.LogWarning("Pool is empty for entity with name: " + entityName);
        return null;
    }

    public void EnqueObject<T>(T po) where T : PoolObject
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (po.GetType() == pools[i].objectPrefab.GetType())
            {
                bool isFound = false;

                // Kuyruktan elemanları çıkar ve liste olarak sakla
                List<PoolObject> pooledObjectsList = new List<PoolObject>();
                while (pools[i].pooledObjects.Count > 0)
                {
                    PoolObject pooledObject = pools[i].pooledObjects.Dequeue();
                    if (pooledObject == po)
                    {
                        isFound = true;
                    }
                    pooledObjectsList.Add(pooledObject);
                }

                // Elemanı ekle
                if (!isFound)
                {
                    pooledObjectsList.Add(po);
                }

                // Listeyi kuyruğa geri ekle
                foreach (var pooledObject in pooledObjectsList)
                {
                    pools[i].pooledObjects.Enqueue(pooledObject);
                }

                break;
            }
        }
    }
}




