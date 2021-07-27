using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MsgBubbleItem
{
    public Image m_spriteBg;                // 冒泡底图
    public Text m_textContent;              // 冒泡内容
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


    public MsgBubbleItem(GameObject go)
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
        UpdateTipPos();
    }

    public void LateUpdate()
    {
        // 更新提示坐标
        UpdateTipPos();
    }

    private void UpdateTipPos()
    {
        if (WorldCamera == null || m_tfPosNode == null) return;

        Vector3 pos = m_tfPosNode.position;
        pos.y = (float)(pos.y + 2.3);

        Root.transform.position = DTransform.GetNamePos(pos, WorldCamera.transform.position, m_radius);
        Root.transform.rotation = WorldCamera.transform.rotation;
        float scale = DTransform.CalUIScale(WorldCamera, WorldCamera.transform, Root.transform);//1;
        Root.transform.localScale = new Vector3(scale, scale, 1);
    }

    public void OnRecycle()
    {
        m_radius = 0;
        Root.SetActive(false);
    }
}
