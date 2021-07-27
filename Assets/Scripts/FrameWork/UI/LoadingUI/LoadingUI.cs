using UnityEngine;
using UnityEngine.UI;


class LoadingUI:UIWindow
{
    private Text Loadingtext;
    private Image LoadingProgressBar;
    private Image Logo;
    private GameTimerTick m_timerTick;
    protected override void ScriptGenerator()
    {
        Logo = FindChildComponent<Image>("Logo");
        Loadingtext = FindChildComponent<Text>("LoadingText");
        LoadingProgressBar = FindChildComponent<Image>("LoadingProgressBar");
    }

    protected override void RegisterEvent()
    {
        AddUIEvent<float>("Load", Loading);
        AddUIEvent("Test", Test);
    }

    private void Test()
    {
        Debug.Log("Test");
    }

    protected override void OnCreate()
    {
        m_timerTick = new GameTimerTick(1f, OnTick);
        UISprite.SetSprite(Logo,"pr_4_6");
    }

    private void OnTick()
    {
        //transform.DOShakePosition(1, new Vector3(3, 3, 0));
    }

    protected override void OnUpdate()
    {
        if (m_timerTick != null)
        {
            m_timerTick.OnUpdate();
        }
    }

    private void Loading(float info)
    {
        LoadingProgressBar.fillAmount = info;
    }
}
