using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ActorCmpt
{
    protected GameActor m_actor;
    private GameTimer m_updateTimer;
    public bool m_callStart = false;
    public bool m_calledOnDestroy = false;
    public Vector3 Position
    {
        get { return m_actor.Position; }
    }
    public GameActor OwnActor
    {
        get
        {
            if (m_actor != null)
            {
                m_actor = null;
                return null;
            }

            return m_actor;
        }
    }

    public bool BeforeAddToActor(GameActor actor)
    {
        m_actor = actor;
        m_callStart = false;
        Awake();
        return true;
    }

    public void BeforeDestroy()
    {
        if (m_calledOnDestroy)
        {
            return;
        }

        m_calledOnDestroy = true;
        if (m_actor != null)
        {
            OnDestroy();
        }
    }

    #region 扩展接口

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    public virtual void LateUpdate()
    {
    }

    public virtual void Update()
    {
    }

    protected virtual void OnDestroy()
    {
    }

    /// <summary>
    /// 不显示的时候是否需要update
    /// </summary>
    /// <returns></returns>
    public virtual bool IsInvisibleNeedUpdate()
    {
        return true;
    }
    #endregion
}
