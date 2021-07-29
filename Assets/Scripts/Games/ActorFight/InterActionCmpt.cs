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
    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        Character = gameObject.GetComponent<ThirdPersonCharacter>();
    }

    public bool BeAttacked(InterActionCmpt attacker)
    {
        if (attacker == this)
        {
            return false;
        }

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
}
