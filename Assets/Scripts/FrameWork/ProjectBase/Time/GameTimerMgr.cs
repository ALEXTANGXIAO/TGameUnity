using System;
using System.Collections.Generic;
using System.Diagnostics;


enum GameTimerType
{
    TimerTime = 0,  //受pause影响
    TimerFrameUpdate,
    TimerFrameLateUpdate,
    TimerFrameOnceUpdate,
    TimerUnscaledTime,  //不受pausse影响
}
class GameTimer
{
    public GameTimer m_next;
    public GameTimer m_prev;

    /// <summary>
    /// 调试用
    /// </summary>
    public string m_sourceName;
    public Action m_callAction = null;
    public bool m_destroyed = false;

    public bool m_loop;
    public float m_interval;
    public float m_triggerTime;
    public bool m_inExeQueue = false;
    public GameTimerType m_type;

#if DOD_DEBUG
    public GameTimerList m_ownList;
#endif

    public static bool IsNull(GameTimer timer)
    {
        return timer == null || timer.m_destroyed;
    }
}

class GameTimerList
{
    public GameTimer m_head;
    public GameTimer m_tail;

    public int m_count = 0;

    public bool IsEmpty
    {
        get { return m_head == null; }
    }

    public void AddTail(GameTimer node)
    {
        var tail = m_tail;
        if (tail != null)
        {
            tail.m_next = node;
            node.m_prev = tail;
        }
        else
        {
            m_head = node;
        }

        m_tail = node;
        AddCount();
    }


    [Conditional("DOD_DEBUG")]
    private void CheckListSize()
    {
#if DOD_DEBUG
        int count = 0;
        var node = m_head;
        while (node != null)
        {
            count++;
            node = node.m_next;
        }

        if (count != m_count)
        {
            DLogger.Assert(count == m_count);
        }
#endif
    }

    private void AddCount()
    {
        m_count++;
        CheckListSize();
    }

    public void DecCount(int sub = 1)
    {
        m_count -= sub;
    }

    public void AddSorted(GameTimer node, float triggerTime)
    {
        node.m_triggerTime = triggerTime;

        var head = m_head;
        while (head != null)
        {
            if (head.m_triggerTime >= triggerTime)
            {
                break;
            }
            head = head.m_next;
        }

        if (head != null)
        {
#if DOD_DEBUG
            DLogger.Assert(node.m_ownList == null);
            node.m_ownList = this;
#endif

            var prev = head.m_prev;
            if (prev != null)
            {
                prev.m_next = node;
            }
            node.m_prev = prev;

            node.m_next = head;
            head.m_prev = node;

            //如果是第一个，那么设置为头结点
            if (prev == null)
            {
                m_head = node;
            }

            AddCount();
        }
        else
        {
            AddTail(node);
        }
    }

    public void Remove(GameTimer node)
    {
#if DOD_DEBUG
        DLogger.Assert(node.m_ownList == this);
        node.m_ownList = null;
#endif

        var prev = node.m_prev;
        var next = node.m_next;

        if (prev != null)
        {
            prev.m_next = next;
        }

        if (next != null)
        {
            next.m_prev = prev;
        }

        node.m_next = null;
        node.m_prev = null;

        if (m_head == node)
        {
            m_head = next;
        }

        if (m_tail == node)
        {
            m_tail = prev;
        }

        DecCount();
    }

    public void Clear()
    {
        m_head = null;
        m_tail = null;
        m_count = 0;
    }
}

class GameTimerMgr : BehaviourSingleton<GameTimerMgr>
{
    private GameTimerList m_runningList = new GameTimerList();
    private GameTimerList m_frameUpdateList = new GameTimerList();
    private GameTimerList m_frameLateUpdateList = new GameTimerList();
    private GameTimerList m_frameOnceUpdateList = new GameTimerList();
    private GameTimerList m_unscaleRunningList = new GameTimerList();

    public override void Awake()
    {
    }

    public void PrintDebug()
    {
        TDebug.Log("timer count: {0} updaet count:{1}, late update count:{2}, once update count:{3}",
                m_runningList.m_count, m_frameUpdateList.m_count, m_frameLateUpdateList.m_count,
                m_frameOnceUpdateList.m_count);

        TDebug.Log("------------------Update---------------------");
        PrintTimerListStatic(m_frameUpdateList);

        TDebug.Log("------------------LateUpdate---------------------");
        PrintTimerListStatic(m_frameLateUpdateList);

        TDebug.Log("------------------Running---------------------");
        PrintTimerListStatic(m_runningList);

        TDebug.Log("------------------Unscale Running---------------------");
        PrintTimerListStatic(m_unscaleRunningList);
    }


