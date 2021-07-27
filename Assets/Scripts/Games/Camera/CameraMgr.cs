using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using UnityEngine;


class CameraMgr: BehaviourSingleton<CameraMgr>
{
    private Camera mainCamera;
    public override void Awake()
    {
        Init();
        RegisterEvent();
    }

    private void Init()
    {
        mainCamera = Camera.main;
        //MonoManager.Instance.DontDestroyOnLoad(mainCamera.gameObject);
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener<Camera>("ChangeCamera", ChangeCamera);
        //GameClient.Instance.RegActionHandle((int)ActionCode.AddCharacter, AddCharacter);
        //GameClient.Instance.RegActionHandle((int)ActionCode.UpCharacterList, RemoveCharacter);
    }

    private void ChangeCamera(Camera camera)
    {
        mainCamera = camera;
    }
}
