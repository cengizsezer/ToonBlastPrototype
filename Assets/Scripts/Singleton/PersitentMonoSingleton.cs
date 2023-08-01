using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersitentMonoSingleton<T> : MonoBehaviour where T:MonoBehaviour
{
    protected static T i;
    protected static bool _isDestroyedByGameQuit = false;
    public static T I
    {
        get
        {
            if (_isDestroyedByGameQuit) return null;
            if (i != null) return i;

            i = FindObjectOfType<T>();
            if (i == null)
                i = new GameObject(typeof(T) + " Instance").AddComponent<T>();

            return i;
        }
    }
   
    protected virtual void Awake()
    {
        if (i != null)
        {
            Debug.Log($"[{this.GetType().Name}]: Not null, destroy");
            Destroy(gameObject);
            return;
        }
        i = this as T;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (i == this) _isDestroyedByGameQuit = true;
    }
   
}
