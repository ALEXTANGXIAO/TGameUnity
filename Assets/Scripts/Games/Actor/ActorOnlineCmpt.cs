using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
public enum AnimDefine
{
    IsAttack = 1,
    IsSkill = 2,
    IsJump = 3,
    IsDefence = 4,
}

public class ActorOnlineCmpt : MonoBehaviour
{
    private int animation;
    private bool m_IsCrouch;
    private Vector3 selfPos;
    private Vector3 MoveVector3;
    private ThirdPersonCharacter m_Character;
    private float m_selfangle;
    private bool m_IsAttack = false;
    private bool m_IsSkill = false;
    private bool m_IsJump = false;
    private bool m_IsDefence = false;
    public void SetState(Vector3 selfpos,Vector3 moveVector3, float selfangle, int animation, int crouch)
    {
        selfPos = selfpos;
        MoveVector3 = moveVector3;
        this.animation = animation;
        this.m_IsCrouch = crouch == 1;
        m_IsAttack = animation == (int)AnimDefine.IsAttack;
        m_IsSkill = animation == (int)AnimDefine.IsSkill;
        m_IsDefence = animation == (int)AnimDefine.IsDefence;
        m_selfangle = selfangle;
    }

    private void Start()
    {
        this.transform.position = selfPos;
        this.transform.eulerAngles =new Vector3(0, m_selfangle,0); 

        m_Character = GetComponent<ThirdPersonCharacter>();
    }

    void FixedUpdate()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, selfPos, 0.25f); //selfPos; 
        this.transform.eulerAngles = new Vector3(0, m_selfangle, 0);   
        m_Character.Move(MoveVector3, m_IsCrouch, m_IsJump);
        m_Character.Battle(m_IsAttack, m_IsSkill, m_IsDefence);
    }
}

