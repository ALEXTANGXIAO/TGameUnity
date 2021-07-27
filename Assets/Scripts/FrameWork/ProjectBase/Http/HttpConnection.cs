using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum CSMsgResult
{
    NoError = 0,
    NetworkError = 1,
    InternalError = 2,
    MsgTimeOut = 3,
    PingTimeOut = 4,
}

public class HttpRequestParam
{
    public HttpRequestParam()
    {
        hasParamSign = true;
    }

    private Dictionary<string, string> m_mapParam;

    private WWWForm m_form;

    public WWWForm Form
    {
        get { return m_form; }
    }

    public bool hasParamSign { get; set; }

    public void AddParam(string param, string val)
    {
        if (m_mapParam == null)
        {
            m_mapParam = new Dictionary<string, string>();
        }
        m_mapParam[param] = val;
    }

    public void InitForm()
    {
        if (m_form == null)
        {
            m_form = new WWWForm();
        }
    }

    /// <summary>
    /// 上传资源用
    /// </summary>
    /// <param name="param"></param>
    /// <param name="data"></param>
    public void AddParam(string param, byte[] data, string fileName = "application/octet-stream", string mimeType = "image/png")
    {
        if (m_form == null)
        {
            m_form = new WWWForm();
        }
        m_form.AddBinaryData(param, data, fileName, mimeType);
    }

    /// <summary>
    /// 上传资源用
    /// </summary>
    /// <param name="param"></param>
    /// <param name="data"></param>
    public void AddPostParam(string param, string val)
    {
        if (m_form == null)
        {
            m_form = new WWWForm();
        }
        m_form.AddField(param, val);
    }

    public override string ToString()
    {
        if (m_mapParam == null || m_mapParam.Count == 0)
        {
            return "";
        }
        StringBuilder sb = new StringBuilder();

        foreach (KeyValuePair<string, string> kv in m_mapParam)
        {
            if (sb.Length == 0 && hasParamSign)
            {
                sb.Append('?');
            }
            else
            {
                sb.Append('&');
            }
            sb.AppendFormat("{0}={1}", WWW.EscapeURL(kv.Key), WWW.EscapeURL(kv.Value));
        }
        return sb.ToString();
    }

    public void Clear()
    {
        if (m_mapParam != null)
        {
            m_mapParam.Clear();
        }
    }
}

public class HttpConnection : BBehaviourSingleton<HttpConnection>
{
    private bool m_needLog = false;

    void Awake()
    {
        m_needLog = true;
    }

    public void Request(string url, HttpRequestParam param, DHttpRequest.CallBack callback, float timeout = 0f, bool recordLog = false)
    {
        WWWForm form = null;
        if (param != null)
        {
            url = url + param.ToString();
            form = param.Form;
        }

        StartCoroutine(ReuqestCort(url, form, callback, timeout, recordLog));
    }

    public void Request(string url, WWWForm form, DHttpRequest.CallBack callback, float timeout, bool recordLog)
    {
        StartCoroutine(ReuqestCort(url, form, callback, timeout, recordLog));
    }

    public IEnumerator ReuqestCort(string url, WWWForm form, DHttpRequest.CallBack callback, float timeout, bool recordLog)
    {
        if (recordLog)
        {
            TDebug.Warning("http request: {0}", url);
        }
        else
        {
            TDebug.Error("http request: {0}", url);
        }

        WWWEx game = WWWEx.Init(timeout < 0.1f ? 10f : timeout);
        yield return StartCoroutine(game.Request(url, form));
        if (game.isTimeOut)
        {
            if (callback != null)
            {
                callback(CSMsgResult.PingTimeOut, null);
            }
        }
        else
        {
            WWW www = game.result;

            if (!string.IsNullOrEmpty(www.error))
            {
                TDebug.Error("http request failed, url:{0}, error: {1}", url, www.error);
                if (callback != null)
                {
                    callback(CSMsgResult.NetworkError, null);
                }
                game.Destroy();
                yield break;
            }

            if (callback != null)
            {
                callback(CSMsgResult.NoError, www.text);
            }

            game.Destroy();
        }
    }
}



