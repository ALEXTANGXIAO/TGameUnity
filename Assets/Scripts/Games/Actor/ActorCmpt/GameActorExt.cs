using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public partial class GameActor
{
    private List<ActorCmpt> m_listCmpt = new List<ActorCmpt>();
    private Dictionary<string, ActorCmpt> m_mapCmpt = new Dictionary<string, ActorCmpt>();

    public T AddCmpt<T>(bool respawn = true) where T : ActorCmpt, new()
    {
        T cmpt = GetCmpt<T>();
        if (cmpt != null)
        {
            return cmpt;
        }

        cmpt = new T();
        if (!AddCmpt_Imp(cmpt))
        {
            TDebug.LogError("AddComponent failed, Component name: {0}" + GetClassName(typeof(T)));
            cmpt = null;
            return null;
        }
        return cmpt;
    }

    private bool AddCmpt_Imp<T>(T cmpt) where T : ActorCmpt
    {
        //判断是否已经存在
        if (!cmpt.BeforeAddToActor(this))
        {
            return false;
        }

        m_listCmpt.Add(cmpt);
        m_mapCmpt[GetClassName(typeof(T))] = cmpt;
        return true;
    }

    public void RemoveCmpt<T>() where T : ActorCmpt
    {
        string className = GetClassName(typeof(T));
        ActorCmpt cmpt;
        if (m_mapCmpt.TryGetValue(className, out cmpt))
        {
            //Event.RemoveAllListenerByOwner(cmpt);
            m_mapCmpt.Remove(className);
            m_listCmpt.Remove(cmpt);
        }
    }

    private string GetClassName(Type type)
    {
        return type.FullName;
    }

    public T GetCmpt<T>() where T : ActorCmpt
    {
        ActorCmpt cmpt;
        if (m_mapCmpt.TryGetValue(GetClassName(typeof(T)), out cmpt))
        {
            return cmpt as T;
        }
        return null;
    }

    private void DestroyAllCmpt()
    {
        var listCmpt = m_listCmpt;
        for (int i = listCmpt.Count - 1; i >= 0; i--)
        {
            listCmpt[i] = null;
        }

        m_listCmpt.Clear();
        m_mapCmpt.Clear();
    }
}