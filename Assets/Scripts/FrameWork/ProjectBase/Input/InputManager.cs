using UnityEngine;

class InputManager : Singleton<InputManager>
{
    private bool isOpen = false;

    /// <summary>
    /// 构造函数中添加Update监听
    /// </summary>
    public InputManager()
    {
        MonoManager.Instance.AddUpdateListener(Update);
    }

    /// <summary> 
    /// 是否开启或者关闭 输入检测
    /// </summary>
    public void StatOrEndCheck(bool isopen)
    {
        isOpen = isopen;
    }

    public void Update()
    {
        if (!isOpen)
        {
            return;
        }
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.D);
    }

    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            EventCenter.Instance.EventTrigger(InputEvent.KeyDown, key);
        }

        if (Input.GetKeyUp(key))
        {
            EventCenter.Instance.EventTrigger(InputEvent.KeyUp, key);
        }
    }
}
