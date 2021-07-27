using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class DResources
{
    public static GameObject AllocGameObject(string path)
    {
        var obj =ResourcesManager.Instance.AllocGameObject(path);

        return obj;
    }
}
