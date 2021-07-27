using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ProtoUtil
{
    public static MainPack BuildMainPack(RequestCode requestCode,ActionCode actionCode)
    {
        MainPack pack = new MainPack();
        pack.Requestcode = requestCode;
        pack.Actioncode = actionCode;
        return pack;
    }
}
