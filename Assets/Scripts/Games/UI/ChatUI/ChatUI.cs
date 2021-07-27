using System;
using UnityEngine;
using UnityEngine.UI;

class ChatUI : UIWindow
{
    #region 脚本工具生成的代码
    private InputField m_inputMsg;
    private Button m_btnSend;
    private Button m_btnSetting;
    protected override void ScriptGenerator()
    {
        m_inputMsg = FindChildComponent<InputField>("m_inputMsg");
        m_btnSend = FindChildComponent<Button>("m_btnSend");
        m_btnSetting = FindChildComponent<Button>("m_btnSetting");
        m_btnSend.onClick.AddListener(OnClickSendBtn);
        m_btnSetting.onClick.AddListener(OnClickSettingBtn);
    }


    #endregion

    #region 事件
    private void OnClickSendBtn()
    {
        if (String.IsNullOrEmpty(m_inputMsg.text))
        {
            return;
        }
        ChatDataMgr.Instance.ChatReq(m_inputMsg.text);
    }

    private void OnClickSettingBtn()
    {
        UISys.Mgr.ShowWindow<SettingUI>();
    }
    #endregion

}