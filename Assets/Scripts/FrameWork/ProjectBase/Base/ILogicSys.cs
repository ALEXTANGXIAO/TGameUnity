public interface ILogicSys
{
    bool OnInit();

    void OnDestroy();

    void OnStart();

    void OnUpdate();

    void OnLateUpdate();

    void OnRoleLogout();

    void OnPause();

    void OnResume();
}