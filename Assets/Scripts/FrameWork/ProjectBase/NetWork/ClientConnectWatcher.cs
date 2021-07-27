using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ClientConnectWatcherStatus
{
    StatusInit,
    StatusReconnectAuto,
    StatusReconnectConfirm,
    StatusWaitExit
}

public class ClientConnectWatcher
{
    private GameClient m_client;
    private ClientConnectWatcherStatus m_status = ClientConnectWatcherStatus.StatusInit;
    private float m_statusTime;
    private int m_reconnetCnt = 0;
    private int m_disconnectReason = 0;

    private bool m_enable = false;
    public bool Enable
    {
        get { return m_enable; }
        set
        {
            if (m_enable != value)
            {
                m_enable = value;
                if (m_enable)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }
    }

    public ClientConnectWatcherStatus Status
    {
        get { return m_status; }
        set
        {
            if (m_status != value)
            {
                m_status = value;
                m_statusTime = GameTime.time;
            }
        }
    }

    public ClientConnectWatcher(GameClient client)
    {
        m_client = client;
        m_statusTime = GameTime.time;
        m_status = ClientConnectWatcherStatus.StatusInit;
    }

    public void Update()
    {
        if (!m_enable)
        {
            return;
        }

        if (m_client.Status == GameClientStatus.StatusConnect)
        {
            return;
        }

        switch (m_status)
        {
            case ClientConnectWatcherStatus.StatusInit:
                UpdateOnInitStatus();
                break;
            case ClientConnectWatcherStatus.StatusReconnectAuto:
                UpdateOnReconnectAuto();
                break;
            case ClientConnectWatcherStatus.StatusReconnectConfirm:
                UpdateOnReconnectConfirm();
                break;
            case ClientConnectWatcherStatus.StatusWaitExit:
                UpdateOnWaitExit();
                break;
            default:
                break;
        }
    }


    void UpdateOnInitStatus()
    {
        if (m_reconnetCnt <= 2)
        {
            if (m_reconnetCnt == 0)
            {
                //m_disconnectReason = m_client.LastNetErrCode;
            }

            Status = ClientConnectWatcherStatus.StatusReconnectAuto;
            m_reconnetCnt++;

            TDebug.LogError("ClientConnectWatcher Reconnect !!!! Count :" + m_reconnetCnt);
            ///重连
            m_client.Reconnect();
        }
        else
        {
            TDebug.LogError("ClientConnectWatcher Reconnect Failed !!!!" + m_reconnetCnt);
            Status = ClientConnectWatcherStatus.StatusReconnectConfirm;
            m_reconnetCnt++;

            //var window = UISys.Mgr.ShowWindow<Tip_NetDisconn>();
            //window.SetErrCode(m_disconnectReason);
        }
    }

    void UpdateOnReconnectAuto()
    {
        if (m_client.Status == GameClientStatus.StatusConnect)
        {
            Status = ClientConnectWatcherStatus.StatusInit;
            m_reconnetCnt = 0;
            return;
        }

        float nowTime = GameTime.time;
        if (m_statusTime + 5 < nowTime)
        {
            ///切换到默认的，下一帧继续判断是否需要自动还是手动
            Status = ClientConnectWatcherStatus.StatusInit;
            return;
        }
    }

    void UpdateOnReconnectConfirm()
    {
        ///do nothing
    }

    void UpdateOnWaitExit()
    {
    }

    private void OnDisable()
    {
        Status = ClientConnectWatcherStatus.StatusInit;
        m_reconnetCnt = 0;
    }

    private void OnEnable()
    {
        Status = ClientConnectWatcherStatus.StatusInit;
        m_reconnetCnt = 0;
    }
}
