using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity自动单例模式,自动实例化自己
/// </summary>
/// <typeparam name="T"></typeparam>
public class UnityAutoSingleton<T> : MonoBehaviour
where T : MonoBehaviour
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                _instance = obj.AddComponent<T>();
                //obj.hideFlags = HideFlags.DontSave;
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
            GameObject obj = new GameObject();
            obj.name = typeof(T).ToString();
            _instance = obj.AddComponent<T>();
            return true;
        }
        else
        {
            TDebug.LogError("HAD INIT");
            return true;
        }
    }
}
