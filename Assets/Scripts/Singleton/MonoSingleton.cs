using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T>:MonoBehaviour where T:MonoBehaviour
{
    public static T I = null;

    private void Awake()
    {
        if (I != null) return;

        I = this as T;
    }
}
