using UnityEngine;
using UnityEngine.UI;

class SettingUI : UIWindow
{
    private GameTimerTick m_timerTick;
    #region 脚本工具生成的代码
    private Text m_textBgMusic;
    private Text m_textBtnMusic;
    private Slider m_sliderBg;
    private Slider m_sliderBtn;
    private Button m_btnClose;
    protected override void ScriptGenerator()
    {
        m_textBgMusic = FindChildComponent<Text>("m_textBgMusic");
        m_textBtnMusic = FindChildComponent<Text>("m_textBtnMusic");
        m_sliderBg = FindChildComponent<Slider>("m_sliderBg");
        m_sliderBtn = FindChildComponent<Slider>("m_sliderBtn");
        m_btnClose = FindChildComponent<Button>("m_btnClose");
        m_btnClose.onClick.AddListener(OnClickCloseBtn);
        m_sliderBg.onValueChanged.AddListener(BgValueChanged);
        m_sliderBtn.onValueChanged.AddListener(BtnValueChanged);
    }
    #endregion

    protected override void OnCreate()
    {
        m_timerTick = new GameTimerTick(1f, OnTick);
        m_sliderBg.value = AudioMgr.Instance.BGVALUE;
        m_sliderBtn.value = AudioMgr.Instance.SOUNDVALUE;
    }

    protected override void OnUpdate()
    {
        if (gameObject == null)
        {
            return;
        }

        if (m_timerTick != null)
        {
            m_timerTick.OnUpdate();
        }

        AudioMgr.Instance.SetBgValue(m_sliderBg.value);
        AudioMgr.Instance.SetSoundValue(m_sliderBtn.value);

    }

    private void OnTick()
    {
        saveData();
    }

    private void BgValueChanged(float value)
    {
        AudioMgr.Instance.SetBgValue(value);
    }

    private void BtnValueChanged(float value)
    {
        AudioMgr.Instance.SetSoundValue(value);
    }

    #region 事件
    private void OnClickCloseBtn()
    {
        UISys.Mgr.CloseWindow<SettingUI>();
    }
    #endregion


    private void saveData()
    {
        var saveData = ClientSaveData.Instance.GetSaveData<SettingSaveData>();

        saveData.m_saved = 1;
        saveData.m_bgvalue = m_sliderBg.value;
        saveData.m_soundvalue = m_sliderBtn.value;
        saveData.Save();
    }

    protected override void OnDestroy()
    {
        m_timerTick = null;
    }
}
