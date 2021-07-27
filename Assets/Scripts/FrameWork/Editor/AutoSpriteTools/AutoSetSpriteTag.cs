using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

/// <summary>
/// UI/RAW 目录资源自动设置成sprite填充默认packing tag
/// </summary>
public class AutoSetSpriteTag : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (var s in importedAssets)
        {
            EditorSpriteSaveInfo.OnImportSprite(s);
        }

        foreach (var s in deletedAssets)
        {
            EditorSpriteSaveInfo.OnDeleteSprite(s);
        }

        foreach (var s in movedFromAssetPaths)
        {
            EditorSpriteSaveInfo.OnDeleteSprite(s);
        }

        foreach (var s in movedAssets)
        {
            EditorSpriteSaveInfo.OnImportSprite(s);
        }

        //EditorSpriteSaveInfo.CheckDirty();
    }
}

public static class EditorSpriteSaveInfo
{
    private static bool m_inited = false;

    private static Dictionary<string, List<string>> m_allASprites = new Dictionary<string, List<string>>();
    private static Dictionary<string, string> m_uiAtlasMap = new Dictionary<string, string>();
    private static bool m_dirty = false;
    private static List<string> m_dirtyAtlasList = new List<string>();

    private const string NormalAtlasDir = "Assets/Atlas";

    private const string SpriteConfigPath = "Assets/Resources/Config/SpriteConfig/SpriteCfg.bytes";
    private const string SpriteConfigDir = "Assets/Resources/Config/SpriteConfig/";

    public static void Init()
    {
        if (m_inited)
        {
            return;
        }
        EditorApplication.update += CheckDirty;

        //读取所有图集信息
        string[] findAssets = AssetDatabase.FindAssets("t:spriteatlas", new[] { NormalAtlasDir });
        foreach (var findAsset in findAssets)
        {
            var path = AssetDatabase.GUIDToAssetPath(findAsset);
            SpriteAtlas sa = AssetDatabase.LoadAssetAtPath(path, typeof(SpriteAtlas)) as SpriteAtlas;
            if (sa == null)
            {
                Debug.LogError(string.Format("加载图集数据{0}失败", path));
                continue;
            }

            string atlasName = Path.GetFileNameWithoutExtension(path);
            var objects = sa.GetPackables();
            foreach (var o in objects)
            {
                List<string> list;
                if (!m_allASprites.TryGetValue(atlasName, out list))
                {
                    list = new List<string>();
                    m_allASprites.Add(atlasName, list);
                }

                list.Add(AssetDatabase.GetAssetPath(o));
            }
        }

        //读取spriteCfg
        FileInfo fileInfo = new FileInfo(SpriteConfigPath);
        StreamReader sr = fileInfo.OpenText();
        string str = sr.ReadToEnd();
        sr.Close();

        var data = JsonConvert.DeserializeObject<List<string>>(str);
        if (data != null)
        {
            foreach (var kv in data)
            {
                var list = kv.Split(':');
                var atlasName = list[0];
                var spriteName = list[1];
                string oldAtlasName;
                if (!m_uiAtlasMap.TryGetValue(spriteName, out oldAtlasName))
                {
                    m_uiAtlasMap[spriteName] = atlasName;
                }
                else if (oldAtlasName != atlasName)
                {
                    Debug.LogError(string.Format("有重名的图片：{0}\n旧图集：{1}\n新图集：{2} ", spriteName, oldAtlasName, atlasName));
                }
            }
        }

        m_inited = true;
    }

    public static void OnProcessSprite(string assetPath)
    {
        if (!assetPath.StartsWith("Assets"))
        {
            return;
        }

        Init();

        string atlasName = TextureHelper.GetPackageTag(assetPath);
        if (string.IsNullOrEmpty(atlasName))
        {
            return;
        }

        if (assetPath.StartsWith("Assets/UIRaw"))
        {
            var spriteName = Path.GetFileNameWithoutExtension(assetPath);
            string oldAtlasName;
            if (!m_uiAtlasMap.TryGetValue(spriteName, out oldAtlasName))
            {
                m_uiAtlasMap.Add(spriteName, atlasName);
                m_dirty = true;
            }
            else if (oldAtlasName != atlasName)
            {
                Debug.LogError(string.Format("有重名的图片：{0}\n旧图集：{1}\n新图集：{2} ", spriteName, oldAtlasName, atlasName));
                m_uiAtlasMap[spriteName] = atlasName;
                m_dirty = true;
            }
        }

        List<string> ret;
        if (!m_allASprites.TryGetValue(atlasName, out ret))
        {
            ret = new List<string>();
            m_allASprites.Add(atlasName, ret);
        }

        if (!ret.Contains(assetPath))
        {
            ret.Add(assetPath);
            m_dirty = true;
            if (!m_dirtyAtlasList.Contains(atlasName))
            {
                m_dirtyAtlasList.Add(atlasName);
            }
        }
    }

