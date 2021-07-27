using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI层级枚举
/// </summary>
public enum UI_Layer
{
    Bottom,
    Mid,
    Top,
    System,
}

/// <summary>
/// UI管理器
/// </summary>
public class UIManager : Singleton<UIManager>
{
    public RectTransform canvas;

    private Transform bottom;
    private Transform mid;
    private Transform top;
    private Transform system;

    public UIManager()
    {
        GameObject obj =  ResourcesManager.Instance.AllocGameObject("UI/Canvas");
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);
        obj = ResourcesManager.Instance.AllocGameObject("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);

        bottom = canvas.Find("Bottom");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
    }

    private static Dictionary<string, UIWindow> m_typeToInst = new Dictionary<string, UIWindow>();
    public Dictionary<uint, UIWindow> uiidDic = new Dictionary<uint, UIWindow>();
    private List<UIWindow> listWindows = new List<UIWindow>();

    private uint uiid;

    public T ShowWindow<T>(UI_Layer layer = UI_Layer.Mid, UnityAction<T> callback = null) where T : UIWindow, new()
    {
        string typeName = GetWindowTypeName<T>();

        T window = GetUIWindowByType(typeName) as T;
        if (window == null)
        {
            window = new T();
            if (!CreateWindowByType(window, typeName, layer))
            {
                return null;
            }
        }
        listWindows.Add(window);
        window.Show();
        return window;
    }

    public void CloseWindow<T>() where T : UIWindow
    {
        string typeName = GetWindowTypeName<T>();
        CloseWindow(typeName);
    }

    public void CloseWindow(string typeName)
    {
        UIWindow window = GetUIWindowByType(typeName);
        if (window != null)
        {
            CloseWindow(window);
        }
    }

    public void CloseWindow(UIWindow window)
    {
        if (window.IsDestroyed)
        {
            return;
        }

        string typeName = window.GetType().Name;

        UIWindow typeWindow;

        if (m_typeToInst.TryGetValue(typeName, out typeWindow) && typeWindow == window)
        {
            m_typeToInst.Remove(typeName);
        }

        listWindows.Remove(window);

        if (uiidDic.ContainsKey(uiid))
        {
            uiidDic.Remove(uiid);
        }

        window.Destroy();

        window = null;
    }

    public string GetWindowTypeName<T>()
    {
        string typeName = typeof(T).Name;
        return typeName;
    }

    public UIWindow GetUIWindowByType(string typeName)
    {
        UIWindow window;
        if (m_typeToInst.TryGetValue(typeName, out window))
        {
            return window;
        }

        return null;
    }

    private bool CreateWindowByType(UIWindow window, string typeName, UI_Layer layer = UI_Layer.Mid)
    {
        m_typeToInst[typeName] = window;

        string resPath = GetUIResourcePath(typeName);

        if (string.IsNullOrEmpty(resPath))
        {
            TDebug.Log("CreateWindowByType failed, typeName:" + typeName);
            return false;
        }

        GameObject uiObj = null;

        uiObj = (GameObject)ResourcesManager.Instance.Load(resPath);
        if (uiObj == null)
        {
            TDebug.Log("CreateWindowByType failed, " + typeName + resPath);
            //Download
            return false;
        }

        uiObj.name = typeName;

        uiid = uiid + 1;

        window.AllocUIID(uiid);

        uiidDic.Add(uiid, window);

        Transform father = bottom;
        switch (layer)
        {
            case UI_Layer.Mid:
                father = mid;
                break;
            case UI_Layer.Top:
                father = top;
                break;
            case UI_Layer.System:
                father = system;
                break;
        }

        uiObj.transform.SetParent(father);

        RectTransform rectTrans = uiObj.transform as RectTransform;
        rectTrans.localRotation = Quaternion.identity;
        rectTrans.localScale = Vector3.one;

        uiObj.transform.localPosition = Vector3.zero;
        uiObj.transform.localPosition = Vector3.one;
        (uiObj.transform as RectTransform).offsetMax = Vector2.zero;
        (uiObj.transform as RectTransform).offsetMin = Vector2.zero;

        if (!window.Create(this, uiObj))
        {
            TDebug.Log("window create failed, typeName: "+ typeName);
            if (uiObj != null)
            {
                GameObject.Destroy(uiObj);
                uiObj = null;
            }

            return false;
        }

        return true;
    }
    private string GetUIResourcePath(string typeName)
    {
        string path = string.Format("UI/{0}", typeName);
        return path;
    }

    public void Update()
    {
        var allList = listWindows;
        for (int i = 0; i < allList.Count; i++)
        {
            UIWindow window = allList[i];
            if (!window.IsDestroyed)
            {
                window.Update();
            }
        }
    }
    //-----------------------------------------------------------------------------------------------------------------------------//

    /// <summary>
    /// 销毁面板
    /// </summary>
    /// <param name="panelName"></param>
    public void DestroyUI(string panelName)
    {
        if (m_typeToInst.ContainsKey(panelName))
        {
            m_typeToInst[panelName].Hide();
            GameObject.Destroy(m_typeToInst[panelName].gameObject);
            m_typeToInst.Remove(panelName);
        }
    }

    /// <summary>
    /// 获得面板
    /// </summary>
    public T GetUI<T>(string name) where T:UIWindow
    {
        if (m_typeToInst.ContainsKey(name))
        {
            return m_typeToInst[name] as T;
        }

        return null;
    }

    public Transform GetLayerFather(UI_Layer layer)
    {
        switch (layer)
        {
            case UI_Layer.Bottom:
                return this.bottom;
            case UI_Layer.Mid:
                return this.mid;
            case UI_Layer.Top:
                return this.top;
            case UI_Layer.System:
                return this.system;

        }
        return null;
    }

    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callback">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control,EventTriggerType type,UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();

        if(trigger == null)
        {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);

        trigger.triggers.Add(entry);
    }
}
