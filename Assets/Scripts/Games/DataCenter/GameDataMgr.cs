using SocketGameProtocol;
using System;
using UnityEngine;

public class GameDataMgr : DataCenterModule<GameDataMgr>
{
    private ChatUI chatUI;
    public override void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.StartGame, StartGameRes);
        GameClient.Instance.RegActionHandle((int)ActionCode.Starting, StartingRes);
    }

    private void StartingRes(MainPack mainPack)
    {
        TDebug.Log(mainPack);
        if (TUtil.CheckHaveError(mainPack))
        {
            return;
        }

        if (chatUI == null)
        {
            UISys.Mgr.CloseWindow<RoomUI>();

            chatUI = UISys.Mgr.ShowWindow<ChatUI>();
        }
        //Cursor.visible = false;

        ActorMgr.Instance.AddPlayerEntity(mainPack);
    }

    private void StartGameRes(MainPack mainPack)
    {
        TDebug.Log(mainPack);
        if (TUtil.CheckHaveError(mainPack))
        {
            return;
        }
    }

    public void StartGameReq()
    {
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.StartGame);
        GameClient.Instance.SendCSMsg(mainPack);
    }
}