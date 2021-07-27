using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class ActorNameEntity
{
    private GameActor m_actor;
    public GameActor Actor
    {
        get { return m_actor; }
        private set { m_actor = value; }
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


    public ActorNameEntity(GameObject go)
    {
        Root = go;
        //m_spriteBg = UnityUtil.FindChildComponent<Image>(Root.transform, "m_imgBg");
        m_textContent = UnityUtil.FindChildComponent<Text>(Root.transform, "m_textContent");
    }

    public void ReInit(GameObject go, string content)
    {
        Root.SetActive(true);
        m_tfPosNode = go.transform;
        m_textContent.text = content;

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
