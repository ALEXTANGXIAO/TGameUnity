using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using UnityEngine;

class ActorFightMgr:BehaviourSingleton<ActorFightMgr>
{
    public override void Awake()
    {
        Init();
        RegisterEvent();
    }

    private void Init()
    {
        
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener<GameActor>(ActorEvent.AttackHandle, AttackHandle);
        EventCenter.Instance.AddEventListener<GameActor>(ActorEvent.HeartHandle, HeartHandle);
    }

    private void AttackHandle(GameActor actor)
    {
        if (actor ==null ||actor.Character == null)
        {
            return;
        }
        actor.Character.StandStill();
    }

    private void HeartHandle(GameActor actor)
    {
        if (actor == null || actor.Character == null)
        {
            return;
        }
        actor.Character.StandStill();
    }
}
