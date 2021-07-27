using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class UnitySingleton<T> : MonoBehaviour
where T : MonoBehaviour
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance = null)
                {
                    GameObject obj = new GameObject();
                    _instance = (T)obj.AddComponent(typeof(T));
                    obj.hideFlags = HideFlags.DontSave;
                    obj.name = typeof(T).Name;
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
    public static bool OnInit()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(T)) as T;
            if (_instance = null)
            {
                GameObject obj = new GameObject();
                _instance = (T)obj.AddComponent(typeof(T));
                obj.hideFlags = HideFlags.DontSave;
                obj.name = typeof(T).Name;
            }
            return true;
        }
        else
        {
            TDebug.LogError("HAD INIT");
            return true;
        }
    }
}
