
using UnityEngine;

class ActorNameCmpt : MonoBehaviour
{
    public void Init(GameActor actor,string actorName)
    {
        OwnActor = actor;
        ActorName = actorName;
        AddActorName(actorName);
    }

    private ActorNameMgr m_mgr
    {
        get { return ActorNameMgr.Instance; }
    }

    public GameActor OwnActor;

    private string ActorName;


    private float m_wordDestroyTime;                    // 最近一次台词消失时间
    private float m_nextNewWordStarTime;                // 下一次台词开始时间
    private bool m_isWordDestroy = true;                 // 台词是否被删除了的标识
    private const float FIRST_WORD_START_WAIT_TIME = 5;    // 第一句台词开始前需要等待的时间
    private GameTimer m_timerUpdateBubble;


    protected void Start()
    {
        var randomDurTime = UnityUtil.RandomRangeFloat(0.5f, 2f);
        GameTimerMgr.Instance.CreateLoopTimer(ref m_timerUpdateBubble, "UpdateActorName", randomDurTime, UpdateActorName);
    }

    private void UpdateActorName()
    {
        GameActor mainActor = ActorMgr.Instance.GetMainPlayer();
        if (mainActor == null)
        {
            return;
        }

        // 玩家和OwnActor距离
        float sqrtDist = Vector3.SqrMagnitude(mainActor.gameObject.transform.position - OwnActor.gameObject.transform.position);

        // 离开视野 防止对象销毁了来不及去掉台词，这里比视野小一点点
        if (sqrtDist > 200 - 1)
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
                AddActorName(ActorName);
            }
        }
    }

    public void AddActorName(string content)
    {
        var info = m_mgr.GetActorNameInfo();

        if (info.ContainsKey(OwnActor))
        {
            return;
        }

        m_nextNewWordStarTime = Time.realtimeSinceStartup + 1;

        m_wordDestroyTime = 3 + Time.realtimeSinceStartup;

        //info.Add(OwnActor, m_nextNewWordStarTime);

        EventCenter.Instance.EventTrigger<GameActor, string>(ActorNameEvent.AddNewActorName, OwnActor, content);
    }

    // 删除实例
    private void DestroyWordInst()
    {
        var info = m_mgr.GetActorNameInfo();
        if (info.ContainsKey(OwnActor))
        {
            EventCenter.Instance.EventTrigger<GameActor>(ActorNameEvent.DestroyActorName, OwnActor);
            info.Remove(OwnActor);
        }
    }

    protected void OnDestroy()
    {
        DestroyWordInst();

        GameTimerMgr.Instance.DestroyTimer(ref m_timerUpdateBubble);
    }
}
