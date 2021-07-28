using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

internal interface IEventInfo
{

}

public class EventInfo<T>:IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo<T,U> : IEventInfo
{
    public UnityAction<T, U> actions;

    public EventInfo(UnityAction<T, U> action)
    {
        actions += action;
    }
}

public class EventInfo<T, U, W> : IEventInfo
{
    public UnityAction<T, U, W> actions;

    public EventInfo(UnityAction<T, U, W> action)
    {
        actions += action;
    }
}

public class EventInfo: IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件中心
/// </summary>
public class EventCenter : Singleton<EventCenter>
{
    //Key    --- 事件名称
    //Value  --- 监听这个事件对应函数的委托
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    //Key    --- 事件名称
    //Value  --- 监听这个事件对应函数的委托
    /* 事件中心的优化
     * Dictionary应该都是Hash算法，其时间复杂度接近O(1)Hash算法也就是将Int或者String映射到实际哈希表里面的下标中，
     * 去取对应的数据。如果说速度影响的话，应该是Int是通过某种函数计算出Hash值，速度较快，而String将会遍历每个字符，
     * 然后通过算法来计算哈希值，并防止碰撞，所以用int事件id来代替string减少计算哈希值的过程达到优化事件中心
     */
    private Dictionary<int, IEventInfo> m_eventDic = new Dictionary<int, IEventInfo>();

    //---------------------------------------------------------------------------------------------------------------------------//

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddEventListener<T>(string name,UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener<T>(string name,UnityAction<T> action)
    {
        if (action == null)
        {
            return;
        }

        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }

    public void RemoveEventListener(string name, UnityAction action)
    {
        if (action == null)
        {
            return;
        }

        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">触发事件的名字</param>
    public void EventTrigger<T>(string name,T info)
    {
        if (eventDic.ContainsKey(name))
        {
            //直接执行委托eventDic[name]();
            if ((eventDic[name] as EventInfo<T>).actions != null){
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }

    /// <summary>
    /// 事件触发 无参
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            //直接执行委托eventDic[name]();
            if ((eventDic[name] as EventInfo).actions != null)
            {
                (eventDic[name] as EventInfo).actions.Invoke();
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------//
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddEventListener<T>(int eventid, UnityAction<T> action)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            (m_eventDic[eventid] as EventInfo<T>).actions += action;
        }
        else
        {
            m_eventDic.Add(eventid, new EventInfo<T>(action));
        }
    }

    public void AddEventListener<T,U>(int eventid, UnityAction<T,U> action)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            (m_eventDic[eventid] as EventInfo<T,U>).actions += action;
        }
        else
        {
            m_eventDic.Add(eventid, new EventInfo<T,U>(action));
        }
    }

    public void AddEventListener<T, U, W>(int eventid, UnityAction<T, U, W> action)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            (m_eventDic[eventid] as EventInfo<T, U, W>).actions += action;
        }
        else
        {
            m_eventDic.Add(eventid, new EventInfo<T, U , W>(action));
        }
    }

    public void AddEventListener(int eventid, UnityAction action)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            (m_eventDic[eventid] as EventInfo).actions += action;
        }
        else
        {
            m_eventDic.Add(eventid, new EventInfo(action));
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener<T>(int eventid, UnityAction<T> action)
    {
        if (action == null)
        {
            return;
        }

        if (m_eventDic.ContainsKey(eventid))
        {
            (m_eventDic[eventid] as EventInfo<T>).actions -= action;
        }
    }

    public void RemoveEventListener(int eventid, UnityAction action)
    {
        if (action == null)
        {
            return;
        }

        if (m_eventDic.ContainsKey(eventid))
        {
            (m_eventDic[eventid] as EventInfo).actions -= action;
        }
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">触发事件的名字</param>
    public void EventTrigger<T>(int eventid, T info)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            //直接执行委托eventDic[name]();
            if ((m_eventDic[eventid] as EventInfo<T>).actions != null)
            {
                (m_eventDic[eventid] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }

    public void EventTrigger<T,U>(int eventid, T info,U info2)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            //直接执行委托eventDic[name]();
            if ((m_eventDic[eventid] as EventInfo<T,U>).actions != null)
            {
                (m_eventDic[eventid] as EventInfo<T,U>).actions.Invoke(info,info2);
            }
        }
    }

    public void EventTrigger<T, U , W>(int eventid, T info, U info2, W info3)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            //直接执行委托eventDic[name]();
            if ((m_eventDic[eventid] as EventInfo<T, U , W>).actions != null)
            {
                (m_eventDic[eventid] as EventInfo<T, U , W>).actions.Invoke(info, info2, info3);
            }
        }
    }

    /// <summary>
    /// 事件触发 无参
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger(int eventid)
    {
        if (m_eventDic.ContainsKey(eventid))
        {
            //直接执行委托eventDic[name]();
            if ((m_eventDic[eventid] as EventInfo).actions != null)
            {
                (m_eventDic[eventid] as EventInfo).actions.Invoke();
            }
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------//


    /// <summary>
    /// 清除事件中心
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
