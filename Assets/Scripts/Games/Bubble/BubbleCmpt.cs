using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class BubbleCmpt:MonoBehaviour
{
    private BubbleMgr m_mgr
    {
        get { return BubbleMgr.Instance; }
    }

    public GameObject OwnActor
    {
        get
        {
            return this.gameObject;
        }
    }

    private float m_wordDestroyTime;    // 最近一次台词消失时间
    private float m_nextNewWordStarTime;    // 下一次台词开始时间
    private GameTimer m_timerUpdateBubble;


    protected void Start()
    {
        var randomDurTime = UnityUtil.RandomRangeFloat(0.5f, 2f);
        GameTimerMgr.Instance.CreateLoopTimer(ref m_timerUpdateBubble, "UpdateBubble", randomDurTime, UpdateBubble);
    }

    private void UpdateBubble()
    {
        GameActor mainActor = ActorMgr.Instance.GetMainPlayer();
        if (mainActor == null)
        {
            return;
        }

        // 玩家和OwnActor距离
        float sqrtDist = Vector3.SqrMagnitude(mainActor.gameObject.transform.position - OwnActor.transform.position);

        // 离开视野 防止对象销毁了来不及去掉台词，这里比视野小一点点
        if (sqrtDist > 200 - 1)
        {
            DestroyWordInst();
            return;
        }

        //够时间了
        if (m_wordDestroyTime > 0 && Time.realtimeSinceStartup >= m_wordDestroyTime)
        {
            DestroyWordInst();
            return;
        }

        // 距离够了
        if (sqrtDist <= 190)
        {
            // cd好了
            if (Time.realtimeSinceStartup >= m_nextNewWordStarTime)
            {
                //AddNewBubble("");
            }
        }
    }

    public void AddNewBubble(string content)
    {
        m_nextNewWordStarTime = Time.realtimeSinceStartup + 1;

        m_wordDestroyTime = 3 + Time.realtimeSinceStartup;

        EventCenter.Instance.EventTrigger<GameObject,string>(BubbleEvent.AddNewBubble,OwnActor,content);
    }

    // 删除冒泡实例
    private void DestroyWordInst()
    {
        var info = m_mgr.GetBubbleInfo();
        if (info.ContainsKey(OwnActor))
        {
            EventCenter.Instance.EventTrigger<GameObject>(BubbleEvent.DestroyBubble,OwnActor);
            info.Remove(OwnActor);
        }
    }

    protected void OnDestroy()
    {
        DestroyWordInst();

        GameTimerMgr.Instance.DestroyTimer(ref m_timerUpdateBubble);
    }
}
