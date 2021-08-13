using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleNameTransformBind : TitleTransformBind
{
    public TitleNameTransformBind(Transform transform) : base(transform)
    {

    }

    private Text m_bindText;

    public Text BindText
    {
        get { return m_bindText; }
    }

    public void OnVisibleChange(bool visible)
    {
        text = visible ? m_cacheText : string.Empty;
    }

    private string m_cacheText = string.Empty;
    public string text
    {
        get
        {
            if (m_bindText != null)
            {
                return m_bindText.text;
            }

            return string.Empty;
        }

        set
        {
            if (m_bindText != null)
            {
                m_bindText.text = value;
                m_cacheText = value;
            }
        }
    }


    public void Init(Text bindTarget)
    {
        m_bindText = bindTarget;
        Init(bindTarget.rectTransform);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_bindText = null;
    }
}

public class TitleTransformBind
{
    private RectTransform m_bindTransform;

    public RectTransform BindTransform
    {
        get { return m_bindTransform; }
    }
    private LayoutElement m_layOutElement;

    private GameObject m_obj;

    private Transform m_transform;

    public TitleTransformBind(Transform transform)
    {
        m_obj = transform.gameObject;
        m_transform = transform;
        m_layOutElement = m_obj.GetComponent<LayoutElement>();
    }

    //改成外部调用，确保可控
    private float m_lastSizeX;
    private float m_lastSizeY;
    public void OnLateUpdate()
    {
        var pos = m_transform.position;
        m_bindTransform.position = pos;

        var sizeDelta = m_bindTransform.sizeDelta;
        if (!Equal(m_lastSizeX, sizeDelta.x) || !Equal(m_lastSizeY, sizeDelta.y))
        {
            m_lastSizeX = sizeDelta.x;
            m_lastSizeY = sizeDelta.y;
            m_layOutElement.minWidth = m_lastSizeX;
            m_layOutElement.minHeight = m_lastSizeY;

            if (m_onLayoutChange != null)
            {
                m_onLayoutChange(m_layOutElement);
            }
        }
    }

    private event Action<LayoutElement> m_onLayoutChange;
    public void RegisterLayoutChangeEvent(Action<LayoutElement> onLayoutChangeEvent)
    {
        m_onLayoutChange += onLayoutChangeEvent;
    }

    public void UnRegisterEventLayoutChangeEvent(Action<LayoutElement> onLayoutChangeEvent)
    {
        m_onLayoutChange -= onLayoutChangeEvent;
    }

    private bool Equal(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.001f;
    }

    public void Init(RectTransform bindTarget)
    {
        m_bindTransform = bindTarget;
    }

    public void SetActive(bool value)
    {
        if (m_obj.activeSelf != value)
            m_obj.SetActive(value);

        if (m_bindTransform != null)
        {
            GameObject binGo = m_bindTransform.gameObject;
            if (binGo.activeSelf != value)
                binGo.SetActive(value);
        }
    }

    public virtual void OnDestroy()
    {
        m_bindTransform = null;
        m_layOutElement = null;
        m_obj = null;
        m_transform = null;
    }
}