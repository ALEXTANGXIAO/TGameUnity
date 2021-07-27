using SocketGameProtocol;
using UnityEngine;

public class ActorDataMgr : DataCenterModule<ActorDataMgr>
{
    private bool IsUDP = true;

    public override void Init()
    {
        GameClient.Instance.UdpRegActionHandle((int)ActionCode.UpPos, UpPosRes);
        CachePack();
    }


    private void UpPosRes(MainPack mainPack)
    {
        //TDebug.Log(mainPack);
        if (TUtil.CheckHaveError(mainPack))
        {
            return;
        }

        ActorMgr.Instance.UpPos(mainPack);
    }

    public void UpPosReq(Vector3 pos,Vector3 moveVector3, float characterRot, int Animation, int dirt)
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Game, ActionCode.UpPos);
        PosPack posPack = new PosPack();
        PlayerPack playerPack = new PlayerPack();
        posPack.PosX = pos.x;
        posPack.PosY = pos.y;
        posPack.PosZ = pos.z;

        posPack.MoveX = moveVector3.x;
        posPack.MoveY = moveVector3.y;
        posPack.MoveZ = moveVector3.z;

        posPack.RotaY = characterRot;
        posPack.Animation = Animation;
        posPack.Dirt = dirt;
        playerPack.Playername = LoginDataMgr.Instance.m_userName;
        playerPack.PosPack = posPack;
        mainPack.Playerpack.Add(playerPack);

        if (IsUDP)
        {
            mainPack.User = LoginDataMgr.Instance.m_userName;
            GameClient.Instance.SendCSMsgUdp(mainPack);
        }
        else
        {
            GameClient.Instance.SendCSMsg(mainPack);
        }
    }

    public void UpCachePosReq(Vector3 pos, Vector3 moveVector3, float characterRot, int Animation, int dirt)
    {
        m_mainPack.Playerpack[0].PosPack.PosX = pos.x;
        m_mainPack.Playerpack[0].PosPack.PosY = pos.y;
        m_mainPack.Playerpack[0].PosPack.PosZ = pos.z;

        m_mainPack.Playerpack[0].PosPack.MoveX = moveVector3.x;
        m_mainPack.Playerpack[0].PosPack.MoveY = moveVector3.y;
        m_mainPack.Playerpack[0].PosPack.MoveZ = moveVector3.z;
        m_mainPack.Playerpack[0].PosPack.RotaY = characterRot;
        m_mainPack.Playerpack[0].PosPack.Animation = Animation;
        m_mainPack.Playerpack[0].PosPack.Dirt = dirt;
        m_mainPack.Playerpack[0].Playername = LoginDataMgr.Instance.m_userName;
        if (IsUDP)
        {
            m_mainPack.User = LoginDataMgr.Instance.m_userName;
            GameClient.Instance.SendCSMsgUdp(m_mainPack);
        }
        else
        {
            GameClient.Instance.SendCSMsg(m_mainPack);
        }
    }

    private bool m_setName;
    private MainPack m_mainPack;
    private void CachePack()
    {
        m_mainPack = ProtoUtil.BuildMainPack(RequestCode.Game, ActionCode.UpPos);
        PosPack posPack = new PosPack();
        PlayerPack playerPack = new PlayerPack();
        playerPack.PosPack = posPack;
        m_mainPack.Playerpack.Add(playerPack);
    }
}