using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class UIWindow
{
    public uint UIID;
    protected GameObject m_go;
    protected RectTransform m_transform;

    protected UIWindow m_parent = null;
    public List<UIWindow> m_listChild = null;

    protected Canvas m_canvas;
    protected string m_name;
    protected bool m_destroyed = true;
    public bool m_visible = false;
    private bool m_isClosed = false;

    protected UIManager m_UIManager = null;

    //通过里氏转换原则 存储所有控件
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    enum UIWindowBaseType
    {
        None,
        Window,
        Widget,
    }



    public GameObject gameObject
    {
        get { return m_go; }
    }
    public bool IsDestroyed
    {
        get { return m_destroyed; }
    }

    public bool IsCreated
    {
        get { return !IsDestroyed; }
    }

    public RectTransform transform
    {
        get { return m_transform; }
    }
    public string name
    {
        get
        {
            if (string.IsNullOrEmpty(m_name))
            {
                m_name = GetType().Name;
            }

            return m_name;
        }
    }

    private List<IUIEventInfo> UIEventInfoList;
    private List<IUIEventInfo> m_UIEventInfoList
    {
        get
        {
            if (UIEventInfoList == null)
                UIEventInfoList = new List<IUIEventInfo>();
            return UIEventInfoList;
        }
    }

    public void AllocUIID(uint uiid)
    {
        if (UIID != 0)
        {
            TDebug.Log("UIID has Alloc: " + uiid);
            return;
        }

        UIID = uiid;
    }

    //-------------------------------------------------------------接口方法----------------------------------------------------------//
    public bool Create(UIManager uiMgr, GameObject uiGo)
    {
        if (IsCreated)
        {
            return true;
        }

        m_isClosed = false;
        if (!CreateBase(uiGo, true))
        {
            return false;
        }

        //if (m_canvas == null)
        //{
        //    TDebug.Log(this.ToString() + " have not a canvas!!");
        //    Destroy();
        //    return false;
        //}

        m_UIManager = uiMgr;

        if (m_canvas != null)
        {
            m_canvas.overrideSorting = true;
        }

        ScriptGenerator();
        RegisterEvent();

        OnCreate();

        FindChildrenControl<Button>();

        if (m_isClosed)
        {
            Destroy();
            return false;
        }

        return true;
    }


    protected bool CreateBase(GameObject go, bool bindGo = false)
    {
        if (!m_destroyed)
        {
            TDebug.Log("UI has created: " + go.name);
            return false;
        }

        if (go == null)
        {
            return false;
        }

        m_destroyed = false;

        m_go = go;

        m_transform = go.GetComponent<RectTransform>();

        m_canvas = gameObject.GetComponent<Canvas>();

        var canvas = gameObject.GetComponentsInChildren<Canvas>(true);

        for (var i = 0; i < canvas.Length; i++)
        {
            var canva = canvas[i];
            canva.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
        }

        return true;
    }



    public virtual void Show(bool visible = true)
    {
        if (m_destroyed || gameObject == null)
        {
            return;
        }
        if (m_visible != visible)
        {
            m_visible = visible;
            if (visible)
            {
                gameObject.SetActive(true);
                OnVisible();
            }
            else
            {
                Hide();

                if (gameObject == null)
                {
                    TDebug.Log("ui bug, hiden destory gameobject: " + name);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }


    public void Destroy()
    {
        if (IsDestroyed)
        {
            return;
        }

        m_destroyed = true;

        ClearAllRegistEvent();

        OnDestroy();

        if (m_go != null)
        {
            GameObject.Destroy(m_go);
            m_go = null;
        }
        m_destroyed = true;
        m_transform = null;
    }

    protected void AddUIEvent(string eventName, UnityAction action)
    {
        EventCenter.Instance.AddEventListener(eventName, action);
        m_UIEventInfoList.Add(new UIEventInfo(eventName, action));
    }

    protected void AddUIEvent<T>(string eventName, UnityAction<T> action)
    {
        EventCenter.Instance.AddEventListener<T>(eventName, action);
        m_UIEventInfoList.Add(new UIEventInfo<T>(eventName, action));
    }

    protected void ClearAllRegistEvent()
    {
        var uiEventList = UIEventInfoList;
        if (uiEventList != null)
        {
            for (int i = 0; i < uiEventList.Count; i++)
            {
                var msg = uiEventList[i] as UIEventInfo;

                if(msg == null)
                {
                    continue;
                }

                EventCenter.Instance.RemoveEventListener(msg.eventName, msg.action);
            }
            uiEventList.Clear();
        }
    }

    public Transform FindChild(string path)
    {
        return UnityUtil.FindChild(transform, path);
    }

    public Transform FindChild(Transform _transform, string path)
    {
        return UnityUtil.FindChild(_transform, path);
    }

    public T FindChildComponent<T>(string path) where T : Component
    {
        return UnityUtil.FindChildComponent<T>(transform, path);
    }

    public T FindChildComponent<T>(Transform _transform, string path) where T : Component
    {
        return UnityUtil.FindChildComponent<T>(_transform, path);
    }

    /**
     * 创建窗口内嵌的界面
     */
    public bool Create(UIWindow parent, GameObject widgetRoot, bool visible = true)
    {
        return CreateImp(parent, widgetRoot, false, visible);
    }

    private bool CreateImp(UIWindow parent, GameObject widgetRoot, bool bindGo, bool visible = true)
    {
        if (!CreateBase(widgetRoot, bindGo))
        {
            return false;
        }
        RestChildCanvas(parent);
        m_parent = parent;
        if (m_parent != null)
        {
            m_parent.AddChild(this);
        }

        if (m_canvas != null)
        {
            m_canvas.overrideSorting = true;
        }
        ScriptGenerator();
        RegisterEvent();

        OnCreate();

        if (visible)
        {
            Show(true);
        }
        else
        {
            widgetRoot.SetActive(false);
        }

        return true;
    }

    /// <summary>
    /// 一个界面中最大sortOrder值
    /// </summary>
    public const int MaxCanvasSortingOrder = 50;
    private void RestChildCanvas(UIWindow parent)
    {
        if (gameObject == null)
        {
            return;
        }
        if (parent == null || parent.gameObject == null)
        {
            return;
        }
        Canvas parentCanvas = parent.gameObject.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            return;
        }
        var listCanvas = gameObject.GetComponentsInChildren<Canvas>(true);
        for (var index = 0; index < listCanvas.Length; index++)
        {
            var childCanvas = listCanvas[index];
            childCanvas.sortingOrder = parentCanvas.sortingOrder + childCanvas.sortingOrder % UIWindow.MaxCanvasSortingOrder;
        }
    }

    public void AddChild(UIWindow child)
    {
        if (m_listChild == null)
        {
            m_listChild = new List<UIWindow>();
        }

        m_listChild.Add(child);
    }

    public void _OnSortingOrderChg()
    {
        if (m_listChild != null)
        {
            for (int i = 0; i < m_listChild.Count; i++)
            {
                if (m_listChild[i].m_visible)
                {
                    m_listChild[i]._OnSortingOrderChg();
                }
            }
        }
        OnSortingOrderChg();
    }

    protected virtual void OnSortingOrderChg()
    {
    }

    protected Coroutine StartCoroutine(string name, IEnumerator routine)
    {
        return MonoManager.Instance.StartCoroutine(routine);
    }

    protected void StopCoroutine(Coroutine cort)
    {
        MonoManager.Instance.StopCoroutine(cort);
    }

    /// <summary>
    /// 得到对应名字的控制脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDic[controlName].Count; i++)
            {
                if (controlDic[controlName][i] is T)
                {
                    return controlDic[controlName][i] as T;
                }
            }
        }

        return null;
    }

    protected T GetUIComponent<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDic[controlName].Count; i++)
            {
                if (controlDic[controlName][i] is T)
                {
                    return controlDic[controlName][i] as T;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = UnityUtil.GetComponentsInChildren<T>(gameObject);

        if (gameObject == null)
        {
            TDebug.LogError(name + ": UI GameObject Is Null Now...");
            return;
        }

        if (controls == null)
        {
            TDebug.LogError(name +": Controls Is Null Now...");
            return;
        }

        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].gameObject.name;

            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add(controls[i]);
            }
            else
            {
                controlDic.Add(controls[i].gameObject.name, new List<UIBehaviour>() { controls[i] });

                if (controls[i] is Button)
                {
                    if (controls[i].gameObject.GetComponent<UIButtonSound>() == null)
                    {
                        controls[i].gameObject.AddComponent<UIButtonSound>();
                    }
                    (controls[i] as Button).onClick.AddListener(() =>
                    {
                        OnClick(objName);
                    });
                }
            }
        }
    }

    protected virtual void OnClick(string btnName)
    {

    }
    //--------------------------------------------------------------生命周期---------------------------------------------------------//
    protected virtual void OnCreate()
    {

    }

    protected virtual void ScriptGenerator()
    {

    }

    protected virtual void RegisterEvent()
    {

    }

    public bool Update()
    {
        if (!m_visible || m_destroyed)
        {
            return false;
        }

        if (m_go.activeSelf == false)
        {
            return false;
        }

        OnUpdate();

        return true;
    }

    protected virtual void OnUpdate()
    {

    }

    public virtual void Hide()
    {

    }

    protected virtual void OnVisible()
    {

    }

    protected virtual void OnDestroy()
    {

    }
    //--------------------------------------------------------------生命周期---------------------------------------------------------//

}
public class UIEventInfo: IUIEventInfo
{
    public string eventName;
    public UnityAction action;

    public UIEventInfo(string n, UnityAction a)
    {
        eventName = n;
        action = a;
    }
}

public class UIEventInfo<T>: IUIEventInfo
{
    public string eventName;
    public UnityAction<T> action;

    public UIEventInfo(string n, UnityAction<T> a)
    {
        eventName = n;
        action = a;
    }
}

internal interface IUIEventInfo
{

}

