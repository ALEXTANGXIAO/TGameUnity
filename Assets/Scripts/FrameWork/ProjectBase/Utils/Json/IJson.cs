using System;
using System.Collections.Generic;

public class DJsonData
{
    private List<object> m_listData;
    private Dictionary<string, object> m_dictData;

    public DJsonData()
    {
    }

    public DJsonData(object data)
    {
        var dictData = data as Dictionary<string, object>;
        if (dictData != null)
        {
            SetDictData(dictData);
            return;
        }

        var listData = data as List<object>;
        if (listData != null)
        {
            SetListData(listData);
        }
    }

    public object GetJsonDataObject()
    {
        return GetJsonDataObject(this);
    }

    private object GetJsonDataObject(object data)
    {
        if (data is DJsonData)
        {
            var jsonData = (DJsonData) data;
            if (jsonData.m_listData != null)
            {
                List<object> result = new List<object>();
                for (int i = 0; i < jsonData.m_listData.Count; i++)
                {
                    var childData = jsonData.m_listData[i];
                    result.Add(GetJsonDataObject(childData));
                }

                return result;
            }

            if (jsonData.m_dictData != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                var iter = jsonData.m_dictData.GetEnumerator();
                while (iter.MoveNext())
                {
                    var childData = iter.Current.Value;
                    result.Add(iter.Current.Key, GetJsonDataObject(childData));
                }

                return result;
            }

            return null;
        }

        return data;
    }

    private void SetDictData(Dictionary<string, object> dataSet)
    {
        m_dictData = new Dictionary<string, object>();
        var itr = dataSet.GetEnumerator();
        while (itr.MoveNext())
        {
            var kv = itr.Current;
            if (IsComplexData(kv.Value))
            {
                m_dictData[kv.Key] = new DJsonData(kv.Value);
            }
            else
            {
                m_dictData[kv.Key] = kv.Value;
            }
        }

        itr.Dispose();
    }

    private void SetListData(List<object> dataSet)
    {
        m_listData = new List<object>();

        for (int i = 0; i < dataSet.Count; i++)
        {
            var jsonObj = dataSet[i];
            if (IsComplexData(jsonObj))
            {
                m_listData.Add(new DJsonData(jsonObj));
            }
            else
            {
                m_listData.Add(jsonObj);
            }
        }
    }

    private bool IsComplexData(object data)
    {
        return data is Dictionary<string, object> ||
               data is List<object>;
    }

    public int ArrayCount
    {
        get { return m_listData != null ? m_listData.Count : 0; }
    }

    public bool IsArrayType
    {
        get { return m_listData != null; }
    }

    public bool IsTableType
    {
        get { return m_dictData != null; }
    }

    public string[] GetDictKeyList()
    {
        if (m_dictData.Count <= 0)
        {
            return null;
        }

        var list = new string[m_dictData.Count];
        var itr = m_dictData.GetEnumerator();

        int index = 0;
        while (itr.MoveNext())
        {
            var kv = itr.Current;
            list[index++] = kv.Key;
        }

        return list;
    }

    private T GetDataByKey<T>(string key)
    {
        if (m_dictData != null)
        {
            object val;
            if (m_dictData.TryGetValue(key, out val))
            {
                if (val != null)
                {
                    try
                    {
                        if (typeof(T) == typeof(float))
                        {
                            val = Convert.ToSingle(val);
                        }

                        if (typeof(T) == typeof(double))
                        {
                            val = Convert.ToDouble(val);
                        }

                        return (T) val;
                    }
                    catch (Exception e)
                    {
                        TDebug.Log("Key:{0} cant caster type: {1} to type:{2}", key, val.GetType().Name,
                            typeof(T).Name);
                        throw;
                    }
                }
            }
        }

        return default(T);
    }

    public DJsonData GetJsonDataByKey(string key)
    {
        return GetDataByKey<DJsonData>(key);
    }

    public string GetStringDataByKey(string key)
    {
        return GetDataByKey<string>(key);
    }

    public int GetIntDataByKey(string key)
    {
        return (int) GetInt64DataByKey(key);
    }

    public float GetFloatDataByKey(string key)
    {
        return GetDataByKey<float>(key);
    }

    public Int64 GetInt64DataByKey(string key)
    {
        return GetDataByKey<Int64>(key);
    }

    public bool ContainsKey(string key)
    {
        return m_dictData != null && m_dictData.ContainsKey(key);
    }

    private T GetDataByIndex<T>(int index)
    {
        if (m_listData != null)
        {
            if (index >= 0 && index < m_listData.Count)
            {
                return (T) m_listData[index];
            }
        }

        return default(T);
    }

    public DJsonData GetJsonDataByIndex(int index)
    {
        return GetDataByIndex<DJsonData>(index);
    }

    public string GetStringDataByIndex(int index)
    {
        return GetDataByIndex<string>(index);
    }

    public int GetIntDataByIndex(int index)
    {
        return (int) GetInt64DataByIndex(index);
    }

    public float GetFloatDataByIndex(int index)
    {
        return GetDataByIndex<float>(index);
    }

    public Int64 GetInt64DataByIndex(int index)
    {
        return GetDataByIndex<Int64>(index);
    }


    private void AddArrayTempData<T>(T val)
    {
        if (m_listData == null)
        {
            m_listData = new List<object>();
        }

        m_listData.Add(val);
    }

    public void AddArrayData(DJsonData jsonData)
    {
        AddArrayTempData(jsonData);
    }

    public void AddArrayData(string data)
    {
        AddArrayTempData(data);
    }

    public void AddArrayData(int data)
    {
        AddArrayTempData(data);
    }

    public void AddArrayData(uint data)
    {
        AddArrayTempData((Int64) data);
    }

    public void AddArrayData(Int64 data)
    {
        AddArrayTempData(data);
    }

    public void AddArrayData(float data)
    {
        AddArrayTempData(data);
    }

    private void AddDictTempData<T>(string key, T val)
    {
        if (m_dictData == null)
        {
            m_dictData = new Dictionary<string, object>();
        }

        m_dictData[key] = val;
    }

    public void AddDictData(string key, DJsonData data)
    {
        AddDictTempData(key, data);
    }

    public void AddDictData(string key, string data)
    {
        AddDictTempData(key, data);
    }

    public void AddDictData(string key, int data)
    {
        AddDictTempData(key, data);
    }

    public void AddDictData(string key, uint data)
    {
        AddDictTempData(key, (Int64) data);
    }

    public void AddDictData(string key, Int64 data)
    {
        AddDictTempData(key, data);
    }

    public void AddDictData(string key, double data)
    {
        AddDictTempData(key, data);
    }
}

public interface IJson
{
    /// <summary>
    /// 序列化JSON
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    DJsonData Deserialize(string json);

    /// <summary>
    /// 反序列化Json
    /// </summary>
    /// <param name="jsonData"></param>
    /// <returns></returns>
    string Serialize(DJsonData jsonData);
}
