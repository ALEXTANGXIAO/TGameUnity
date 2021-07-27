using UnityEngine;

class HttpRequestImp : IHttpRequest
{
    public void Request(string url, WWWForm form, DHttpRequest.CallBack callback, float timeout, bool recordLog)
    {
        HttpConnection.Instance.Request(url, form, callback, timeout, recordLog);
    }
}
