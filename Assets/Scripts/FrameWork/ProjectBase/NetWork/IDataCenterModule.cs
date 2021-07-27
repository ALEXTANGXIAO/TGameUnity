﻿using UnityEngine;
using System.Collections;

interface IDataCenterModule
{
    void Init();

    void OnRoleLogout();

    void OnUpdate();

    void OnMainPlayerMapChange();
}
public class DataCenterModule<T> : IDataCenterModule where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new T();
                //DLogger.Assert(instance != null);
            }
            return instance;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void OnRoleLogout()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnMainPlayerMapChange()
    {

    }
}
