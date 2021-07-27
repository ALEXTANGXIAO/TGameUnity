using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

sealed partial class GameApp
{
    public void InitLog()
    {
        TDebug.SetLogHandler(new GameLogHandler());
    }

    /// <summary>
    /// 设置帧率
    /// </summary>
    private void SetTargetFrameRate()
    {
        Application.targetFrameRate = TargetFrameRate;
    }

    #region 生命周期
    public void Start()
    {
        GameTime.StartFrame();
        var listLogic = m_listLogicMgr;
        var logicCnt = listLogic.Count;
        for (int i = 0; i < logicCnt; i++)
        {
            var logic = listLogic[i];
            logic.OnStart();
        }
    }

    public void Update()
    {
        GameTime.StartFrame();
        GameClient.Instance.Update();
        var listLogic = m_listLogicMgr;
        var logicCnt = listLogic.Count;
        for (int i = 0; i < logicCnt; i++)
        {
            var logic = listLogic[i];
            logic.OnUpdate();
        }
    }

    public void LateUpdate()
    {
        GameTime.StartFrame();
        var listLogic = m_listLogicMgr;
        var logicCnt = listLogic.Count;
        for (int i = 0; i < logicCnt; i++)
        {
            var logic = listLogic[i];
            logic.OnLateUpdate();
        }
    }

    public void OnPause()
    {
        GameTime.StartFrame();
        for (int i = 0; i < m_listLogicMgr.Count; i++)
        {
            var logicSys = m_listLogicMgr[i];
            logicSys.OnPause();
        }
    }

    public void OnResume()
    {
        GameTime.StartFrame();
        for (int i = 0; i < m_listLogicMgr.Count; i++)
        {
            var logicSys = m_listLogicMgr[i];
            logicSys.OnResume();
        }
    }

    public void OnDestroy()
    {
        GameTime.StartFrame();
        GameClient.Instance.OnDestroy();
        for (int i = 0; i < m_listLogicMgr.Count; i++)
        {
            var logicSys = m_listLogicMgr[i];
            logicSys.OnDestroy();
        }
    }

    #endregion

    #region 系统注册
    //-------------------------------------------------------系统注册--------------------------------------------------------//
    private List<ILogicSys> m_listLogicMgr = new List<ILogicSys>();

    private void InitLibImp()
    {
        TJson.RegistImp(new JsonImp());
        DHttpRequest.RegistImp(new HttpRequestImp());
    }

    private void RegistAllSystem()
    {
        EventCenter.OnInit();
        InputManager.OnInit();
        GameClient.OnInit();
        CameraMgr.Instance.Awake();
        AddLogicSys(BehaviourSingleSystem.Instance);
        AddLogicSys(DataCenterSys.Instance);
        AddLogicSys(UISys.Instance);
        AddLogicSys(BubbleMgr.Instance);
        AddLogicSys(ActorNameMgr.Instance);
        AddLogicSys(AudioSys.Instance);
    }

    private void InitAllConfig()
    {
        UISpriteHelper.Instance.ReadConfig();
    }

    protected bool AddLogicSys(ILogicSys logicSys)
    {
        if (m_listLogicMgr.Contains(logicSys))
        {
            Debug.Log("Repeat add logic system: " + logicSys.GetType().Name);
            return false;
        }

        if (!logicSys.OnInit())
        {
            Debug.Log(" Init failed " + logicSys.GetType().Name);
            return false;
        }

        m_listLogicMgr.Add(logicSys);

        return true;
    }
    #endregion
}
