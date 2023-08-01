using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    public string PoolObjectName;
    public abstract void OnCreated();
    public abstract void OnDeactive();
    public abstract void OnSpawn();
}
