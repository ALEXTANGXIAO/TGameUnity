using System;


public class LoadingUIController : IUIController
{
    public void ResigterUIEvent()
    {
        EventCenter.Instance.AddEventListener("LoadingUI.Load", ShowLoadingUI);
        EventCenter.Instance.AddEventListener("LoadingUI.Close", CloseLoadingUI);
    }

    private void CloseLoadingUI()
    {
        UISys.Mgr.CloseWindow<LoadingUI>();
    }

    private void ShowLoadingUI()
    {
        var ui = UISys.Mgr.ShowWindow<LoadingUI>();
    }
}