    #region 创建定时器接口

    public bool CreateLoopTimer(ref GameTimer result, string source, float interval, Action timerAction)
    {
        if (!GameTimer.IsNull(result))
        {
            return false;
        }

        result = DoCreateLoopTimer(source, interval, timerAction);
        return true;
    }

    private GameTimer DoCreateLoopTimer(string source, float interval, Action timerAction)
    {
        interval = Math.Max(interval, 0.001f);
        var timer = AllocTimer(source, timerAction);
        timer.m_type = GameTimerType.TimerTime;
        timer.m_loop = true;
        timer.m_interval = interval;
        var triggerTime = GameTime.time + interval;
        m_runningList.AddSorted(timer, triggerTime);
        return timer;
    }

    public GameTimer CreateOnceTimer(string source, float elapse, Action timerAction)
    {
        var timer = AllocTimer(source, timerAction);
        timer.m_type = GameTimerType.TimerTime;
        var triggerTime = GameTime.time + elapse;
        m_runningList.AddSorted(timer, triggerTime);
        return timer;
    }

    /// <summary>
    /// 修改定时器循环创建接口，改为更加安全的方式
    /// </summary>
    /// <param name="result"></param>
    /// <param name="source"></param>
    /// <param name="timerAction"></param>
    public bool CreateLoopFrameTimer(ref GameTimer result, string source, Action timerAction)
    {
        if (!GameTimer.IsNull(result))
        {
            return false;
        }

        result = DoCreateLoopFrameTimer(source, timerAction);
        return true;
    }

    private GameTimer DoCreateLoopFrameTimer(string source, Action timerAction)
    {
        var timer = AllocTimer(source, timerAction);
        timer.m_type = GameTimerType.TimerFrameUpdate;
        m_frameUpdateList.AddTail(timer);
        return timer;
    }

    public GameTimer CreateOnceFrameTimer(string source, Action timerAction)
    {
        var timer = AllocTimer(source, timerAction);
        timer.m_type = GameTimerType.TimerFrameOnceUpdate;
        m_frameOnceUpdateList.AddTail(timer);
        return timer;
    }

    public bool CreateLoopFrameLateTimer(ref GameTimer result, string source, Action timerAction)
    {
        if (!GameTimer.IsNull(result))
        {
            return false;
        }

        result = DoCreateLoopFrameLateTimer(source, timerAction);
        return true;
    }

    private GameTimer DoCreateLoopFrameLateTimer(string source, Action timerAction)
    {
        var timer = AllocTimer(source, timerAction);
        timer.m_type = GameTimerType.TimerFrameLateUpdate;
        m_frameLateUpdateList.AddTail(timer);
        return timer;
    }

    /// <summary>
    /// 不受
    /// </summary>
    /// <param name="source"></param>
    /// <param name="interval"></param>
    /// <param name="timerAction"></param>
    /// <returns></returns>
    public GameTimer CreateUnscaleLoopTimer(string source, float interval, Action timerAction)
    {
        interval = Math.Max(interval, 0.001f);
        var timer = AllocTimer(source, timerAction);
        timer.m_type = GameTimerType.TimerUnscaledTime;
        timer.m_loop = true;
        timer.m_interval = interval;
        var triggerTime = GameTime.unscaledTime + interval;
        m_unscaleRunningList.AddSorted(timer, triggerTime);
        return timer;
    }

    public GameTimer CreateUnscaleOnceTimer(string source, float elapse, Action timerAction)
    {
        var timer = AllocTimer(source, timerAction);
        timer.m_type = GameTimerType.TimerUnscaledTime;
        var triggerTime = GameTime.unscaledTime + elapse;
        m_unscaleRunningList.AddSorted(timer, triggerTime);
        return timer;
    }
    #endregion

    #region 销毁定时器接口

    public void DestroyTimer(ref GameTimer timer)
    {
        ProcessDestroyTimer(timer);
        timer = null;
    }

    private void ProcessDestroyTimer(GameTimer timer)
    {
        if (timer == null || timer.m_destroyed)
        {
            return;
        }

        DoDestroy(timer);

        //如果在执行队列里，那么什么都不做
        if (!timer.m_inExeQueue)
        {
            var type = timer.m_type;
            if (type == GameTimerType.TimerTime)
            {
                m_runningList.Remove(timer);
            }
            else if (type == GameTimerType.TimerFrameUpdate)
            {
                m_frameUpdateList.Remove(timer);
            }
            else if (type == GameTimerType.TimerFrameOnceUpdate)
            {
                m_frameOnceUpdateList.Remove(timer);
            }
            else if (type == GameTimerType.TimerUnscaledTime)
            {
                m_unscaleRunningList.Remove(timer);
            }
            else
            {
                m_frameLateUpdateList.Remove(timer);
            }
        }
        else
        {
            TDebug.Log("Free when in exuete queue");
        }
    }

