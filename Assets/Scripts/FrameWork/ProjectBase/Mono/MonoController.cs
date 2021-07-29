using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mono管理者
/// </summary>
public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if(updateEvent != null)
        {
            updateEvent();
        }
    }

    void FixedUpdate()
    {
        if (fixedUpdateEvent != null)
        {
            fixedUpdateEvent();
        }
    }

    public void AddFixedUpdateListener(UnityAction fun)
    {
        fixedUpdateEvent += fun;
    }

    public void RemoveFixedUpdateListener(UnityAction fun)
    {
        fixedUpdateEvent -= fun;
    }

    /// <summary>
    /// 为给外部提供的 添加帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    /// <summary>
    /// 移除帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }


    public void m_DontDestroyOnLoad(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        DontDestroyOnLoad(go);
    }
}
