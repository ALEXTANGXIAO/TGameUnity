using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 抽屉数据，池子中的一列容器
/// </summary>
public class PoolData
{
    public GameObject fatherObject;
    
    public List<GameObject> poolList;

    public PoolData(GameObject obj,GameObject poolObject)
    {
        fatherObject = new GameObject(obj.name);

        fatherObject.transform.parent = poolObject.transform;

        poolList = new List<GameObject>() { };

        PushObject(obj);
    }

    public GameObject GetObject()
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);
        obj.SetActive(true);
        obj.transform.parent = null;
        return obj;
    }

    /// <summary>
    /// 存放缓存池
    /// </summary>
    /// <param name="obj"></param>
    public void PushObject(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.parent = fatherObject.transform;
    }
}

public class PoolManager : Singleton<PoolManager>
{
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObject;

    /// <summary>
    /// 获取GameObject
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetObject(string name,UnityAction<GameObject> callback)
    {
        GameObject obj = null;
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            obj = poolDic[name].GetObject();
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
        }
        return obj;
    }

    /// <summary>
    /// 异步获取GameObject
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void GetObjectAsync(string name, UnityAction<GameObject> callback)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callback(poolDic[name].GetObject());
        }
        else
        {
            //异步加载资源 创建对象给外部用
            ResourcesManager.Instance.LoadAsync<GameObject>(name, (obj) =>
            {
                obj.name = name;
                callback(obj);
            });
        }
    }

    /// <summary>
    /// 存放进缓存池
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public void PushObject(string name, GameObject obj)
    {
        if (poolObject == null)
        {
            poolObject = new GameObject("Pool");
        }

        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObject(obj);
        }
        else
        {
            poolDic.Add(name, new PoolData(obj,poolObject));
        }
    }

    /// <summary>
    /// 清空缓存池方法，主要用于切换场景
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObject = null;
    }
}
