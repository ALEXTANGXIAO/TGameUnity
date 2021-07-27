using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class WeaponData : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        ACTIVE
    }
    public STATE state;

    public float ATK;

    public InterActionCmpt ImActionCmpt;

    public ThirdPersonCharacter Character;

    private void OnTriggerEnter(Collider collider)
    {
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
        }
    }

    private void StandStill()
    {
        PlayerSound();
        if (Character!= null)
        {
            Character.StandStill();
        }
        //EventCenter.Instance.EventTrigger(ActorEvent.StandStill);
    }

    private void PlayerSound()
    {
        AudioMgr.Instance.PlaySound("hit2");
    }
}
