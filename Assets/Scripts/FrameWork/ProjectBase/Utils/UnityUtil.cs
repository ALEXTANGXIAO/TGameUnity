﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngineInternal;
using Object = UnityEngine.Object;


/**
 * 封装Unity相关的一些通用接口
 */
public class UnityUtil
{
    [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
    public static Component AddMonoBehaviour(Type type, GameObject go)
    {
        var comp = go.GetComponent(type);
        if (comp == null)
        {
            comp = go.AddComponent(type);
        }

        return comp;
    }

    public static T AddMonoBehaviour<T>(Component comp) where T : Component
    {

        var ret = comp.GetComponent<T>();
        if (ret == null)
        {
            ret = comp.gameObject.AddComponent<T>();
        }

        return ret;
    }

    public static T AddMonoBehaviour<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }

        return comp;
    }

    [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
    public static void RmvMonoBehaviour(Type type, GameObject go)
    {
        var comp = go.GetComponent(type);
        if (comp != null)
        {
            UnityEngine.Object.Destroy(comp);
        }
    }

    public static void RmvMonoBehaviour<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp != null)
        {
            UnityEngine.Object.Destroy(comp);
        }
    }


    public static Transform FindChild(Transform transform, string path)
    {
        var findTrans = transform.Find(path);
        if (findTrans != null)
        {
            return findTrans;
        }

        return null;
    }

    public static Transform FindChildByName(Transform transform, string name)
    {
        if (transform == null)
        {
            return null;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var childTrans = transform.GetChild(i);
            if (childTrans.name == name)
            {
                return childTrans;
            }

            var find = FindChildByName(childTrans, name);
            if (find != null)
            {
                return find;
            }
        }

        return null;
    }

    [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
    public static Component FindChildComponent(Type type, Transform transform, string path)
    {
        var findTrans = transform.Find(path);
        if (findTrans != null)
        {
            return findTrans.gameObject.GetComponent(type);
        }

        return null;
    }

    public static T FindChildComponent<T>(Transform transform, string path) where T : Component
    {
        var findTrans = transform.Find(path);
        if (findTrans != null)
        {
            return findTrans.gameObject.GetComponent<T>();
        }

        return null;
    }

    public static void SetLayer(GameObject go, int layer)
    {
        if (go == null)
        {
            return;
        }
        SetLayer(go.transform, layer);
    }

    public static void SetLayer(Transform trans, int layer)
    {
        if (trans == null)
        {
            return;
        }
        trans.gameObject.layer = layer;
        for (int i = 0, imax = trans.childCount; i < imax; ++i)
        {
            Transform child = trans.GetChild(i);
            SetLayer(child, layer);
        }
    }

    public static int RandomRangeInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float RandomRangeFloat(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static Vector2 RandomInsideCircle(float radius)
    {
        return UnityEngine.Random.insideUnitCircle * radius;
    }

    [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
    public static Array CreateUnityArray(Type type, int length)
    {
        return Array.CreateInstance(type, length);
    }

    public static T[] CreateUnityArray<T>(int length)
    {
        return new T[length];
    }


    public static GameObject Instantiate(GameObject go)
    {
        if (go != null)
        {
            return UnityEngine.GameObject.Instantiate(go);
        }

        return null;
    }

    public static int GetHashCodeByString(string str)
    {
        return str.GetHashCode();
    }

    public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask)
    {
        return Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
    }

    public static List<string> GetRegexMatchGroups(string pattern, string input)
    {
        List<string> list = new List<string>();
        var regexLink = new Regex(pattern);
        var links = regexLink.Match(input);
        for (var i = 0; i < links.Groups.Count; ++i)
        {
            list.Add(links.Groups[i].Value);
        }
        return list;
    }

    public static void SetMaterialVector3(Material mat, int nameId, Vector3 val)
    {
        mat.SetVector(nameId, val);
    }

    public static void GetVectorData(Vector3 val, out float x, out float y, out float z)
    {
        x = val.x;
        y = val.y;
        z = val.z;
    }
    public static void GetVector2Data(Vector2 val, out float x, out float y)
    {
        x = val.x;
        y = val.y;
    }

    public static bool GetTouchByFingerId(int fingerId, out Touch findTouch)
    {
        var finded = false;
        var touchCnt = Input.touchCount;

        findTouch = new Touch();
        for (int i = 0; i < touchCnt; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.fingerId == fingerId)
            {
                findTouch = touch;
                finded = true;
                break;
            }
        }

        return finded;
    }

    public static bool SetAnimatorController(GameObject go, string resPath)
    {
        RuntimeAnimatorController rac = (RuntimeAnimatorController)ResourcesManager.Instance.Load<UnityEngine.Object>(resPath);
        if (rac == null)
        {
            TDebug.Log("GetAnimator failed path: "+ resPath);
            return false;
        }

        var ani = go.GetComponentInChildren<Animator>(true);

        if (ani != null)
        {
            ani.runtimeAnimatorController = rac;
            return true;
        }

        return false;
    }

    public static void SetGameObjectActive(GameObject go, bool active)
    {
        if (go != null && go.activeSelf != active)
        {
            go.SetActive(active);
        }
    }

    public static T[] GetComponentsInChildren<T>(GameObject go) where T : UIBehaviour
    {
        if (go != null)
        {
           return go.GetComponentsInChildren<T>();
        }

        return null;
    }
}
