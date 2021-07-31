using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class WeaponData : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        ACTIVE,
        DEFACTIVE
    }
    public STATE state;

    public float ATK;

    public InterActionCmpt ImActionCmpt;

    public ThirdPersonCharacter Character;

    [System.NonSerialized]
    public CapsuleCollider Capcollider;

    private void Start()
    {
        Capcollider = this.GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider collider)
    {
#if UNITY_EDITOR
        //print(this.name + "is hitting" + collider.name);
        //print(im);
#endif
        bool IsWeapon = collider.tag.Equals("Weapon");
        bool IsPlayer= collider.tag.Equals("Player");
        if (!IsPlayer &&!IsWeapon)
        {
            return;
        }

        //攻击方
        if (state == STATE.ACTIVE)
        {
            InterActionCmpt colliderActionCmpt = collider.GetComponent<InterActionCmpt>();
            if (colliderActionCmpt != null)
            {
                StandStill();
                colliderActionCmpt.BeAttacked(ImActionCmpt);

            }
            //砍中武器了
            else if (IsWeapon)
            {
                Capcollider.enabled = false;
                state = STATE.IDLE;
                StartCoroutine(BeBlocked());
            }
        }
        //被攻击方
        if (state == STATE.DEFACTIVE)
        {
            if (IsWeapon)
            {
                Character.Animator.SetBool(AnimatorParamDefine.DefenceSuccess, true);
                Invoke("ResetDefence",0.5f);
            }
            print(this.name + "is DEFACTIVE" + collider.name);
        }
    }

    private void StandStill(float stilltime = 0.12f)
    {
        PlayerSound();
        if (Character != null)
        {
            Character.StandStill(stilltime);
        }
        //EventCenter.Instance.EventTrigger(ActorEvent.StandStill);
    }

    private void PlayerSound()
    {
        AudioMgr.Instance.PlaySound("hit2");
    }

    float timer = 0;
    float animaSpeed = 1;

    /// <summary>
    /// 被格挡了
    /// </summary>
    /// <returns></returns>
    public IEnumerator BeBlocked()
    {
        StandStill(0.2f);
        Debug.LogError("Be Blocked");
        AudioMgr.Instance.PlaySound("IsDefence");
        //OnAnimation_CloseWeaponCollier();       //关闭武器碰撞盒
        timer = 0;
        while (timer < 0.8f)                    //弹反动画播放0.8s
        {
            SetAnimaSpeed();
            yield return new WaitForFixedUpdate();
        }
        Character.Animator.SetBool(AnimatorParamDefine.IsAttack,false);
        //hurt//Damage_Front_Big_ver_A
        Character.Animator.CrossFade("Grounded", 0,0,0);
        Character.Animator.SetFloat("AnimaSpeed", 1);               //播放速度恢复正常
    }

    //设置格挡回弹时的速度
    public void SetAnimaSpeed()
    {
        timer += Time.fixedDeltaTime;                   //计时
        animaSpeed = Character.AnimaSpeedCur.Evaluate(timer);     //读取曲线数据
        Character.Animator.SetFloat("AnimaSpeed", animaSpeed);    //设置播放速度
    }

    private void ResetDefence()
    {
        Character.Animator.SetBool(AnimatorParamDefine.DefenceSuccess, false);
    }
}
