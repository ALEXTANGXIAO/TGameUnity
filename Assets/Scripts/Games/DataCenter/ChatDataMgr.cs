using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ChatDataMgr : DataCenterModule<ChatDataMgr>
{
    public override void Init()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.Chat, ChatRes);
    }

    private void ChatRes(MainPack mainPack)
    {
        if (TUtil.CheckHaveError(mainPack))
        {
            return;
        }

        MsgBubble(mainPack.User, mainPack.Str);
        TDebug.Log(mainPack.User + ":" + mainPack.Str);
    }

    public void ChatReq(String msg)
    {
        TDebug.Log( string.Format("我:{0}",msg));
        MainPack mainPack = ProtoUtil.BuildMainPack(RequestCode.Room, ActionCode.Chat);
        mainPack.Str = msg;
        GameClient.Instance.SendCSMsg(mainPack);
        MsgBubble(LoginDataMgr.Instance.m_userName, mainPack.Str);
    }

    public void MsgBubble(string id,string msg)
    {
        var player = ActorMgr.Instance.GetPlayerById(id);
        if (player != null)
        {
            player.gameObject.GetComponent<BubbleCmpt>().AddNewBubble(msg);
        }
    }
}
