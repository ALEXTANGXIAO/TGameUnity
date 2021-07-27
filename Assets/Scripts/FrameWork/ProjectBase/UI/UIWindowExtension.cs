using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public partial class UIWindow
{
    /**
    * 创建子控件，
    * goPath 控件的gameobject相当于本window的位置
    */
    public T CreateWidget<T>(string goPath, bool visible = true) where T : UIWindowWidget, new()
    {
        var goRootTrans = FindChild(goPath);
        if (goRootTrans != null)
        {
            return CreateWidget<T>(goRootTrans.gameObject, visible);
        }

        TDebug.Log("CreateWidget failed, path: {0}, widget type: {1}", goPath, typeof(T).FullName);
        return null;
    }

    public T CreateWidget<T>(GameObject goRoot, bool visible = true) where T : UIWindowWidget, new()
    {
        var widget = new T();
        if (!widget.Create(this, goRoot, visible))
        {
            return null;
        }

        return widget;
    }

    /// <summary>
    /// 调整图标数量
    /// </summary>
    public void AdjustIconNum<T>(List<T> listIcon, int tarNum, Transform parent, GameObject prefab = null) where T : UIWindowWidget, new()
    {
        if (listIcon == null)
            listIcon = new List<T>();
        if (listIcon.Count < tarNum) // 不足则添加
        {
            T tmpT;
            int needNum = tarNum - listIcon.Count;
            for (int iconIdx = 0; iconIdx < needNum; iconIdx++)
            {
                if (prefab == null)
                {
                    tmpT = CreateWidgetByType<T>(parent);
                }
                else
                {
                    tmpT = CreateWidgetByPrefab<T>(prefab, parent);
                }
                listIcon.Add(tmpT);
            }
        }
        else if (listIcon.Count > tarNum) // 多则删除
        {
            RemoveUnuseItem<T>(listIcon, tarNum);
        }
    }

    /// <summary>
    /// 异步创建接口，maxNumPerFrame单帧最多的创建数量
    /// 注意disable的对象无法运行协程
    /// </summary>
    public IEnumerator AsyncAdjustIconNumIE<T>(List<T> listIcon, int tarNum, Transform parent, int maxNumPerFrame, Action<T, int> updateAction, GameObject prefab) where T : UIWindowWidget, new()
    {
        if (listIcon == null)
        {
            listIcon = new List<T>();
        }

        int createCnt = 0;

        for (int i = 0; i < tarNum; i++)
        {
            T tmpT;
            if (i < listIcon.Count)
            {
                tmpT = listIcon[i];
            }
            else
            {
                if (prefab == null)
                {
                    tmpT = CreateWidgetByType<T>(parent);
                }
                else
                {
                    tmpT = CreateWidgetByPrefab<T>(prefab, parent);
                }
                listIcon.Add(tmpT);
            }
            int index = i;
            if (updateAction != null)
            {
                updateAction(tmpT, index);
            }

            createCnt++;
            if (createCnt >= maxNumPerFrame)
            {
                createCnt = 0;
                yield return null;
            }
        }
        if (listIcon.Count > tarNum) // 多则删除
        {
            RemoveUnuseItem(listIcon, tarNum);
        }
    }

    public void AsyncAdjustIconNum<T>(string name, List<T> listIcon, int tarNum, Transform parent, int maxNumPerFrame = 5,
        Action<T, int> updateAction = null, GameObject prefab = null) where T : UIWindowWidget, new()
    {
        StartCoroutine(name, AsyncAdjustIconNumIE(listIcon, tarNum, parent, maxNumPerFrame, updateAction, prefab));
    }

    public T CreateWidgetByType<T>(Transform parent, bool visible = true) where T : UIWindowWidget, new()
    {
        string resPath = string.Format("UI/{0}", typeof(T).Name);
        return CreateWidgetByResPath<T>(resPath, parent, visible);
    }
    /**
    * 根据prefab或者模版来创建新的 widget
    */
    public bool CreateByPrefab(UIWindow parent, GameObject goPrefab, Transform parentTrans, bool visible = true)
    {
        if (parentTrans == null)
        {
            parentTrans = parent.transform;
        }

        var widgetRoot = GameObject.Instantiate(goPrefab, parentTrans);
        return CreateImp(parent, widgetRoot, true, visible);
    }

    public T CreateWidgetByPrefab<T>(GameObject goPrefab, Transform parent, bool visible = true) where T : UIWindowWidget, new()
    {
        var widget = new T();
        if (!widget.CreateByPrefab(this, goPrefab, parent, visible))
        {
            return null;
        }
        return widget;
    }

    public T CreateWidgetByResPath<T>(string resPath, Transform parent, bool visible = true) where T : UIWindowWidget, new()
    {
        var widget = new T();
        if (!widget.CreateByPath(resPath, this, parent, visible))
        {
            return null;
        }
        return widget;
    }

    public bool CreateByPath(string resPath, UIWindow parent, Transform parentTrans = null, bool visible = true)
    {
        GameObject goInst = (GameObject)ResourcesManager.Instance.Load(resPath);
        goInst.transform.SetParent(parent.transform);
        //GameObject goInst = DResources.AllocGameObject(resPath, parentTrans);
        if (goInst == null)
        {
            return false;
        }
        if (!Create(parent, goInst, visible))
        {
            return false;
        }
        goInst.transform.localScale = Vector3.one;
        goInst.transform.localPosition = Vector3.zero;
        return true;
    }

    private void RemoveUnuseItem<T>(List<T> listIcon, int tarNum) where T : UIWindowWidget, new()
    {
        for (int i = 0; i < listIcon.Count; i++)
        {
            var icon = listIcon[i];
            if (i >= tarNum)
            {
                listIcon.RemoveAt(i);
                icon.Destroy();
                --i;
            }
        }
    }
}

public class UIWindowWidget:UIWindow
{
    public int SortingOrder
    {
        get
        {
            if (m_canvas != null)
            {
                return m_canvas.sortingOrder;
            }

            return 0;
        }

        set
        {
            if (m_canvas != null)
            {
                int oldOrder = m_canvas.sortingOrder;
                if (oldOrder != value)
                {
                    var listCanvas = gameObject.GetComponentsInChildren<Canvas>(true);
                    for (int i = 0; i < listCanvas.Length; i++)
                    {
                        var childCanvas = listCanvas[i];
                        childCanvas.sortingOrder = value + (childCanvas.sortingOrder - oldOrder);
                    }
                    m_canvas.sortingOrder = value;
                    _OnSortingOrderChg();
                }
            }
        }
    }

    protected void _OnSortingOrderChg()
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
}
