using UnityEngine;

public class BBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool sHaveInstanced;
    private static T sInstance;

    public static T Instance
    {
        get
        {
            if ((Object)BBehaviourSingleton<T>.sInstance == (Object)null && !BBehaviourSingleton<T>.sHaveInstanced)
            {
                if (!Application.isPlaying)
                    return default(T);
                Transform rootGo = UBehhaviourRoot.GetRootGo();
                string str = "_" + typeof(T).ToString();
                Transform transform = rootGo.Find(str);
                if ((Object)transform == (Object)null)
                    BBehaviourSingleton<T>.sInstance = new GameObject(str)
                    {
                        transform = {
                            parent = rootGo
                        }
                    }.AddComponent<T>();
                else
                    BBehaviourSingleton<T>.sInstance = transform.gameObject.GetComponent<T>();
                BBehaviourSingleton<T>.sHaveInstanced = true;
            }
            return BBehaviourSingleton<T>.sInstance;
        }
    }

    public static bool HasInstance
    {
        get
        {
            return (Object)BBehaviourSingleton<T>.sInstance != (Object)null;
        }
    }
}