using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

class ActorNameMgr : BaseLogicSys<ActorNameMgr>
{
    private Dictionary<GameActor, float> m_ActorNameInfoDic = new Dictionary<GameActor, float>();

    private ActorNamePool m_pool = new ActorNamePool();

    public override bool OnInit()
    {
        EventCenter.Instance.AddEventListener<GameActor, string>(ActorNameEvent.AddNewActorName, AddNewActorName);
        EventCenter.Instance.AddEventListener<GameActor>(ActorNameEvent.HideActorName, HideActorName);
        EventCenter.Instance.AddEventListener<GameActor>(ActorNameEvent.DestroyActorName, DestroyActorName);
        return true;
    }
    private void AddNewActorName(GameActor actor, string content)
    {
        m_pool.CreateEntity(actor, content);
        m_ActorNameInfoDic[actor] = Time.realtimeSinceStartup;
    }

    private void DestroyActorName(GameActor actor)
    {
        m_pool.DestroyEntity(actor);
        m_ActorNameInfoDic.Remove(actor);
    }

    public void HideActorName(GameActor actor)
    {
        m_pool.HideEntity(actor);
    }

    public Dictionary<GameActor, float> GetActorNameInfo()
    {
        return m_ActorNameInfoDic;
    }

    public override void OnLateUpdate()
    {
        m_pool.LateUpdate();
    }
}

public class ActorNamePool
{
    private const int MaxCacheCount = 5;
    private List<ActorNameEntity> m_listItem = new List<ActorNameEntity>();
    private Dictionary<GameObject, ActorNameEntity> m_itemDic = new Dictionary<GameObject, ActorNameEntity>();
    private Queue<ActorNameEntity> m_cacheQueue = new Queue<ActorNameEntity>();

    public void CreateEntity(GameActor actor, string content)
    {
        if (actor == null)
        {
            return;
        }
        if (actor.gameObject == null)
        {
            return;
        }
        ActorNameEntity item;
        if (!m_itemDic.TryGetValue(actor.gameObject, out item))
        {
            item = m_cacheQueue.Count > 0 ? m_cacheQueue.Dequeue() : CreateNewEntity();
            item.Actor = actor;
            m_itemDic.Add(actor.gameObject, item);
            m_listItem.Add(item);
        }
        item.ReInit(actor.gameObject, content);
    }

    private string m_itemPath = "UI/ActorNameEntity";
    private string m_rootTransPath = "UI/ActorNameRoot";
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

    private ActorNameEntity CreateNewEntity()
    {
        GameObject go = DResources.AllocGameObject(m_itemPath);
        var transform = go.transform;
        transform.SetParent(RootTrans);
        //UnityUtil.SetLayer(transform, GameLayer.ActorLayer);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        return new ActorNameEntity(go);
    }

    public void DestroyEntity(GameActor actor)
    {
        if (actor.gameObject == null)
        {
            return;
        }
        ActorNameEntity item;
        if (m_itemDic.TryGetValue(actor.gameObject, out item))
        {
            m_itemDic.Remove(actor.gameObject);
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

    public void HideEntity(GameActor actor)
    {
        if (actor.gameObject == null)
        {
            return;
        }

        ActorNameEntity item;
        if (m_itemDic.TryGetValue(actor.gameObject, out item))
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