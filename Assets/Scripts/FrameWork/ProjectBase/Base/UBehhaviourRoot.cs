using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class UBehhaviourRoot
{
    private static Transform m_goRootTrans;

    public static Transform GetRootGo()
    {
        if ((Object)UBehhaviourRoot.m_goRootTrans == (Object)null)
        {
            GameObject gameObject = new GameObject("__instset_root");
            Object.DontDestroyOnLoad((Object)gameObject);
            UBehhaviourRoot.m_goRootTrans = gameObject.transform;
        }
        return UBehhaviourRoot.m_goRootTrans;
    }
}
