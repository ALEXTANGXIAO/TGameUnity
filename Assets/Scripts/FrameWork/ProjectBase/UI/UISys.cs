using System.Collections.Generic;
/// <summary>
/// UISystem 实现UIManager的生命周期，可注册Controller实现MVC结构，以及MVVM结构实现双向绑定
/// </summary>
class UISys:BaseLogicSys<UISys>
{
    private List<IUIController> m_listController = new List<IUIController>();

    public static UIManager Mgr
    {
        get { return UIManager.Instance; }
    }

    public override void OnUpdate()
    {
        UIManager.Instance.Update();
    }

    public override bool OnInit()
    {
        base.OnInit();

        RegistAllController();

        return true;
    }

    private void RegistAllController()
    {
        AddController<LoadingUIController>();
    }

    private void AddController<T>() where T : IUIController, new()
    {
        for (int i = 0; i < m_listController.Count; i++)
        {
            var type = m_listController[i].GetType();

            if (type == typeof(T))
            {
                TDebug.Log("repeat controller type: {0}", typeof(T).Name);

                return;
            }
        }

        var controller = new T();

        m_listController.Add(controller);

        controller.ResigterUIEvent();
    }
}

