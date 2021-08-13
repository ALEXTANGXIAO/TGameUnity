using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public partial class GameActor
{
    public GameActor(GameObject obj)
    {
        m_gameObject = obj;
        gameObject = m_gameObject;
        Character = m_gameObject.GetComponent<ThirdPersonCharacter>();
        Character.GameActor = this;
        gameObject.GetComponent<ActorWeapon>().actor = this;
        ResigterEvent();
    }

    private void ResigterEvent()
    {
        EventCenter.Instance.AddEventListener<GameActor,float>(ActorEvent.HeartHandle, HeartHandle);
    }

    private void HeartHandle(GameActor actor,float value)
    {
        if (this != actor)
        {
            return;
        }
        NameEntity.HurtHandle(value);
    }

    private GameObject m_gameObject;

    public ThirdPersonCharacter Character;
    public GameObject gameObject
    {
        get;
        private set;
    }

    public ActorNameEntity NameEntity;

    public Vector3 Position
    {
        get { return m_gameObject.transform.position; }
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
