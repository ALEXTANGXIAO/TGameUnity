using UnityEngine;
using UnityEngine.UI;

public class ActorNameEntity
{
    private GameActor m_actor;
    public GameActor Actor
    {
        get { return m_actor; }
        set { m_actor = value; }
    }

    public Image m_spriteBg;                // Name底图
    public Text m_textContent;              // Name内容
    private static Camera s_worldCamera;    // 世界摄像机

    private float m_radius;
    private Transform m_tfPosNode;      // 提示对应的位置节点

    private Camera WorldCamera
    {
        get
        {
            if (s_worldCamera == null)
            {
                s_worldCamera = Camera.main;
            }

            return s_worldCamera;
        }
    }

    public GameObject Root { private set; get; }

    //是否可以显示血条
    private bool m_hpVisible;
    public bool HpVisible
    {
        get { return m_hpVisible; }
        set
        {
            if (m_hpVisible != value)
            {
                m_hpVisible = value;
                //OnVisibleChanged();
            }
        }
    }

    private RectTransform m_rootTrans;
    private RectTransform m_textTrans;
    private RectTransform m_effectTrans;
    private RectTransform m_specialTrans;
    private GameObject m_spriteObj;
    private GameObject m_textObj;
    private GameObject m_effectObj;
    private GameObject m_specialObj;

    static readonly Vector3 HidePos = new Vector3(50000, 50000, 50000);

    public ActorNameEntity(GameObject go)
    {
        Root = go;
        //m_spriteBg = UnityUtil.FindChildComponent<Image>(Root.transform, "m_imgBg");
        m_textContent = UnityUtil.FindChildComponent<Text>(Root.transform, "m_textContent");

        InitWidget();
        RegisterEvent();
    }
    private ActorNameAttackHp m_attackHp;

    private void InitWidget()
    {
        RectTransform transAttackHp = UnityUtil.FindChildComponent<RectTransform>(Root.transform, "PosAttackHp");
        m_attackHp = new ActorNameAttackHp();
        m_attackHp.Init(transAttackHp, m_textTrans, m_effectTrans, m_specialTrans);

        if (m_actor == ActorMgr.Instance.GetMainPlayer())
        {
            m_attackHp.SetActive(false);
        }
        //m_attackHp.SetActive(false);
    }

    private void RegisterEvent()
    {
        //EventCenter.Instance.AddEventListener<GameActor>(ActorEvent.HeartHandle, HurtHandle);
    }

    public void HurtHandle(float value)
    {
        m_attackHp.Update(value);
    }

    public void ReInit(GameObject go, string content)
    {
        Root.SetActive(true);
        m_tfPosNode = go.transform;
        m_textContent.text = content;

        if (m_actor == ActorMgr.Instance.GetMainPlayer())
        {
            m_attackHp.SetActive(false);
        }
        // 更新提示坐标
        UpdateTransform();
    }

    public void LateUpdate()
    {
        // 更新坐标
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        if (WorldCamera == null || m_tfPosNode == null) return;

        if (Root == null)
        {
            return;
        }

        Vector3 pos = m_tfPosNode.position;
        pos.y = (float)(pos.y + 2.0);

        Root.transform.position = DTransform.GetNamePos(pos, WorldCamera.transform.position, m_radius);
        Root.transform.rotation = WorldCamera.transform.rotation;
        float scale = DTransform.CalUIScale(WorldCamera, WorldCamera.transform, Root.transform);//1;
        Root.transform.localScale = new Vector3(scale, scale, 1);
    }

    public void OnRecycle()
    {
        if (Root == null)
        {
            return;
        }
        m_radius = 0;
        Root.SetActive(false);
    }
}