    public static void OnImportSprite(string s)
    {
        //先只对 uiraw 目录处理
        if (!s.StartsWith("Assets/UIRaw"))
        {
            return;
        }

        TextureImporter ti = AssetImporter.GetAtPath(s) as TextureImporter;
        if (ti != null)
        {
            var modify = false;

            if (s.StartsWith("Assets/UIRaw"))
            {
                if (ti.textureType != TextureImporterType.Sprite)
                {
                    ti.textureType = TextureImporterType.Sprite;
                    modify = true;
                }

                if (string.IsNullOrEmpty(ti.spritePackingTag))
                {
                    ti.spritePackingTag = TextureHelper.GetPackageTag(s);
                    modify = true;
                }

                var setting = new TextureImporterSettings();
                ti.ReadTextureSettings(setting);
                if (setting.spriteGenerateFallbackPhysicsShape)
                {
                    setting.spriteGenerateFallbackPhysicsShape = false;
                    ti.SetTextureSettings(setting);
                    modify = true;
                }
            }

            if (ti.textureType == TextureImporterType.Sprite)
            {
                //调整 ios 压缩方式
                //var platformSettings = ti.GetPlatformTextureSettings("iPhone");
                //if (!platformSettings.overridden)
                //{
                //    platformSettings.overridden = true;
                //    platformSettings.format = TextureImporterFormat.ASTC_RGBA_5x5;
                //    platformSettings.compressionQuality = 100;
                //    ti.SetPlatformTextureSettings(platformSettings);
                //    modify = true;
                //}
            }

            if (modify)
            {
                ti.SaveAndReimport();
            }

            if (ti.textureType == TextureImporterType.Sprite)
            {
                EditorSpriteSaveInfo.OnProcessSprite(s);
            }
        }
    }

    public static void OnDeleteSprite(string assetPath)
    {
        Init();
        string atlasName = TextureHelper.GetPackageTag(assetPath);


        List<string> ret;
        if (!m_allASprites.TryGetValue(atlasName, out ret))
        {
            return;
        }

        //改成文件名的匹配
        if (!ret.Exists(s => Path.GetFileName(s) == Path.GetFileName(assetPath)))
        {
            return;
        }

        if (assetPath.StartsWith("Assets/UIRaw"))
        {
            var spriteName = Path.GetFileNameWithoutExtension(assetPath);
            if (m_uiAtlasMap.ContainsKey(spriteName))
            {
                m_uiAtlasMap.Remove(spriteName);
                //SaveUISpriteCfg();
                m_dirty = true;
            }
        }

        ret.Remove(assetPath);
        m_dirty = true;
        if (!m_dirtyAtlasList.Contains(atlasName))
        {
            m_dirtyAtlasList.Add(atlasName);
        }
    }

