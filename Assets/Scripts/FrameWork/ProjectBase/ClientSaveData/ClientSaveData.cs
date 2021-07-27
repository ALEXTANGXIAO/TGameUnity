
using System.Collections.Generic;
using UnityEngine;

abstract class BaseClientData
{
    private string m_configName;

    public void Init(string configName)
    {
        m_configName = configName;
        Load();
    }

    public void Load()
    {
        string fullName = GetSaveUniqPrefix() + m_configName;
        //PlayerPrefs.DeleteKey(fullName);
        var jsonString = PlayerPrefs.GetString(fullName);
        if (!string.IsNullOrEmpty(jsonString))
        {
            DJsonData json = (DJsonData)TJson.Deserialize(jsonString);

            if (json != null)
            {
                Deserialize(json);
            }
        }
    }

    public void Save()
    {
        string fullName = GetSaveUniqPrefix() + m_configName;
        DJsonData jsonData = new DJsonData();
        Serialize(jsonData);

        var jsonTex = TJson.Serialize(jsonData);
        if (!string.IsNullOrEmpty(jsonTex))
        {
            PlayerPrefs.SetString(fullName, jsonTex);
            PlayerPrefs.Save();
        }
    }
    /**
     * 序列化为json字符串
     */
    protected abstract void Serialize(DJsonData json);

    /**
     * 解码字符串
     */
    protected abstract void Deserialize(DJsonData json);

    private string GetSaveUniqPrefix()
    {
        string hashPath = UnityUtil.GetHashCodeByString(Application.dataPath).ToString();
        string uniqInstance = SystemInfo.deviceUniqueIdentifier;
        string uniqKey = hashPath + uniqInstance;
        return uniqKey;
    }
}

class ClientSaveData : Singleton<ClientSaveData>
{
    private Dictionary<string, BaseClientData> m_dictSaveData = new Dictionary<string, BaseClientData>();

    public T GetSaveData<T>() where T : BaseClientData, new()
    {
        string typeName = typeof(T).Name;
        BaseClientData ret;
        if (!m_dictSaveData.TryGetValue(typeName, out ret))
        {
            ret = new T();
            ret.Init(typeName);
            m_dictSaveData.Add(typeName, ret);
        }
        return (T)ret;
    }

    public void SaveAllData()
    {
        var enumerator = m_dictSaveData.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.Value.Save();
        }
    }
}

//var saveData = ClientSaveData.Instance.GetSaveData<TestSaveData>();
//saveData.m_uuid = "123";
//saveData.m_pwd = "223";
//saveData.Save();

class TestSaveData :BaseClientData
{
    public string m_uuid;
    public string m_pwd;

    protected override void Serialize(DJsonData json)
    {
        json.AddDictData("uuid", m_uuid);
        json.AddDictData("pwd", m_pwd);
    }

    protected override void Deserialize(DJsonData json)
    {
        m_uuid = json.GetStringDataByKey("uuid");
        m_pwd = json.GetStringDataByKey("pwd");
    }
}

class SettingSaveData : BaseClientData
{
    public int m_saved;
    public float m_bgvalue;
    public float m_soundvalue;

    protected override void Serialize(DJsonData json)
    {
        json.AddDictData("saved", m_saved);
        json.AddDictData("bgvalue", m_bgvalue);
        json.AddDictData("soundvalue", m_soundvalue);
    }

    protected override void Deserialize(DJsonData json)
    {
        m_saved = json.GetIntDataByKey("saved");
        m_bgvalue = json.GetFloatDataByKey("bgvalue");
        m_soundvalue = json.GetFloatDataByKey("soundvalue");
    }
}
