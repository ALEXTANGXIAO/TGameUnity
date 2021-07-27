using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed partial class GameApp : UnitySingleton<GameApp>
{
    public int TargetFrameRate = 300;

    public string Host
    {
        get
        {
            switch (hostPoint)
            {
                case (HostPoint.LocalHost):
                    return "127.0.0.1";
                case (HostPoint.LinuxServer):
                    return "47.106.96.238";
                case (HostPoint.WinServer):
                    return "1.14.132.143";
            }
            return "127.0.0.1";
        }
    }


    public HostPoint hostPoint = HostPoint.LocalHost;
    public enum HostPoint
    {
        LocalHost,
        LinuxServer,
        WinServer,
    }

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        InitLog();
        TDebug.DEBUG(ColorUtil.R(ColorType.PINK, "========INIT GAMEAPP========="));
        SetTargetFrameRate();
        InitLibImp();
        RegistAllSystem();
        UISys.Mgr.ShowWindow<MsgUI>(UI_Layer.Top);
        UISys.Mgr.ShowWindow<LoginUI>(UI_Layer.Mid);
        UISys.Mgr.ShowWindow<FPS>(UI_Layer.Top);

#if UNITY_ANDROID || UNITY_IPHONE
        //var gameObject = ResourcesManager.Instance.AllocGameObject("UI/TouchpadUI");
        //gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
        UISys.Mgr.ShowWindow<TouchpadUI>(UI_Layer.Bottom);
        UISys.Mgr.ShowWindow<JoyStickUI>(UI_Layer.Bottom);
        TDebug.Log("UNITY_ANDROID/UNITY_IPHONE");
#endif
    }
}