    public static void CheckDirty()
    {
        if (m_dirty)
        {
            m_dirty = false;

            AssetDatabase.Refresh();
            float lastProgress = -1;
            SaveUISpriteCfg();
            for (int i = 0; i < m_dirtyAtlasList.Count; i++)
            {
                string atlasName = m_dirtyAtlasList[i];
                Debug.Log("更新图集 : " + atlasName);
                var curProgress = (float)i / m_dirtyAtlasList.Count;
                if (curProgress > lastProgress + 0.01f)
                {
                    lastProgress = curProgress;
                    var progressText = string.Format("当前进度：{0}/{1} {2}", i, m_dirtyAtlasList.Count, atlasName);
                    bool cancel = EditorUtility.DisplayCancelableProgressBar("刷新图集" + atlasName, progressText, curProgress);
                    if (cancel)
                    {
                        break;
                    }
                }
                bool isUI = atlasName.StartsWith("UIRaw");
                SaveAtlas(atlasName, isUI);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //             SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);

            m_dirtyAtlasList.Clear();
        }
    }

    private static void SaveUISpriteCfg()
    {
        List<string> list = new List<string>();
        foreach (var pair in m_uiAtlasMap)
        {
            list.Add(pair.Value + ":" + pair.Key);
        }
        list.Sort();

        var json = JsonConvert.SerializeObject(list, Formatting.Indented);

        var fileInfo = new FileInfo(SpriteConfigPath);
        var streamWriter = fileInfo.CreateText();
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }

    public static void ClearAtlas(string atlasName)
    {
        if (!m_allASprites.ContainsKey(atlasName))
        {
            return;
        }
        m_allASprites.Remove(atlasName);
    }

    public static void SaveAtlas(string atlasName, bool isUI)
    {
        if (!m_allASprites.ContainsKey(atlasName))
        {
            return;
        }
        var list = m_allASprites[atlasName];
        list.Sort(StringComparer.Ordinal);

        List<Object> spriteList = new List<Object>();
        foreach (var s in list)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(s);
            if (sprite != null)
            {
                spriteList.Add(sprite);
            }
        }
        var path = string.Format("{0}/{1}.spriteatlas", NormalAtlasDir, atlasName);

        if (spriteList.Count == 0)
        {
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }

            string cfgPath = SpriteConfigDir + atlasName + ".prefab";
            if (File.Exists(cfgPath))
            {
                AssetDatabase.DeleteAsset(cfgPath);
            }
            return;
        }

        var atlas = new SpriteAtlas();
        var setting = new SpriteAtlasPackingSettings();
        setting.blockOffset = 1;
        setting.padding = 2;
        setting.enableRotation = true;

        bool isOpaque = atlasName.Contains("Opaque");

        var textureSetting = new SpriteAtlasTextureSettings();
        textureSetting.generateMipMaps = false;
        textureSetting.sRGB = true;
        textureSetting.filterMode = FilterMode.Bilinear;
        atlas.SetTextureSettings(textureSetting);

        var iphonePlatformSetting = atlas.GetPlatformSettings("iPhone");
        if (!iphonePlatformSetting.overridden)
        {
            iphonePlatformSetting.overridden = true;
            iphonePlatformSetting.format = isOpaque ? TextureImporterFormat.ASTC_RGB_5x5 : TextureImporterFormat.ASTC_RGBA_5x5;
            iphonePlatformSetting.compressionQuality = 100;
            atlas.SetPlatformSettings(iphonePlatformSetting);
        }

        var androidPlatformSetting = atlas.GetPlatformSettings("Android");
        if (isOpaque && !androidPlatformSetting.overridden)
        {
            androidPlatformSetting.overridden = true;
            androidPlatformSetting.format = TextureImporterFormat.ETC_RGB4;
            androidPlatformSetting.compressionQuality = 100;
            atlas.SetPlatformSettings(androidPlatformSetting);
        }

        atlas.SetPackingSettings(setting);
        atlas.Add(spriteList.ToArray());

        AssetDatabase.CreateAsset(atlas, path);

        if (isUI)
        {
            ProcessSpriteSaveInfo(spriteList, atlasName);
        }
    }

    private static void ProcessSpriteSaveInfo(List<Object> spriteList, string atlasName)
    {
        string cfgPath = SpriteConfigDir + atlasName + ".prefab";
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(cfgPath);
        SpriteSaveInfo saveInfo;
        bool isNew = false;
        if (go == null)
        {
            go = new GameObject();
            saveInfo = go.AddComponent<SpriteSaveInfo>();
            isNew = true;
        }
        else
        {
            saveInfo = go.GetComponent<SpriteSaveInfo>();
        }

        saveInfo.Sprites.Clear();
        for (int i = 0; i < spriteList.Count; i++)
        {
            saveInfo.Sprites.Add((Sprite)spriteList[i]);
        }

        if (isNew)
        {
            bool success;
            PrefabUtility.SaveAsPrefabAsset(go, cfgPath, out success);
            if (!success)
            {
                Debug.LogError(string.Format("save prefab to {0} failed", cfgPath));
            }
            else
            {
                Debug.Log(string.Format("save prefab [{0}] success", atlasName));
            }

            GameObject.DestroyImmediate(go);
        }
        else
        {
            //EditorUtility.SetDirty(go);
            PrefabUtility.SavePrefabAsset(go);
        }
    }
}