using UnityEngine;
using UnityEngine.UI;

class RoomUI : UIWindow
{
	#region 脚本工具生成的代码
	private InputField m_inputRoomName;
	private Button m_btnFindRoom;
	private Button m_btnJoinRoom;
	private Button m_btnCreateRoom;
	private Button m_btnStartRoom;
	protected override void ScriptGenerator()
	{
		m_inputRoomName = FindChildComponent<InputField>("m_inputRoomName");
		m_btnFindRoom = FindChildComponent<Button>("m_btnFindRoom");
		m_btnJoinRoom = FindChildComponent<Button>("m_btnJoinRoom");
		m_btnCreateRoom = FindChildComponent<Button>("m_btnCreateRoom");
		m_btnStartRoom = FindChildComponent<Button>("m_btnStartRoom");
		m_btnFindRoom.onClick.AddListener(OnClickFindRoomBtn);
		m_btnJoinRoom.onClick.AddListener(OnClickJoinServerBtn);
		m_btnCreateRoom.onClick.AddListener(OnClickCreateRoomBtn);
		m_btnStartRoom.onClick.AddListener(OnClickStartRoomBtn);
	}
    #endregion

    #region 事件
    private void OnClickFindRoomBtn()
    {
        RoomDataMgr.Instance.FindRoomReq();
    }
    private void OnClickJoinRoomBtn()
    {
        RoomDataMgr.Instance.JoinRoomReq(m_inputRoomName.text);
    }
    private void OnClickCreateRoomBtn()
    {
        RoomDataMgr.Instance.CreateRoomReq(m_inputRoomName.text, 999);
    }
    private void OnClickStartRoomBtn()
	{
		GameDataMgr.Instance.StartGameReq();
	}

    private void OnClickJoinServerBtn()
    {
        RoomDataMgr.Instance.JoinRoomReq(1.ToString());
    }
	#endregion

}
