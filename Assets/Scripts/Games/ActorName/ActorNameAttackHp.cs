using UnityEngine.UI;

class ActorNameAttackHp : ActorNameBase
{
    private Image m_hp;

    protected override void OnInit()
    {
        m_hp = UnityUtil.FindChildComponent<Image>(m_tfNode, "m_goHp/m_dimgbg/m_dimghp");
    }

    public override void UpdateInfo(GameActor actor)
    {
    }

    public void Update(GameActor mActor)
    {
        if (mActor != null)
        {
            //var hpPct = ActorDataUtil.GetHpValue(mActor);
            m_hp.fillAmount = 1;
        }
    }

    public override void OnVisibleStateChange(bool visible)
    {
        m_hp.enabled = visible;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_hp = null;
    }
}