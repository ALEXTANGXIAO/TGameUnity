using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class UISpriteHelper : Singleton<UISpriteHelper>
{
    private const string ConfigPath = "Config/SpriteConfig/SpriteCfg";

    private const string SpriteInfoPath = "Config/SpriteConfig/";

    private Dictionary<string, string> m_spritePathMap = new Dictionary<string, string>();                      //iconName->saveInfoPath

    private Dictionary<string, SpriteSaveInfo> m_spriteSaveInfoDic = new Dictionary<string, SpriteSaveInfo>();   //saveInfoPath->saveInfo

    public void ReadConfig()
    {
        string data = ResourcesManager.Instance.Load<TextAsset>(ConfigPath).text;

        //if (!string.IsNullOrEmpty(data))
        //{
        //    var jsonData = JsonMapper.Deserialize(data);

        //    if (jsonData != null)
        //    {
        //        for (int i = 0; i < jsonData.Count; i++)
        //        {
        //            var strContent = jsonData[i].ToString();

        //            var strArr = strContent.Split(':');

        //            if (strArr.Length == 2)
        //            {
        //                var atlasName = strArr[0];

        //                var spriteName = strArr[1];

        //                m_spritePathMap[spriteName] = atlasName;
        //            }
        //        }
        //    }
        //}

        if (!string.IsNullOrEmpty(data))
        {
            var jsonData = TJson.Deserialize(data);

            if (jsonData != null)
            {
                for (int i = 0; i < jsonData.ArrayCount; i++)
                {

                    var strContent = jsonData.GetStringDataByIndex(i);

                    var strArr = strContent.Split(':');

                    if (strArr.Length == 2)
                    {
                        var atlasName = strArr[0];

                        var spriteName = strArr[1];

                        m_spritePathMap[spriteName] = atlasName;
                    }
                }
            }
        }
    }

    private SpriteSaveInfo LoadSaveInfo(string name)
    {
        SpriteSaveInfo saveInfo = null;
        if (!m_spriteSaveInfoDic.TryGetValue(name, out saveInfo))
        {
            var path = string.Format("{0}{1}", SpriteInfoPath, name);
            var cfgPrefab = ResourcesManager.Instance.Load<GameObject>(path);
            if (cfgPrefab != null)
            {
                saveInfo = cfgPrefab.GetComponent<SpriteSaveInfo>();
                if (saveInfo != null)
                {
                    saveInfo.CacheSprites();
                    m_spriteSaveInfoDic.Add(name, saveInfo);
                }
                else
                {
                    TDebug.Log("load spriteSaveInfo fatal.");
                }
            }
        }

        return saveInfo;
    }

    public void UnloadAllAtlas()
    {
        var enumerator = m_spriteSaveInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            //Destroy(enumerator.Current.Value);
        }

        m_spriteSaveInfoDic.Clear();
    }

    public void UnloadSaveInfo(string name)
    {
        SpriteSaveInfo saveInfo = null;
        if (m_spriteSaveInfoDic.TryGetValue(name, out saveInfo))
        {
            //Destroy(saveInfo.gameObject);
            m_spriteSaveInfoDic.Remove(name);
        }
    }

    /// <summary>
    ///  设置图标 
    /// </summary>
    /// <param name="image"></param>
    /// <param name="spriteName"></param>
    public void SetSprite(Image image, string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName) || image == null)
        {
            return;
        }

        string path;
        if (m_spritePathMap.TryGetValue(spriteName, out path))
        {
            var saveInfo = LoadSaveInfo(path);
            if (saveInfo != null)
            {
                Sprite sprite = saveInfo.GetSprite(spriteName);
                image.sprite = sprite;
            }
        }
    }
}
