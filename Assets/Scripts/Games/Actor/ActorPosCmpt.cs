using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

class ActorPosCmpt : MonoBehaviour
{
    private ThirdPersonUserControl m_playerController;

    void Start()
    {
        InvokeRepeating("UpPosFun", 1, 1f / 30f);
        m_playerController = gameObject.GetComponent<ThirdPersonUserControl>();
    }

    private int m_animation;
    private int Dirt;

    private void UpPosFun()
    {
        if (m_playerController == null)
        {
            return;
        }
        m_animation = GetAnimationToInt();
        Dirt = Convert.ToInt32(m_playerController.Crouch);
        Vector3 pos = this.transform.position;
        float characterRot = transform.eulerAngles.y;

        Vector3 moveVector3 = m_playerController.Move;

        //ActorDataMgr.Instance.UpPosReq(pos, moveVector3, characterRot, m_animation, Dirt);
        ActorDataMgr.Instance.UpCachePosReq(pos, moveVector3, characterRot, m_animation, Dirt);
    }

    private int GetAnimationToInt()
    {
        if (m_playerController.IsAttack)
        {
            return (int)AnimDefine.IsAttack;
        }
        if (m_playerController.IsSkill)
        {
            return (int)AnimDefine.IsSkill;
        }
        if (m_playerController.Jump)
        {
            return (int)AnimDefine.IsJump;
        }
        if (m_playerController.IsDefence)
        {
            return (int)AnimDefine.IsDefence;
        }

        return 0;
    }
}
