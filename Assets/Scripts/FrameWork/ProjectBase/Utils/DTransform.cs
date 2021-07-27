using UnityEngine;

/// <summary>
/// 加速一些transform的属性操作
/// </summary>
public class DTransform
{
    public static void ResetLocalPosScaleRot(Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    public static void ResetLocalPosScale(Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
    }

    public static void SetLocalScale(Transform trans, float x, float y, float z)
    {
        trans.localScale = new Vector3(x, y, z);
    }

    public static void SetLocalPosition(Transform trans, float x, float y, float z)
    {
        trans.localPosition = new Vector3(x, y, z);
    }

    public static void SetPosition(Transform trans, float x, float y, float z)
    {
        trans.position = new Vector3(x, y, z);
    }

    public static void SetPosition(Transform trans, Transform src)
    {
        trans.position = src.position;
    }

    public static void SetForward(Transform trans, float x, float y, float z)
    {
        trans.forward = new Vector3(x, y, z);
    }

    public static void SetRotation(Transform trans, Transform srcTrans)
    {
        trans.rotation = srcTrans.rotation;
    }
    public static void SetLocalRotation(Transform trans, Transform srcTrans)
    {
        trans.localRotation = srcTrans.localRotation;
    }

    public static void ResetLocalRotation(Transform tran)
    {
        tran.localRotation = Quaternion.identity;
    }

    private static float minScale = 0.5f;//最小缩放系数
    private static float maxScale = 1f;//最大缩放系数
    private static float minScaleDistance = 25f;//最小缩放距离
    private static float maxScaleDistance = 1f;//最大缩放距离

    /// <summary>
    /// 保证屏幕的比例和实际设计的比例一致
    /// </summary>
    /// <returns></returns>
    public static float CalUIScale(Camera camera, Transform cameraTrans, Transform rectTrans)
    {
        Vector2 sizeDelta = new Vector2(100f, 100);

        float screenScale = camera.pixelHeight / 640.0f;
        float expectTransWidth = sizeDelta.x * screenScale;
        Vector3 cameraNormalForward = cameraTrans.forward.normalized;
        float distance = Vector3.Dot(rectTrans.position - cameraTrans.position, cameraNormalForward);

        Vector3 centerScreenPos = camera.WorldToScreenPoint(rectTrans.position);

        ///然后计算在屏幕上这么大的尺寸，需要实际多远的距离
        Vector3 pt1 = camera.ScreenToWorldPoint(new Vector3(centerScreenPos.x - expectTransWidth / 2, centerScreenPos.y, distance));
        Vector3 pt2 = camera.ScreenToWorldPoint(new Vector3(centerScreenPos.x + expectTransWidth / 2, centerScreenPos.y, distance));
        Vector3 vec = pt2 - pt1;
        float expectWidth = vec.magnitude;
        float scale = expectWidth / sizeDelta.x;
        float t = Mathf.InverseLerp(minScaleDistance, maxScaleDistance, distance);
        float distanceScale = Mathf.Lerp(minScale, maxScale, t);
        return scale * distanceScale;
    }

    public static Vector3 GetNamePos(Vector3 targetPos, Vector3 cameraPos, float radius)
    {
        var dir = (cameraPos - targetPos).normalized;
        var horDir = new Vector3(dir.x, 0, dir.z).normalized;
        var cosAngle = Vector3.Dot(dir, horDir);
        return targetPos + dir.normalized * radius / cosAngle;
    }

    public static void GetTransformEular(Transform trans, out float x, out float y, out float z)
    {
        var euler = trans.eulerAngles;
        x = euler.x;
        y = euler.y;
        z = euler.z;
    }

    //public static void UpdateVertexOnGround(Mesh mesh, Transform rootTrans, int groundLayer)
    //{
    //    var rootPos = rootTrans.position;
    //    Vector3[] vertices = mesh.vertices;
    //    for (int i = 0; i < vertices.GetLength(0); i++)
    //    {
    //        var vert = vertices[i];
    //        Vector3 newpos = new Vector3();
    //        Vector3 pp1 = vert + rootPos;
    //        bool rt = DPhysics.CastPositionOntoGround(pp1, ref newpos, groundLayer);
    //        if (rt == false)
    //        {
    //            newpos.y = rootPos.y;
    //        }

    //        vert.y = newpos.y - rootPos.y + 0.12f;
    //        vertices[i] = vert;
    //    }

    //    mesh.vertices = vertices;
    //    mesh.RecalculateBounds();
    //}

    public static void UpdateVertexScale(Mesh mesh, float scale)
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x *= scale;
            vertices[i].z *= scale;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    public static void GetTransformEularRotation(Transform trans, out float sin, out float cos)
    {
        float CemeraYRotationAngle = trans.eulerAngles.y * Mathf.Deg2Rad;
        sin = (float)Mathf.Sin(CemeraYRotationAngle);
        cos = (float)Mathf.Cos(CemeraYRotationAngle);

    }

    public static Vector3 GetXZForward(Vector3 pos1, Vector3 pos2)
    {
        var forawrd = pos1 - pos2;
        return new Vector3(forawrd.x, 0, forawrd.z);
    }

    public static void BindTransform(Transform tranActorBind, Transform transHorseBind)
    {
        if (tranActorBind == null || transHorseBind == null || tranActorBind.parent == null)
        {
            return;
        }

        var mat2 = Matrix4x4.TRS(tranActorBind.localPosition, Quaternion.identity, Vector3.one).inverse;
        tranActorBind.parent.position = mat2.MultiplyPoint(transHorseBind.position);
        tranActorBind.parent.rotation = transHorseBind.rotation;
        tranActorBind.parent.Rotate(Vector3.right, 90);
    }
}
