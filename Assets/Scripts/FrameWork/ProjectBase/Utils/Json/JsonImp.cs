using System;
using System.Collections.Generic;

class JsonImp : IJson
{
    public DJsonData Deserialize(string json)
    {
        var obj = MiniJSON.Json.Deserialize(json);
        DJsonData jsonData = GetObjectJsonData(obj) as DJsonData;
        return jsonData;
    }

    public string Serialize(DJsonData jsonData)
    {
        var jsonObjData = jsonData.GetJsonDataObject();
        if (jsonObjData == null)
        {
            return null;
        }

        return MiniJSON.Json.Serialize(jsonObjData);
    }

    private object GetObjectJsonData(object obj)
    {
        if (obj is List<object>)
        {
            DJsonData jsonData = new DJsonData();
            List<object> list = (List<object>)obj;
            for (int i = 0; i < list.Count; i++)
            {
                AddArrayData(jsonData, GetObjectJsonData(list[i]));
            }
            return jsonData;
        }
        if (obj is Dictionary<string, object>)
        {
            DJsonData jsonData = new DJsonData();
            Dictionary<string, object> dic = (Dictionary<string, object>)obj;
            var iter = dic.GetEnumerator();
            while (iter.MoveNext())
            {
                AddDictData(jsonData, iter.Current.Key, GetObjectJsonData(iter.Current.Value));
            }
            return jsonData;
        }
        return obj;
    }

    private void AddArrayData(DJsonData jsonData, object data)
    {
        if (data == null)
        {
            return;
        }
        if (data is DJsonData)
        {
            jsonData.AddArrayData((DJsonData)data);
        }
        else if (data is string)
        {
            jsonData.AddArrayData((string)data);
        }
        else if (data is int)
        {
            jsonData.AddArrayData((int)data);
        }
        else if (data is uint)
        {
            jsonData.AddArrayData((uint)data);
        }
        else if (data is Int64)
        {
            jsonData.AddArrayData((Int64)data);
        }
        else if (data is float)
        {
            jsonData.AddArrayData((float)data);
        }
    }

    private void AddDictData(DJsonData jsonData, string key, object data)
    {
        if (data == null)
        {
            return;
        }
        if (data is DJsonData)
        {
            jsonData.AddDictData(key, (DJsonData)data);
        }
        else if (data is string)
        {
            jsonData.AddDictData(key, (string)data);
        }
        else if (data is int)
        {
            jsonData.AddDictData(key, (int)data);
        }
        else if (data is uint)
        {
            jsonData.AddDictData(key, (uint)data);
        }
        else if (data is Int64)
        {
            jsonData.AddDictData(key, (Int64)data);
        }
        else if (data is double)
        {
            jsonData.AddDictData(key, (double)data);
        }
    }

}
