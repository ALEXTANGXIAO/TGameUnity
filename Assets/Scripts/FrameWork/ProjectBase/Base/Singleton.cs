using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T: new()
{
    public static T _instance;

    public static T Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
    public static bool OnInit()
    {
        if (null == _instance)
        {
            _instance = new T();
        }
        return true;
    }
}
