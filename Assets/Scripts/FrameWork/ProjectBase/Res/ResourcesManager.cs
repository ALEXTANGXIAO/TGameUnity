using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public static void CollectGC()
    {
        GC.Collect();
    }
    /// <summary>
    /// 异步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="p"></param>
    public T LoadAsync<T>(string name, UnityAction<T> callback = null) where T : UnityEngine.Object
    {
        T res = null;

        MonoManager.Instance.StartCoroutine(ReallyLoadAsync(name,callback));

        return res;
    }

    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback = null) where T : UnityEngine.Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);

        yield return r;

        if(r.asset is GameObject)
        {
            callback(GameObject.Instantiate(r.asset) as T);
        }
        else
        {
            callback(r.asset as T);
        }
    }

    public GameObject AllocGameObject(string name)
    {
        GameObject res = Resources.Load<GameObject>(name);

        if (res == null)
        {
            return null;
        }

        var go = GameObject.Instantiate(res);

        go.name = res.name;

        return go;
    }

    /// <summary>
    /// 同步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T Load<T>(string name) where T: UnityEngine.Object
    {
        T res = Resources.Load<T>(name);

        if(res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }

    public UnityEngine.Object Load(string name)
    {
        var res = Resources.Load(name);
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }

    public UnityEngine.Object Load(string path,string name)
    {
        var res = Resources.Load(path + "/"+name);
        if (res is GameObject)
        {
            res.name = name;
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }
}
