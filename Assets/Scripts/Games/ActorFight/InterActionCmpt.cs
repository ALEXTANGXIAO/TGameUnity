using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class InterActionCmpt : MonoBehaviour
{
    private Animator animator;
    [System.NonSerialized]
    public ThirdPersonCharacter Character;

    private ActorWeapon actorWeapon;

    public Vector3 WeaponPosition
    {
        get { return actorWeapon.weaponData.transform.position; }
    }
    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        Character = gameObject.GetComponent<ThirdPersonCharacter>();
        actorWeapon = gameObject.GetComponent<ActorWeapon>();
    }

    public bool BeAttacked(InterActionCmpt attacker)
    {
        if (attacker == this)
        {
            return false;
        }

        CheckAttackDir(attacker.WeaponPosition);
        var damege = attacker.gameObject.GetComponent<ActorWeapon>().GetAtk();
#if UNITY_EDITOR
        print("Do Damage is called.:" + attacker.name + "is attacking" + name + " Damege is:" + damege);
#endif
        if (animator!= null)
        {
            if (Character != null)
            {
                Character.StandStill();
            }
            animator = gameObject.GetComponent<Animator>();
            animator.SetTrigger(AnimatorParamDefine.IsHurt);
        }
        return true;
    }

    //检测攻击方向
    void CheckAttackDir(Vector3 weaponPosition)
    {
        Vector3 Dir = weaponPosition - transform.position;      //从玩家指向敌人攻击源
        float angle = Vector3.Angle(transform.right, Dir);      //计算Dir与角色右方的夹角
        Debug.Log(angle);
        if (angle < 90)                                         //夹角<90度往右格挡
        {
            //isRightB = true;
            Debug.Log("Is isRight");
        }
        else
        {
            //isRightB = false;                                   //否则往左格挡
            Debug.Log("Is isLeft");
        }
    }
}



public class DamageCheckEventArgs : EventArgs
{
    public enum DamageType
    {
        Instant = 0,
        Delay = 1,
    }

    public enum EquipmentStatus
    {
        
    }
    public DamageType damageType;
    public EquipmentStatus senderEquip;
    public Vector3 senderPos;
    public int senderDirect;
    public int senderCamp;

    public void StandStill(float time)
    {
        //UpperAnim[playingAnimString].speed = 0f;
        //standAnimString = playingAnimString;
        //standstillTime = time;
        //Debug.Log(playingAnimString);
    }

    public class EventManager : UnitySingleton<EventManager>
    {
        public delegate void EventHandler(object sender, DamageCheckEventArgs e);
        public event EventHandler DamageCheckEvent;

        public void DamageCheck(object sender, DamageCheckEventArgs e)
        {
            DamageCheckEvent(sender, e);
        }
    }
}