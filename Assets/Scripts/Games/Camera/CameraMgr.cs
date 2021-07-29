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
    private ShakeCamera shakeHandle;
    public override void Awake()
    {
        Init();
        RegisterEvent();
    }

    private void Init()
    {
        mainCamera = Camera.main;
        shakeHandle = mainCamera.GetComponent<ShakeCamera>();
        //MonoManager.Instance.DontDestroyOnLoad(mainCamera.gameObject);
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.AddEventListener<Camera>("ChangeCamera", ChangeCamera);
        EventCenter.Instance.AddEventListener<GameActor,float,float>(CameraEvent.ShakeCamera, ShakeCamera);
        //GameClient.Instance.RegActionHandle((int)ActionCode.AddCharacter, AddCharacter);
        //GameClient.Instance.RegActionHandle((int)ActionCode.UpCharacterList, RemoveCharacter);
    }
    private void ShakeCamera(GameActor actor,float shakeLevel,float setShakeTime)
    {
        if (actor == null || !ActorMgr.Instance.IsMainPlayer(actor))
        {
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (shakeHandle == null)
        {
            shakeHandle = mainCamera.GetComponent<ShakeCamera>();
        }

        if (shakeHandle != null)
        {
            shakeHandle.shakeLevel = shakeLevel;
            shakeHandle.setShakeTime = setShakeTime;
            shakeHandle.enabled = true;
        }
    }

    public void ShakeTest()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (shakeHandle == null)
        {
            shakeHandle = mainCamera.GetComponent<ShakeCamera>();
        }

        if (shakeHandle != null)
        {
            shakeHandle.shakeLevel = 2f;
            shakeHandle.setShakeTime = 0.2f;
            shakeHandle.enabled = true;
        }
    }

    private void ChangeCamera(Camera camera)
    {
        mainCamera = camera;
        shakeHandle = mainCamera.GetComponent<ShakeCamera>();
    }
}
