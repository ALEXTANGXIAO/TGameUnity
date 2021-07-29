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
        bool IsWeapon = collider.gameObject.tag.Equals("Weapon");
        //攻击方
        if (state == STATE.ACTIVE)
        {
            InterActionCmpt colliderActionCmpt = collider.GetComponent<InterActionCmpt>();
            if (colliderActionCmpt != null)
            {
                StandStill();
                colliderActionCmpt.BeAttacked(ImActionCmpt);
#if UNITY_EDITOR
                //print(this.name + "is hitting" + collider.name);
                //print(im);
#endif
            }
            else if (IsWeapon)
            {
                Capcollider.enabled = false;
                state = STATE.IDLE;
                SetAnimaSpeed();
            }
        }
        //被攻击方
        if (state == STATE.DEFACTIVE)
        {
            if (IsWeapon)
            {
                Character.Animator.SetBool(AnimatorParamDefine.DefenceSuccess, true);
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
    //设置格挡回弹时的速度
    public void SetAnimaSpeed()
    {
        timer += Time.fixedDeltaTime;                   //计时
        animaSpeed = Character.AnimaSpeedCur.Evaluate(timer);     //读取曲线数据
        Character.Animator.SetFloat("AnimaSpeed", animaSpeed);    //设置播放速度
    }
}
