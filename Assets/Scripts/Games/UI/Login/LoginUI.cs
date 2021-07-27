
using System;
using UnityEngine;
using UnityEngine.UI;

class LoginUI : UIWindow
{
	#region 脚本工具生成的代码
	private InputField m_inputPassword;
	private InputField m_inputUserName;
	private Button m_btnLogin;
	private Button m_btnResigter;
	protected override void ScriptGenerator()
	{
		m_inputPassword = FindChildComponent<InputField>("m_inputPassword");
		m_inputUserName = FindChildComponent<InputField>("m_inputUserName");
		m_btnLogin = FindChildComponent<Button>("m_btnLogin");
		m_btnResigter = FindChildComponent<Button>("m_btnResigter");
		m_btnLogin.onClick.AddListener(OnClickLoginBtn);
		m_btnResigter.onClick.AddListener(OnClickResigterBtn);
	}
	#endregion

	#region 事件
	private void OnClickLoginBtn()
	{
        if (m_inputUserName.text.Equals(String.Empty)|| m_inputPassword.text.Equals(String.Empty))
        {
            return;
        }
        LoginDataMgr.Instance.LoginReq(m_inputUserName.text, m_inputPassword.text);
	}
	private void OnClickResigterBtn()
	{
        if (m_inputUserName.text.Equals(String.Empty) || m_inputPassword.text.Equals(String.Empty))
        {
            return;
        }
		LoginDataMgr.Instance.ResigterReq(m_inputUserName.text, m_inputPassword.text);
	}
	#endregion

}
