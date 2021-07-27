using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

class BubbleMgr : BaseLogicSys<BubbleMgr>
{
    private Dictionary<GameObject, float> m_bubbleInfoDic = new Dictionary<GameObject, float>();

    private NpcBubblePool m_pool = new NpcBubblePool();

    public override bool OnInit()
    {
        EventCenter.Instance.AddEventListener<GameObject, string>(BubbleEvent.AddNewBubble, AddNewBubble);
        EventCenter.Instance.AddEventListener<GameObject>(BubbleEvent.HideBubble, HideBubble);
        EventCenter.Instance.AddEventListener<GameObject>(BubbleEvent.DestroyBubble, DestroyBubble);
        return true;
    }
    private void AddNewBubble(GameObject actor, string content)
    {
        m_pool.CreateBubble(actor, content);
        m_bubbleInfoDic[actor] = Time.realtimeSinceStartup;
    }

    private void DestroyBubble(GameObject actor)
    {
        m_pool.DestroyBubble(actor);
        m_bubbleInfoDic.Remove(actor);
    }

    public void HideBubble(GameObject actor)
    {
        m_pool.HideBubble(actor);
    }

    public Dictionary<GameObject, float> GetBubbleInfo()
    {
        return m_bubbleInfoDic;
    }

    public override void OnLateUpdate()
    {
        m_pool.LateUpdate();
    }
}

public class NpcBubblePool
{
    private const int MaxCacheCount = 5;
    private List<MsgBubbleItem> m_listItem = new List<MsgBubbleItem>();
    private Dictionary<GameObject, MsgBubbleItem> m_itemDic = new Dictionary<GameObject, MsgBubbleItem>();
    private Queue<MsgBubbleItem> m_cacheQueue = new Queue<MsgBubbleItem>();

    public void CreateBubble(GameObject actor, string content)
    {
        if (actor == null) return;
        MsgBubbleItem item;
        if (!m_itemDic.TryGetValue(actor, out item))
        {
            item = m_cacheQueue.Count > 0 ? m_cacheQueue.Dequeue() : CreateNewItem();
            m_itemDic.Add(actor, item);
            m_listItem.Add(item);
        }
        item.ReInit(actor, content);
    }

    private string m_itemPath = "UI/MsgBubbleItem";
    private string m_rootTransPath = "UI/BubbleRoot";
    private Canvas m_rootCanvas;
    private RectTransform m_rootTrans;
    private RectTransform RootTrans
    {
        get
        {
            if (m_rootTrans == null)
            {
                GameObject go = DResources.AllocGameObject(m_rootTransPath);
                m_rootTrans = go.GetComponent<RectTransform>();
                m_rootCanvas = m_rootTrans.GetComponent<Canvas>();
                m_rootCanvas.sortingLayerName = "Game";
                m_rootCanvas.sortingOrder = 100;
                Object.DontDestroyOnLoad(go);
                m_rootTrans.position = Vector3.zero;
            }
            return m_rootTrans;
        }
    }

    private MsgBubbleItem CreateNewItem()
    {
        GameObject go = DResources.AllocGameObject(m_itemPath);
        var transform = go.transform;
        transform.SetParent(RootTrans);
        //UnityUtil.SetLayer(transform, GameLayer.ActorLayer);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        return new MsgBubbleItem(go);
    }

    public void DestroyBubble(GameObject actor)
    {
        MsgBubbleItem item;
        if (m_itemDic.TryGetValue(actor, out item))
        {
            m_itemDic.Remove(actor);
            m_listItem.Remove(item);
            if (m_cacheQueue.Count >= MaxCacheCount)
            {
                GameObject.Destroy(item.Root);
            }
            else
            {
                item.OnRecycle();
                m_cacheQueue.Enqueue(item);
            }
        }
    }

    public void HideBubble(GameObject actor)
    {
        MsgBubbleItem item;
        if (m_itemDic.TryGetValue(actor, out item))
        {
            item.Root.SetActive(false);
        }
    }

    public void LateUpdate()
    {
        int count = m_listItem.Count;
        for (int i = 0; i < count; i++)
        {
            m_listItem[i].LateUpdate();
        }
    }
}