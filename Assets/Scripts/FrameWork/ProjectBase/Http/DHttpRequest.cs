using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DHttpRequest : BaseClsTemplate<IHttpRequest>
{
    public delegate void CallBack(CSMsgResult result, string respone);

    /**
     * 请求网络
     */
    public static void Request(string url, CallBack callback, float timeout = 0f, bool recordLog = true)
    {
        if (m_imp != null)
        {
            m_imp.Request(url, null, callback, timeout, recordLog);
        }
    }

    public static void Request(string url, WWWForm form, CallBack callback, float timeout = 0f, bool recordLog = true)
    {
        if (m_imp != null)
        {
            m_imp.Request(url, form, callback, timeout, recordLog);
        }
    }
}
