using System.Collections.Generic;

class BaseBehaviourSingleton
{
    public bool m_isStart = false;

    public virtual void Awake()
    {
    }

    public virtual bool IsHaveLateUpdate()
    {
        return false;
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void LateUpdate()
    {
    }

    public virtual void Destroy()
    {
    }

    public virtual void OnPause()
    {
    }

    public virtual void OnResume()
    {
    }
}

class BehaviourSingleton<T> : BaseBehaviourSingleton where T : BaseBehaviourSingleton, new()
{
    private static T sInstance;
    public static T Instance
    {
        get
        {
            if (null == sInstance)
            {
                sInstance = new T();

                sInstance.Awake();
                RegSingleton(sInstance);
            }

            return sInstance;
        }
    }

    /// <summary>
    /// 注册单例，单例继承自BaseLogicSys
    /// </summary>
    /// <param name="inst"></param>
    private static void RegSingleton(BaseBehaviourSingleton inst)
    {
        BehaviourSingleSystem.Instance.RegSingleton(inst);
    }
}

class BehaviourSingleSystem : BaseLogicSys<BehaviourSingleSystem>
{
    List<BaseBehaviourSingleton> m_listInst = new List<BaseBehaviourSingleton>();
    List<BaseBehaviourSingleton> m_listStart = new List<BaseBehaviourSingleton>();
    List<BaseBehaviourSingleton> m_listUpdate = new List<BaseBehaviourSingleton>();
    List<BaseBehaviourSingleton> m_listLateUpdate = new List<BaseBehaviourSingleton>();

    public void RegSingleton(BaseBehaviourSingleton inst)
    {
        m_listInst.Add(inst);
        m_listStart.Add(inst);
    }

    public override void OnUpdate()
    {
        var listStart = m_listStart;

        var listToUpdate = m_listUpdate;
        var listToLateUpdate = m_listLateUpdate;

        if (listStart.Count > 0)
        {
            for (int i = 0; i < listStart.Count; i++)
            {
                var inst = listStart[i];

                inst.m_isStart = true;
                inst.Start();
                listToUpdate.Add(inst);

                if (inst.IsHaveLateUpdate())
                {
                    listToLateUpdate.Add(inst);
                }
            }

            listStart.Clear();
        }

        var listUpdateCnt = listToUpdate.Count;
        for (int i = 0; i < listUpdateCnt; i++)
        {
            var inst = listToUpdate[i];

            inst.Update();
        }
    }

    public override void OnLateUpdate()
    {
        var listLateUpdate = m_listLateUpdate;
        var listLateUpdateCnt = listLateUpdate.Count;
        for (int i = 0; i < listLateUpdateCnt; i++)
        {
            var inst = listLateUpdate[i];

            inst.LateUpdate();
        }
    }

    public override void OnDestroy()
    {
        for (int i = 0; i < m_listInst.Count; i++)
        {
            var inst = m_listInst[i];
            inst.Destroy();
        }
    }

    public override void OnPause()
    {
        for (int i = 0; i < m_listInst.Count; i++)
        {
            var inst = m_listInst[i];
            inst.OnPause();
        }
    }

    public override void OnResume()
    {
        for (int i = 0; i < m_listInst.Count; i++)
        {
            var inst = m_listInst[i];
            inst.OnResume();
        }
    }
}