using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SocketGameProtocol;
using UnityEngine;

public enum GameClientStatus
{
    StatusInit,         //初始化
    StatusReconnect,    //重新连接
    StatusClose,        //断开连接
    StatusConnect,        //连接中
    StatusEnter,        //AccountLogin成功
}

public delegate void CSMsgDelegate(MainPack mainPack);

public class GameClient : Singleton<GameClient>
{
    private GameClientStatus m_status = GameClientStatus.StatusInit;
    public GameClientStatus Status
    {
        get { return m_status; }
        set { m_status = value; }
    }

    private string m_Host = GameApp.Instance.Host; ///"1.14.132.143";//106.52.118.65//127.0.0.1//1.14.132.143//
    private int m_Port = 54809;

    private Socket socket;
    private Message message;
    private Thread aucThread;
    private int connTimeout;
    private int connRetryMaxNum;
    private ClientConnectWatcher m_connectWatcher;

    Dictionary<int, List<CSMsgDelegate>> m_mapCmdHandle = new Dictionary<int, List<CSMsgDelegate>>();

    public GameClient()
    {
        m_connectWatcher = new ClientConnectWatcher(this);
        SetWatchReconnect(true);
        message = new Message();
        Connect();
        ConnectUdp();
        ResigterEvent();
    }

    public void ResigterEvent()
    {
        EventCenter.Instance.AddEventListener<MainPack>(NetEvent.HeartBeat, Send);
        EventCenter.Instance.AddEventListener(NetEvent.ConnectTcp, Connect);
        EventCenter.Instance.AddEventListener(NetEvent.ConnectUdp, ConnectUdp);
    }

    public bool Connect(string host, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        TDebug.Warning("start connect server[{0}:{1}]...", host, port);
        Status = GameClientStatus.StatusInit;
        try
        {
            socket.Connect(host, port);
            StartReceive();
            Status = GameClientStatus.StatusConnect;
        }
        catch (Exception e)
        {
            TDebug.LogError(e);
            TDebug.Error("socket connect {0}:{1} failed", host, port);
            ChangeStateOnEnterFail();
            return false;
        }
        m_Host = host;
        m_Port = port;
        return true;
    }

