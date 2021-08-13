using UnityEngine;

class TargetLockCmpt
{
    public Transform enemy;
    public Transform player;
    public float cameraSlack;
    public float cameraDistance;

    private Vector3 pivotPoint;
    private GameActor m_actor;
    private Transform Camtransform;

    void Init(GameActor actor)
    {
        m_actor = actor;
        player = m_actor.gameObject.transform;
        Camtransform = Camera.main.transform;
        MonoManager.Instance.AddUpdateListener(Update);
    }

    void Update()
    {
        Vector3 current = pivotPoint;
        Vector3 target = player.transform.position + Vector3.up;
        pivotPoint = Vector3.MoveTowards(current, target, Vector3.Distance(current, target) * cameraSlack);

        Camtransform.position = pivotPoint;
        Camtransform.LookAt((enemy.position + player.position) / 2);
        Camtransform.position -= Camtransform.forward * cameraDistance;
    }
}