    #endregion

    public override bool IsHaveLateUpdate()
    {
        return true;
    }

    public override void LateUpdate()
    {
        UpdateFrameTimer(m_frameLateUpdateList);
    }

    private int m_debugLoopTimerCount = 0;
    private int m_debugOnceTimerCount = 0;
    public override void Update()
    {
        UpdateTickTimer(false);
        UpdateTickTimer(true); ;
        UpdateFrameTimer(m_frameUpdateList);
        UpdateOnceFrameTimer(m_frameOnceUpdateList);
    }

    private void UpdateTickTimer(bool isUnscaled)
    {
        var runningList = isUnscaled ? m_unscaleRunningList : m_runningList;
        var head = runningList.m_head;
        if (head != null)
        {
            var nowTime = isUnscaled ? GameTime.unscaledTime : GameTime.time;
            var node = head;

            int delCount = 0;
            while (node != null && node.m_triggerTime <= nowTime)
            {
                node.m_inExeQueue = true;
                node = node.m_next;

                delCount++;
            }

            ///如果一个都没有，那么直接返回
            if (head == node)
            {
                return;
            }

            GameTimer waitExeHead = null;
            if (node != null)
            {
                //从该节点开始断开
                var prev = node.m_prev;
                prev.m_next = null;

                node.m_prev = null;
                runningList.m_head = node;

                runningList.DecCount(delCount);

                waitExeHead = head;
            }
            else
            {
                ///如果都要执行，那么清空运行列表
                waitExeHead = head;
                runningList.Clear();
            }

            ///然后开始执行
            node = waitExeHead;
            while (node != null)
            {
                var next = node.m_next;
                node.m_next = null;
                node.m_prev = null;
#if DOD_DEBUG
                ///纯为了配合测试和流程验证，正式环境不用麻烦
                var ownList = node.m_ownList;
                node.m_ownList = null;
#endif

                if (!node.m_destroyed)
                {
                    node.m_callAction();
                    ///如果没有被释放掉,而且是循环的
                    if (node.m_loop && !node.m_destroyed)
                    {
                        node.m_inExeQueue = false;
                        var triggerTime = node.m_interval + nowTime;
#if DOD_DEBUG
                        node.m_ownList = null;
#endif

                        runningList.AddSorted(node, triggerTime);
                    }
                    else
                    {
                        DoDestroy(node);
                    }
                }
                else
                {
                    TDebug.Log("destroy timer: {0}", node.m_sourceName);
                }

                node = next;
            }
        }
    }

    private void PrintTimerListStatic(GameTimerList list)
    {
        Dictionary<string, int> dictStat = new Dictionary<string, int>();
        var node = list.m_head;
        while (node != null)
        {
            var count = 0;
            dictStat.TryGetValue(node.m_sourceName, out count);
            count++;

            dictStat[node.m_sourceName] = count;
            node = node.m_next;
        }

        var itr = dictStat.GetEnumerator();
        while (itr.MoveNext())
        {
            TDebug.Log("{0}:{1}", itr.Current.Key, itr.Current.Value);
        }
    }

    private void UpdateFrameTimer(GameTimerList list)
    {
        var node = list.m_head;
        while (node != null)
        {
            node.m_inExeQueue = true;

            node.m_callAction();

            node.m_inExeQueue = false;

            var next = node.m_next;
            if (node.m_destroyed)
            {
                list.Remove(node);
            }

            node = next;
        }
    }

    private void UpdateOnceFrameTimer(GameTimerList list)
    {
        var node = list.m_head;
        list.Clear();

        while (node != null)
        {
            node.m_inExeQueue = true;
            node.m_callAction();
            var next = node.m_next;
            DoDestroy(node);
            node = next;
        }
    }


    private void DoDestroy(GameTimer timer)
    {
        timer.m_destroyed = true;
        timer.m_callAction = null;
    }

    private GameTimer AllocTimer(string source, Action timerAction)
    {
        var freeHead = new GameTimer();
        freeHead.m_callAction = timerAction;
        freeHead.m_sourceName = source;
        return freeHead;
    }
}
