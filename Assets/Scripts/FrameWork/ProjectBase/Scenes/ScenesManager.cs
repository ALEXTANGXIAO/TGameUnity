using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class ScenesManager : Singleton<ScenesManager>
{
    /// <summary>
    /// 切换场景 同步
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name,UnityAction fun)
    {
        SceneManager.LoadScene(name);

        fun();
    }

    public void LoadSceneAsyn(string name,UnityAction fun = null)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, fun));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);

        while (!ao.isDone)
        {
            EventCenter.Instance.EventTrigger<float>("Load", ao.progress);

            yield return ao.progress;
        }

        yield return ao;

        if (fun != null)
        {
            fun();
        }
    }
}
