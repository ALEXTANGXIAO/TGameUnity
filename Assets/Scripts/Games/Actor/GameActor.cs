using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class GameActor
{
    public GameActor(GameObject obj)
    {
        m_gameObject = obj;
        gameObject = m_gameObject;
        Character = m_gameObject.GetComponent<ThirdPersonCharacter>();
    }

    private GameObject m_gameObject;

    public ThirdPersonCharacter Character;
    public GameObject gameObject
    {
        get;
        private set;
    }
    public uint ActorID { get; internal set; }

    private ActorOnlineCmpt m_ActorOnlineCmpt;
    public ActorOnlineCmpt GetActorOnlineCmpt
    {
        get
        {
            if (m_ActorOnlineCmpt == null)
            {
                m_ActorOnlineCmpt = gameObject.GetComponent<ActorOnlineCmpt>();
            }

            return m_ActorOnlineCmpt;
        }
    }
}
