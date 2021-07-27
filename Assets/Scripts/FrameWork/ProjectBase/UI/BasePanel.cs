using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// </summary>
public class BasePanel : MonoBehaviour
{
    //通过里氏转换原则 存储所有控件
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
    }

    public void Create()
    {
        ScriptGenerator();
        RegisterEvent();
        OnCreate();
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

    public virtual void BeDestroy()
    {

    }

    public virtual void Show()
    {

    }

    public virtual void Hide()
    {

    }
    //--------------------------------------------------------------生命周期---------------------------------------------------------//
    protected virtual void OnClick(string btnName)
    {

    }

    /// <summary>
    /// 得到对应名字的控制脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName)where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for(int i = 0; i < controlDic[controlName].Count; i++)
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
    private void FindChildrenControl<T>() where T:UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();

        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].gameObject.name;

            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add( controls[i]);
            }
            else
            {
                controlDic.Add(controls[i].gameObject.name,new List<UIBehaviour>() { controls[i] });

                if(controls[i] is Button)
                {
                    (controls[i] as Button).onClick.AddListener(()=> 
                    {
                        OnClick(objName);
                    });
                }
            }
        }
    }
}
