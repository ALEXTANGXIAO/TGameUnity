
using UnityEngine;
using UnityEngine.UI;

class ActorNameBase
{
    protected GameActor m_actor;
    protected Transform m_textRoot;
    protected Transform m_effectRoot;
    protected Transform m_specialRoot;
    protected RectTransform m_tfNode;      // 根节点
    public RectTransform Node
    {
        get { return m_tfNode; }
    }

    protected virtual void OnInit()
    {
    }

    public void SetActive(bool active)
    {
        GameObject nodeGameObject = m_tfNode.gameObject;
        if (nodeGameObject.activeSelf != active)
            nodeGameObject.SetActive(active);
    }

    public virtual void InitActor(GameActor actor)
    {
        m_actor = actor;
    }

    public virtual void DeInit()
    {
        m_actor = null;
    }

    // 初始化
    public void Init(RectTransform rectTrans, Transform textRoot, Transform effRoot, Transform speRoot)
    {
        m_tfNode = rectTrans;
        m_textRoot = textRoot;
        m_effectRoot = effRoot;
        m_specialRoot = speRoot;
        OnInit();
    }

    // 更新名字
    public virtual void UpdateInfo(GameActor actor)
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void OnVisibleStateChange(bool visible)
    {

    }

    public static TitleNameTransformBind CreateTextBind(Transform rootTrans, Transform textRoot, string rootPath)
    {
        var transform = UnityUtil.FindChild(rootTrans, rootPath);
        if (transform == null) return null;

        TitleNameTransformBind bind = null;
        var target = UnityUtil.FindChildComponent<Text>(textRoot, transform.name);
        if (target != null)
        {
            bind = new TitleNameTransformBind(transform);
            bind.Init(target);
        }
        else
        {
            TDebug.Error("FindTextComponentError, name : " + transform.name);
        }

        return bind;
    }

    public static TitleTransformBind CreateEffectBind(Transform rootTrans, Transform targetRoot, string path)
    {
        var transform = UnityUtil.FindChild(rootTrans, path);
        if (transform == null) return null;

        TitleTransformBind bind = null;
        RectTransform target = UnityUtil.FindChildComponent<RectTransform>(targetRoot, transform.name);
        if (target != null)
        {
            bind = new TitleTransformBind(transform);
            bind.Init(target);
        }
        else
        {
            TDebug.Error("FindComponentError, name : " + transform.name);
        }

        return bind;
    }

    public virtual void OnDestroy()
    {
        m_textRoot = null;
        m_effectRoot = null;
        m_tfNode = null;
    }
}
