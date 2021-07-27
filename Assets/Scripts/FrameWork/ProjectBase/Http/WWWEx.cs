using UnityEngine;
using System.Collections;

public class WWWEx
{
    private float timeOut = 1f;

    private int retryCnt = 1;

    public WWW result;

    public bool isTimeOut = false;

    private int m_retryCnt = 0;

    private WWWEx()
    {

    }

    public IEnumerator Request(string url)
    {
        return Request(url, null);
    }

    public IEnumerator Request(string url, WWWForm form)
    {
        while (m_retryCnt < retryCnt)
        {
            var www = form == null ? (new WWW(url)) : (new WWW(url, form));
            var endTime = Time.realtimeSinceStartup + timeOut;
            while (!www.isDone && Time.realtimeSinceStartup < endTime)
            {
                yield return 0;
            }

            if (!www.isDone)
            {
                isTimeOut = true;
                result = null;
            }
            else
            {
                isTimeOut = false;
                result = www;
                yield break;
            }

            m_retryCnt++;
        }
    }

    public static WWWEx Init(float timeOut = 10f, int retryCnt = 3)
    {
        WWWEx wwwEx = new WWWEx();
        wwwEx.timeOut = timeOut;
        wwwEx.retryCnt = retryCnt;
        return wwwEx;
    }

    public void Destroy()
    {
        if (!isTimeOut && result != null)
        {
            result.Dispose();
        }
    }
}