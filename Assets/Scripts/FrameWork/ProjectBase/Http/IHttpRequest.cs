using UnityEngine;

public interface IHttpRequest
{
    void Request(string url, WWWForm form, DHttpRequest.CallBack callback, float timeout, bool recordLog);
}