    public void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Status = GameClientStatus.StatusInit;
        try
        {
            TDebug.Warning("start connect server[{0}:{1}]...", m_Host, m_Port);
            socket.Connect(m_Host, m_Port);
            StartReceive();
            Status = GameClientStatus.StatusConnect;
        }
        catch (Exception e)
        {
            TDebug.Error("socket connect {0}:{1} failed!!!", m_Host, m_Port);
            TDebug.LogError(e);
            ChangeStateOnEnterFail();
        }
    }


    void StartReceive()
    {
        socket.BeginReceive(message.Buffer, message.StartIndex, message.Remsize, SocketFlags.None, ReceiveCallback, null);
    }

    private void CloseSocket()
    {
        if (socket.Connected && socket != null)
        {
            socket.Close();
        }
        m_status = GameClientStatus.StatusInit;
        m_connectWatcher.Status = ClientConnectWatcherStatus.StatusInit;
    }


    void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            if (socket == null || socket.Connected == false)
            {
                return;
            }

            int Length = socket.EndReceive(asyncResult);

            if (Length == 0)
            {
                CloseSocket();

                return;
            }

            message.ReadBuffer(Length, HandleResponse);

            StartReceive();
        }
        catch (Exception e)
        {
            Debug.LogError("服务器断开:   " + e);
            CloseSocket();
        }
    }

    /// <summary>
    /// 委托缓存堆栈
    /// </summary>
    private Queue<List<CSMsgDelegate>> cachelistHandle = new Queue<List<CSMsgDelegate>>();
    /// <summary>
    /// 消息包缓存堆栈
    /// </summary>
    private Queue<MainPack> queuepPacks = new Queue<MainPack>();
    /// <summary>
    /// Update主线程中释放缓存堆栈中的委托
    /// </summary>
    public void Update()
    {
        m_connectWatcher.Update();
        if ( cachelistHandle.Count <= 0 || queuepPacks.Count <= 0)
        {
            return;
        }

        try
        {
            foreach (CSMsgDelegate handle in cachelistHandle.Dequeue())
            {
                handle(queuepPacks.Peek());
            }
            queuepPacks.Dequeue();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    /// <summary>
    /// 网络消息回调，非主线程
    /// </summary>
    /// <param name="pack"></param>
    private void HandleResponse(MainPack pack)
    {
        lock (cachelistHandle)
        {
            if (HeartBeatResponse(pack))
            {
                return;
            }

            List<CSMsgDelegate> listHandle;

            if (m_mapCmdHandle.TryGetValue((int) pack.Actioncode, out listHandle))
            {
                cachelistHandle.Enqueue(listHandle);    //队列入队

                queuepPacks.Enqueue(pack);     
            }
        }
    }

    /// <summary>
    /// Udp网络消息回调，非主线程
    /// </summary>
    /// <param name="pack"></param>
    private void UdpHandleResponse(MainPack pack)
    {
        List<CSMsgDelegate> listHandle;

        if (m_mapCmdHandle.TryGetValue((int)pack.Actioncode, out listHandle))
        {
            foreach (CSMsgDelegate handle in listHandle)
            {
                handle(pack);
            }
        }
    }

    /// <summary>
    /// 心跳包回包
    /// </summary>
    /// <param name="pack"></param>
    /// <returns></returns>
    private bool HeartBeatResponse(MainPack pack)
    {
        if (pack.Actioncode == ActionCode.HeartBeat)
        {
            EventCenter.Instance.EventTrigger<MainPack>(NetEvent.HeartBeat, pack);
            TDebug.LogError("-----------------------------Heart Beat-------------------------------");
            return true;
        }

        return false;
    }

    public void Send(MainPack pack)
    {
        if (socket.Connected == false || socket == null)
        {
            TDebug.LogError("Socket Connect => false");
            return;
        }
        socket.Send(Message.PackData(pack));
    }
    //---------------------------------UDP协议------------------------------------//
    private Socket udpClient;
    private IPEndPoint ipEndPoint;
    private EndPoint EPoint;
    private Byte[] buffer = new Byte[1024];
    public void ConnectUdp()
    {
        udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipEndPoint = new IPEndPoint(IPAddress.Parse(m_Host), m_Port + 1);
        EPoint = ipEndPoint;
        try
        {
            udpClient.Connect(EPoint);
        }
        catch
        {
            TDebug.LogError("UDP connect failed!...");
            return;
        }

        Loom.RunAsync(() =>
        {
            aucThread = new Thread(ReceiveMsg);
            aucThread.Start();
        }
        );
    }


    private void ReceiveMsg()
    {
        try
        {
            TDebug.LogNet("UDP connecting ...");
            while (true)
            {
                int len = udpClient.ReceiveFrom(buffer, ref EPoint);
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 0, len);
                Loom.QueueOnMainThread((param) =>
                {
                    UdpHandleResponse(pack);
                }, null);
            }
        }
        catch (Exception e)
        {
            TDebug.LogError(e);
        }
    }



    public void SendCSMsgUdp(MainPack pack)
    {
        Byte[] sendBuff = Message.PackDataUDP(pack);
        udpClient.Send(sendBuff, sendBuff.Length, SocketFlags.None);
    }

    //--------------------------------------------------------------------//
    /// <summary>
    /// 注册静态消息
    /// </summary>
    /// <param name="iCmdID"></param>
    /// <param name="msgDelegate"></param>
    public void RegActionHandle(int actionId, CSMsgDelegate msgDelegate)
    {
        List<CSMsgDelegate> listHandle;
        if (!m_mapCmdHandle.TryGetValue(actionId, out listHandle))
        {
            listHandle = new List<CSMsgDelegate>();
            m_mapCmdHandle[actionId] = listHandle;
        }

        if (listHandle != null)
        {
            if (listHandle.Contains(msgDelegate))
            {
                TDebug.LogNet("-------------repeat RegCmdHandle ActionCode:{0}-----------", (ActionCode)actionId);
            }
            listHandle.Add(msgDelegate);
        }
    }
    /// <summary>
    /// 注册Udp静态消息
    /// </summary>
    /// <param name="iCmdID"></param>
    /// <param name="msgDelegate"></param>
    public void UdpRegActionHandle(int actionId, CSMsgDelegate msgDelegate)
    {
        List<CSMsgDelegate> listHandle;
        if (!m_mapCmdHandle.TryGetValue(actionId, out listHandle))
        {
            listHandle = new List<CSMsgDelegate>();
            m_mapCmdHandle[actionId] = listHandle;
        }

        if (listHandle != null)
        {
            if (listHandle.Contains(msgDelegate))
            {
                TDebug.LogNet("-------------repeat RegCmdHandle ActionCode:{0}-----------", (ActionCode)actionId);
            }
            listHandle.Add(msgDelegate);
        }
    }

    /// <summary>
    /// 移除消息处理函数
    /// </summary>
    /// <param name="cmdId"></param>
    /// <param name="msgDelegate"></param>
    public void RmvCmdHandle(int actionId, CSMsgDelegate msgDelegate)
    {
        List<CSMsgDelegate> listHandle;
        if (!m_mapCmdHandle.TryGetValue(actionId, out listHandle))
        {
            return;
        }

        if (listHandle != null)
        {
            listHandle.Remove(msgDelegate);
        }
    }

    private bool CheckPack(MainPack pack)
    {
        if (pack == null)
        {
            return false;
        }

        if (pack.Actioncode == ActionCode.ActionNone)
        {
            return false;
        }

        if (pack.Requestcode == RequestCode.RequestNone)
        {
            return false;
        }

        return true;
    }

    public bool SendCSMsg(MainPack pack)
    {
        if (!CheckPack(pack))
        {
            return false;
        }
        return DoSendData(pack);
    }

    public bool SendCSMsg(MainPack pack, CSMsgDelegate resHandler = null, bool needShowWaitUI = true)
    {
        if (!CheckPack(pack))
        {
            return false;
        }

        var ret = DoSendData(pack);

        if (!ret)
        {
            TDebug.LogError("SendCSMsg Error!!!");
        }
        else
        {
            if (resHandler != null)
            {
                RegActionHandle((int) pack.Actioncode, resHandler);
            }
        }

        return ret;
    }

    private bool DoSendData(MainPack pack)
    {
        if (socket.Connected == false || socket == null)
        {
            return false;
        }
        socket.Send(Message.PackData(pack));

        return true;
    }

    private void ChangeStateOnEnterFail()
    {
        if (Status == GameClientStatus.StatusClose)
        {
            CloseSocket();
            Status = GameClientStatus.StatusClose;
        }
    }

    public void Reconnect()
    {
        Status = GameClientStatus.StatusReconnect;

        if (string.IsNullOrEmpty(m_Host) || m_Port <= 0)
        {
            TDebug.LogNet("Invalid reconnect param");

            return;
        }

        Connect(m_Host, m_Port);
        ConnectUdp();
    }

    /// <summary>
    /// 设置是否需要监控网络重连
    /// 登录成功后，开启监控,可以自动重连或者提示玩家重连
    /// </summary>
    /// <param name="needWatch"></param>
    public void SetWatchReconnect(bool needWatch)
    {
        m_connectWatcher.Enable = needWatch;
    }

    public void OnDestroy()
    {
        socket.Close();
        udpClient.Close();
    }
}

internal class MsgHandleDataToRmv
{
    public int m_msgCmd;
    public CSMsgDelegate m_handle;
};