using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;


public class TUtil
{
    public static bool CheckHaveError(MainPack mainPack)
    {
        bool hasError = false;
        if (mainPack == null)
        {
            var tip = string.Format("网络数据错误{0}", mainPack.Actioncode);
            //UISys.Mgr.ShowTipMsg(tip);
            TDebug.Error("package {0} null!", mainPack.Actioncode);
            hasError = true;
            return hasError;
        }

        if (mainPack.Returncode == ReturnCode.Fail)
        {
            hasError = true;
        }

        return hasError;
    }

    /// <summary>
    /// 存储字典的值到列表里面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="W"></typeparam>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static List<W> SaveDicValueToList<T, W>(Dictionary<T, W> dic)
    {
        List<W> listW = new List<W>();
        var ienuDic = dic.GetEnumerator();

        while (ienuDic.MoveNext())
        {
            listW.Add(ienuDic.Current.Value);
        }

        return listW;
    }
}
