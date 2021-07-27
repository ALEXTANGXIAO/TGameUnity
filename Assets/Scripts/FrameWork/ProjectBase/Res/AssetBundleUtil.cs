using MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
public delegate void RecordLoadFileFunc(string path);

public class AssetRootMetaInfo
{
    public string m_version;
    public string m_platform;
    public bool m_isLua;
}
public class AssetMd5FileInfo
{
    public bool m_compress = false;
    public bool m_downLoad = false;
    public uint m_crc = 0;
    public string m_md5FilePath;
    public string m_md5Content;
    public int m_contentSize;
    public string m_md5ExContent;
}

public class GameCoreConfig
{
    public static bool UseAssetBundle = false;
    public static bool RunLuaMode = false;
    public static int ResourceMaxAsyncLoadNum = 5;
    public static bool EnableResourceAsyncLimit = true;
    public static bool DisableWarmupShader = false;
    public static bool InitWarmupShader = true;
    public static bool UseSmallPackage = false;
}

public static class AssetBundleUtil
{
    public static bool m_abHashMd5 = true;
    public static long m_readAbTotalSize = 0;
    private static string m_fileProtocol = "file://";
    public static bool traceAssetBundleDebug = false;
    private static uint[] s_encKey = (uint[])null;
    public static RecordLoadFileFunc RecordLoadFileFunc = (RecordLoadFileFunc)null;
    public static CultureInfo _info = new CultureInfo("en-GB");
    public const string MD5HASH_FILE_ROOT_NAME = "root.bytes";
    public const string MD5HASH_FILE_INDEX_NAME = "index.bytes";
    public const string ASSET_TIMESTMP = "asset_time.bytes";
    public const string MD5_MAP_FILE = "md5map.bytes";
    public const uint MD5HASH_FILE_INDEX_COUNT = 10;
    private static string m_externAssetBundleDir;

    public static string Md5Sum(string strToEncrypt)
    {
        return AssetBundleUtil.Md5Sum(new UTF8Encoding().GetBytes(strToEncrypt));
    }

    public static string Md5Sum(byte[] bytes)
    {
        byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
        string str = "";
        for (int index = 0; index < hash.Length; ++index)
            str += Convert.ToString(hash[index], 16).PadLeft(2, '0');
        return str.PadLeft(32, '0');
    }

    public static string MapMd5ListFilePath(string assetMd5Path)
    {
        return string.Format("index_{0}.bytes", (object)((uint)assetMd5Path.GetHashCode() % 10U));
    }

    public static string NormalScenePath(string sceneName)
    {
        return "Assets/Scenes/" + sceneName + ".unity";
    }

    public static string GetPathHash(string path)
    {
        if (!path.Contains("/") && !path.Contains("\\"))
            return path;
        if (AssetBundleUtil.m_abHashMd5)
            return AssetBundleUtil.Md5Sum(path);
        return path.Replace("/", "_").Replace("\\", "_").ToLower(AssetBundleUtil._info);
    }

    public static bool IsSceneFile(string strPath)
    {
        return strPath.EndsWith(".unity", true, (CultureInfo)null);
    }

    public static bool IsMatFile(string path)
    {
        string extension = Path.GetExtension(path);
        return !string.IsNullOrEmpty(extension) && extension.Equals(".mat", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsSceneRelateAbPath(string abPath)
    {
        return AssetBundleUtil.IsSceneFile(abPath) || abPath.EndsWith(".unity_vir");
    }

    public static bool IsFbxFile(string strPath)
    {
        return strPath.EndsWith(".fbx", true, (CultureInfo)null);
    }

    public static bool IsScriptFile(string strPath)
    {
        return strPath.EndsWith(".cs", true, (CultureInfo)null) || strPath.EndsWith(".js", true, (CultureInfo)null);
    }

    public static bool IsShaderFile(string strPath)
    {
        return strPath.EndsWith(".shader", true, (CultureInfo)null) || strPath.EndsWith(".cginc", true, (CultureInfo)null);
    }

    public static bool IsResourceFile(string strPath)
    {
        return strPath.IndexOf("Assets/Resources/") == 0;
    }

    public static string NormalPathSepChar(string strPath)
    {
        if (strPath.Contains("\\"))
            return strPath.Replace('\\', '/');
        return strPath;
    }

    public static string GetResourcePathFromFullPath(string strPath)
    {
        strPath = AssetBundleUtil.NormalPathSepChar(strPath);
        if (!AssetBundleUtil.IsResourceFile(strPath))
        {
            Debug.Log((object)("resource path invalid: " + strPath));
            return strPath;
        }
        string str1 = "Assets/Resources/";
        string str2 = strPath.Substring(str1.Length);
        int length = str2.LastIndexOf(".");
        int num = str2.LastIndexOf("/");
        if (length >= 0 && (num < 0 || length > num))
            return str2.Substring(0, length);
        return str2;
    }

    public static string GetExternAssetBundleDir()
    {
        return AssetBundleUtil.m_externAssetBundleDir;
    }

    public static void SetExternAssetBundleDir(string abDir)
    {
        AssetBundleUtil.m_externAssetBundleDir = abDir;
    }

    public static string GetFileProtocol()
    {
        return AssetBundleUtil.m_fileProtocol;
    }

    public static void SetFileProtocol(string protocol)
    {
        AssetBundleUtil.m_fileProtocol = protocol;
    }

    public static bool WriteToFile(string dirOut, string fileName, string content)
    {
        byte[] bytes = new UTF8Encoding().GetBytes(content);
        return AssetBundleUtil.WriteToFile(dirOut, fileName, bytes);
    }

    public static bool WriteToFile(string dirOut, string fileName, byte[] data)
    {
        try
        {
            AssetBundleUtil.CreateDir(dirOut);
            FileStream fileStream = new FileStream(dirOut + "/" + fileName, FileMode.Create);
            fileStream.Write(data, 0, data.Length);
            fileStream.Flush();
            fileStream.Close();
            TDebug.Log("write asset[{0}] ok", (object)fileName);
            return true;
        }
        catch (Exception ex)
        {
            TDebug.Log("write asset[{0}] failed, error[{1}]", (object)fileName, (object)ex.ToString());
            return false;
        }
    }

    public static bool DeleteDir(string dir)
    {
        return XFileUtil.DeleteDir(dir);
    }

    public static bool CreateDir(string dir)
    {
        return XFileUtil.CreateDir(dir);
    }

    public static void DeleteFile(string path)
    {
        XFileUtil.DeleteFile(path);
    }

    public static string GetFileMd5(string path)
    {
        byte[] bytes = AssetBundleUtil.ReadFile(path);
        if (bytes != null)
            return AssetBundleUtil.Md5Sum(bytes);
        TDebug.Log("GetFileMd5 failed, Read file failed: {0}", (object)path);
        return (string)null;
    }

    public static string GetAssetBundleFileName(string md5Path)
    {
        return md5Path + ".ab";
    }

    public static string GetAssetBundlePathFromFileName(string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName);
    }

    public static string GetShaderBundlePath()
    {
        return "xgame_shader";
    }

    public static bool IsFileExist(string path)
    {
        try
        {
            return File.Exists(path);
        }
        catch (Exception ex)
        {
            TDebug.Log("Check exist exception: {0}", (object)ex.ToString());
        }
        return false;
    }

    public static int ReadFileLen(string path)
    {
        try
        {
            if (AssetBundleUtil.IsFileExist(path))
                return (int)new FileInfo(path).Length;
        }
        catch (Exception ex)
        {
            TDebug.Log("ReadFileLen failed:{0}, error:{1}", (object)path, (object)ex.ToString());
        }
        return 0;
    }

    public static byte[] ReadFile(string path)
    {
        string errInfo = (string)null;
        byte[] numArray = AssetBundleUtil.ReadFile(path, ref errInfo);
        if (!string.IsNullOrEmpty(errInfo))
            TDebug.Log("ReadFile failed: {0}", (object)errInfo);
        return numArray;
    }

    public static byte[] ReadFile(string path, ref string errInfo)
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int length = (int)fileStream.Length;
                if (length <= 0)
                {
                    errInfo = "read file failed, empty: " + path + ", expect size: " + length.ToString();
                    return (byte[])null;
                }
                byte[] buffer = new byte[length];
                fileStream.Seek(0L, SeekOrigin.Begin);
                int num = fileStream.Read(buffer, 0, length);
                if (num < length)
                {
                    errInfo = "read file failed: " + path + ", expect size: " + length.ToString() + ", read len: " + num.ToString();
                    return (byte[])null;
                }
                fileStream.Close();
                return buffer;
            }
        }
        catch (Exception ex)
        {
            errInfo = "read file[" + path + "] failed:" + ex.ToString();
        }
        return (byte[])null;
    }

    public static string ReadTextFile(string path)
    {
        string errInfo = (string)null;
        byte[] bytes = AssetBundleUtil.ReadFile(path, ref errInfo);
        if (bytes == null)
            return (string)null;
        return Encoding.UTF8.GetString(bytes);
    }

    public static T ReadJsonKey<T>(Dictionary<string, object> json, string key)
    {
        if (json.ContainsKey(key))
        {
            try
            {
                return (T)json[key];
            }
            catch (Exception ex)
            {
                TDebug.Log("key: {0} convert type {1} to {2} failed, exception:{3}", (object)key, (object)json[key].GetType(), (object)typeof(T).ToString(), (object)ex.ToString());
            }
        }
        return default(T);
    }

    public static List<object> ReadJsonList(string url, string content, ref string error)
    {
        List<object> objectList;
        try
        {
            objectList = Json.Deserialize(content) as List<object>;
            if (objectList == null)
            {
                error = string.Format("parse md5 file failed[{0}] Content[{1}]", (object)url, (object)content);
                TDebug.Log("parse md5 file failed !!!");
                return (List<object>)null;
            }
        }
        catch (Exception ex)
        {
            error = string.Format("parse md5 root file failed[{0}] Content[{1}]", (object)url, (object)content);
            TDebug.Log("parse md5 root file failed[{0}] Content[{1}] Exception[{2}]", (object)url, (object)content, (object)ex.ToString());
            return (List<object>)null;
        }
        return objectList;
    }

    public static AssetRootMetaInfo ParseRootMeta(string content)
    {
        AssetRootMetaInfo assetRootMetaInfo = new AssetRootMetaInfo();
        Dictionary<string, object> json = Json.Deserialize(content) as Dictionary<string, object>;
        if (json != null)
        {
            assetRootMetaInfo.m_version = AssetBundleUtil.ReadJsonKey<string>(json, "ver");
            assetRootMetaInfo.m_platform = AssetBundleUtil.ReadJsonKey<string>(json, "plat");
            long num = AssetBundleUtil.ReadJsonKey<long>(json, "lua");
            assetRootMetaInfo.m_isLua = num == 1L;
        }
        else
            TDebug.Log("parse md5 file failed[{0}] Content[{1}]", (object)"root.byte", (object)content);
        return assetRootMetaInfo;
    }

    public static List<AssetMd5FileInfo> ParseAssetMd5List(
      string url,
      string content,
      ref string error)
    {
        Dictionary<string, object> json1;
        try
        {
            json1 = Json.Deserialize(content) as Dictionary<string, object>;
            if (json1 == null)
            {
                error = string.Format("parse md5 file failed[{0}] Content[{1}]", (object)url, (object)content);
                TDebug.Log("parse md5 file failed !!!");
                return (List<AssetMd5FileInfo>)null;
            }
        }
        catch (Exception ex)
        {
            error = string.Format("parse md5 root file failed[{0}] Content[{1}]", (object)url, (object)content);
            TDebug.Log("parse md5 root file failed[{0}] Content[{1}] Exception[{2}]", (object)url, (object)content, (object)ex.ToString());
            return (List<AssetMd5FileInfo>)null;
        }
        List<AssetMd5FileInfo> assetMd5FileInfoList = new List<AssetMd5FileInfo>();
        List<object> objectList = AssetBundleUtil.ReadJsonKey<List<object>>(json1, "list");
        assetMd5FileInfoList.Capacity = objectList.Count;
        foreach (object obj1 in objectList)
        {
            try
            {
                Dictionary<string, object> json2 = obj1 as Dictionary<string, object>;
                string str1 = AssetBundleUtil.ReadJsonKey<string>(json2, "path");
                string str2 = AssetBundleUtil.ReadJsonKey<string>(json2, "md5");
                string str3 = AssetBundleUtil.ReadJsonKey<string>(json2, "md5_ex");
                object obj2 = AssetBundleUtil.ReadJsonKey<object>(json2, "size");
                uint num1 = (uint)AssetBundleUtil.ReadJsonKey<long>(json2, "crc");
                uint num2 = (uint)AssetBundleUtil.ReadJsonKey<long>(json2, "com");
                int num3 = (int)(long)obj2;
                int num4 = 0;
                if (json2.ContainsKey("down"))
                    num4 = (int)(long)json2["down"];
                assetMd5FileInfoList.Add(new AssetMd5FileInfo()
                {
                    m_md5FilePath = str1,
                    m_md5ExContent = str3,
                    m_md5Content = str2,
                    m_contentSize = num3,
                    m_compress = num2 > 0U,
                    m_downLoad = 1 == num4,
                    m_crc = num1
                });
            }
            catch (Exception ex)
            {
                error = string.Format("parse list error, url[{0}] content[{1}] err[{2}]", (object)url, (object)Json.Serialize(obj1), (object)ex.ToString());
                return (List<AssetMd5FileInfo>)null;
            }
        }
        return assetMd5FileInfoList;
    }

    public static List<string> ReadTextStringList(byte[] textBytes)
    {
        List<string> stringList = new List<string>();
        StreamReader streamReader = new StreamReader((Stream)new MemoryStream(textBytes));
        while (streamReader.Peek() >= 0)
        {
            string str = streamReader.ReadLine();
            if (!string.IsNullOrEmpty(str))
                stringList.Add(str);
            else
                break;
        }
        streamReader.Dispose();
        return stringList;
    }

    public static bool IsLoadResourceFromAssetBundle(string resPath)
    {
        return GameCoreConfig.UseAssetBundle;
    }

    public static bool IsLoadSceneFromAssetBundle(string sceneName)
    {
        return GameCoreConfig.UseAssetBundle;
    }

    public static bool IsLoadShaderFromAssetBundle()
    {
        return GameCoreConfig.UseAssetBundle;
    }

    public static List<string> GetInnerExportAbFile()
    {
        List<string> stringList = new List<string>();
        stringList.Add("deps.bytes");
        stringList.Add("resources.bytes");
        stringList.Add("resources_dir.bytes");
        stringList.Add("never_expire.bytes");
        stringList.Add("shader_path_info.bytes");
        if (GameCoreConfig.UseSmallPackage)
            stringList.Add("priority_list.bytes");
        return stringList;
    }

    public static uint[] GetEncodeKey()
    {
        if (AssetBundleUtil.s_encKey == null)
            AssetBundleUtil.s_encKey = FastXXTEA.ToUInt32Array(Encoding.UTF8.GetBytes("doX%ILyDODme#8X*^#sf"), false);
        return AssetBundleUtil.s_encKey;
    }

    public static byte[] EncodeAb(ref byte[] data)
    {
        return FastXXTEA.Doxx(data, AssetBundleUtil.GetEncodeKey());
    }

    public static byte[] DecodeAb(ref byte[] data)
    {
        return FastXXTEA.Dexx(data, AssetBundleUtil.GetEncodeKey());
    }
}

public class FastXXTEA
{
    private const int BLOCK_SIZE = 16;
    private const int KEY_SIZE = 16;

    private static uint Byte2Uint(byte[] aucData, int index)
    {
        uint num = 0;
        for (int index1 = 0; index1 < 4; ++index1)
            num |= (uint)aucData[index1 + index] << (index1 << 3);
        return num;
    }

    private static void Uint2Byte(uint dwVal, byte[] Result, int index)
    {
        for (int index1 = 0; index1 < 4; ++index1)
            Result[index1 + index] = (byte)(dwVal >> (index1 << 3));
    }

    private static uint Byte2UintV2(byte[] aucData, int iStartPos, int iLength, int index)
    {
        uint num = 0;
        for (int index1 = 0; index1 < 4; ++index1)
            num |= (uint)aucData[index1 + index + iStartPos] << (index1 << 3);
        return num;
    }

    private static void Uint2ByteV2(
      uint dwVal,
      byte[] Result,
      int iStartPos,
      int iLength,
      int index)
    {
        for (int index1 = 0; index1 < 4; ++index1)
            Result[iStartPos + index1 + index] = (byte)(dwVal >> (index1 << 3));
    }

    public static byte[] Doxx(byte[] textData, uint[] k)
    {
        if (textData == null || k == null)
            return (byte[])null;
        int num1 = (textData.Length >> 4 << 2) - 1;
        if (num1 < 1)
            return textData;
        if (k.Length < 4)
        {
            uint[] numArray = new uint[4];
            k.CopyTo((Array)numArray, 0);
            k = numArray;
        }
        uint num2 = FastXXTEA.Byte2Uint(textData, num1 << 2);
        FastXXTEA.Byte2Uint(textData, 0);
        uint num3 = 2654435769;
        uint num4 = 0;
        int num5 = 6 + 52 / (num1 + 1);
        while (num5-- > 0)
        {
            num4 += num3;
            uint num6 = num4 >> 2 & 3U;
            int num7;
            for (num7 = 0; num7 < num1; ++num7)
            {
                uint num8 = FastXXTEA.Byte2Uint(textData, num7 + 1 << 2);
                FastXXTEA.Uint2Byte(num2 = FastXXTEA.Byte2Uint(textData, num7 << 2) + (uint)(((int)(num2 >> 5) ^ (int)num8 << 2) + ((int)(num8 >> 3) ^ (int)num2 << 4) ^ ((int)num4 ^ (int)num8) + ((int)k[(long)(num7 & 3) ^ (long)num6] ^ (int)num2)), textData, num7 << 2);
            }
            uint num9 = FastXXTEA.Byte2Uint(textData, 0);
            FastXXTEA.Uint2Byte(num2 = FastXXTEA.Byte2Uint(textData, num1 << 2) + (uint)(((int)(num2 >> 5) ^ (int)num9 << 2) + ((int)(num9 >> 3) ^ (int)num2 << 4) ^ ((int)num4 ^ (int)num9) + ((int)k[(long)(num7 & 3) ^ (long)num6] ^ (int)num2)), textData, num1 << 2);
        }
        return textData;
    }

    public static byte[] DoxxV2(byte[] textData, int iStartPos, int iLength, uint[] k)
    {
        if (textData == null || k == null)
            return (byte[])null;
        int num1 = (iLength >> 4 << 2) - 1;
        if (num1 < 1)
            return textData;
        if (k.Length < 4)
        {
            uint[] numArray = new uint[4];
            k.CopyTo((Array)numArray, 0);
            k = numArray;
        }
        uint num2 = FastXXTEA.Byte2UintV2(textData, iStartPos, iLength, num1 << 2);
        FastXXTEA.Byte2UintV2(textData, iStartPos, iLength, 0);
        uint num3 = 2654435769;
        uint num4 = 0;
        int num5 = 6 + 52 / (num1 + 1);
        while (num5-- > 0)
        {
            num4 += num3;
            uint num6 = num4 >> 2 & 3U;
            int num7;
            for (num7 = 0; num7 < num1; ++num7)
            {
                uint num8 = FastXXTEA.Byte2UintV2(textData, iStartPos, iLength, num7 + 1 << 2);
                FastXXTEA.Uint2ByteV2(num2 = FastXXTEA.Byte2UintV2(textData, iStartPos, iLength, num7 << 2) + (uint)(((int)(num2 >> 5) ^ (int)num8 << 2) + ((int)(num8 >> 3) ^ (int)num2 << 4) ^ ((int)num4 ^ (int)num8) + ((int)k[(long)(num7 & 3) ^ (long)num6] ^ (int)num2)), textData, iStartPos, iLength, num7 << 2);
            }
            uint num9 = FastXXTEA.Byte2UintV2(textData, iStartPos, iLength, 0);
            FastXXTEA.Uint2ByteV2(num2 = FastXXTEA.Byte2UintV2(textData, iStartPos, iLength, num1 << 2) + (uint)(((int)(num2 >> 5) ^ (int)num9 << 2) + ((int)(num9 >> 3) ^ (int)num2 << 4) ^ ((int)num4 ^ (int)num9) + ((int)k[(long)(num7 & 3) ^ (long)num6] ^ (int)num2)), textData, iStartPos, iLength, num1 << 2);
        }
        return textData;
    }

    public static byte[] Dexx(byte[] textData, uint[] k)
    {
        if (textData == null || k == null)
            return (byte[])null;
        int num1 = (textData.Length >> 4 << 2) - 1;
        if (num1 < 1)
            return textData;
        if (k.Length < 4)
        {
            uint[] numArray = new uint[4];
            k.CopyTo((Array)numArray, 0);
            k = numArray;
        }
        uint num2 = 0;
        byte[] numArray1 = textData;
        int num3 = num1 << 2;
        uint num4 = 0;
        for (int index = 0; index < 4; ++index)
            num4 |= (uint)numArray1[index + num3] << (index << 3);
        num2 = num4;
        byte[] numArray2 = textData;
        int num5 = 0;
        uint num6 = 0;
        for (int index = 0; index < 4; ++index)
            num6 |= (uint)numArray2[index + num5] << (index << 3);
        uint num7 = num6;
        uint num8 = 2654435769;
        for (uint index1 = (uint)((ulong)(6 + 52 / (num1 + 1)) * (ulong)num8); index1 > 0U; index1 -= num8)
        {
            uint num9 = index1 >> 2 & 3U;
            int num10;
            for (num10 = num1; num10 > 0; --num10)
            {
                byte[] numArray3 = textData;
                int num11 = num10 - 1 << 2;
                uint num12 = 0;
                for (int index2 = 0; index2 < 4; ++index2)
                    num12 |= (uint)numArray3[index2 + num11] << (index2 << 3);
                uint num13 = num12;
                byte[] numArray4 = textData;
                int num14 = num10 << 2;
                uint num15 = 0;
                for (int index2 = 0; index2 < 4; ++index2)
                    num15 |= (uint)numArray4[index2 + num14] << (index2 << 3);
                FastXXTEA.Uint2Byte(num7 = num15 - (uint)(((int)(num13 >> 5) ^ (int)num7 << 2) + ((int)(num7 >> 3) ^ (int)num13 << 4) ^ ((int)index1 ^ (int)num7) + ((int)k[(long)(num10 & 3) ^ (long)num9] ^ (int)num13)), textData, num10 << 2);
            }
            byte[] numArray5 = textData;
            int num16 = num1 << 2;
            uint num17 = 0;
            for (int index2 = 0; index2 < 4; ++index2)
                num17 |= (uint)numArray5[index2 + num16] << (index2 << 3);
            uint num18 = num17;
            byte[] numArray6 = textData;
            int num19 = 0;
            uint num20 = 0;
            for (int index2 = 0; index2 < 4; ++index2)
                num20 |= (uint)numArray6[index2 + num19] << (index2 << 3);
            uint num21 = num7 = num20 - (uint)(((int)(num18 >> 5) ^ (int)num7 << 2) + ((int)(num7 >> 3) ^ (int)num18 << 4) ^ ((int)index1 ^ (int)num7) + ((int)k[(long)(num10 & 3) ^ (long)num9] ^ (int)num18));
            byte[] numArray7 = textData;
            int num22 = 0;
            for (int index2 = 0; index2 < 4; ++index2)
                numArray7[index2 + num22] = (byte)(num21 >> (index2 << 3));
        }
        return textData;
    }

    public static byte[] DexxV2(byte[] textData, int iStartPos, int iLength, uint[] k)
    {
        if (textData == null || k == null)
            return (byte[])null;
        int num1 = (iLength >> 4 << 2) - 1;
        if (num1 < 1)
            return textData;
        if (k.Length < 4)
        {
            uint[] numArray = new uint[4];
            k.CopyTo((Array)numArray, 0);
            k = numArray;
        }
        uint num2 = 0;
        byte[] numArray1 = textData;
        int num3 = num1 << 2;
        uint num4 = 0;
        for (int index = 0; index < 4; ++index)
            num4 |= (uint)numArray1[index + num3 + iStartPos] << (index << 3);
        num2 = num4;
        byte[] numArray2 = textData;
        int num5 = 0;
        uint num6 = 0;
        for (int index = 0; index < 4; ++index)
            num6 |= (uint)numArray2[index + num5 + iStartPos] << (index << 3);
        uint num7 = num6;
        uint num8 = 2654435769;
        for (uint index1 = (uint)((ulong)(6 + 52 / (num1 + 1)) * (ulong)num8); index1 > 0U; index1 -= num8)
        {
            uint num9 = index1 >> 2 & 3U;
            int num10;
            for (num10 = num1; num10 > 0; --num10)
            {
                byte[] numArray3 = textData;
                int num11 = num10 - 1 << 2;
                uint num12 = 0;
                for (int index2 = 0; index2 < 4; ++index2)
                    num12 |= (uint)numArray3[index2 + num11 + iStartPos] << (index2 << 3);
                uint num13 = num12;
                byte[] numArray4 = textData;
                int num14 = num10 << 2;
                uint num15 = 0;
                for (int index2 = 0; index2 < 4; ++index2)
                    num15 |= (uint)numArray4[index2 + num14 + iStartPos] << (index2 << 3);
                FastXXTEA.Uint2ByteV2(num7 = num15 - (uint)(((int)(num13 >> 5) ^ (int)num7 << 2) + ((int)(num7 >> 3) ^ (int)num13 << 4) ^ ((int)index1 ^ (int)num7) + ((int)k[(long)(num10 & 3) ^ (long)num9] ^ (int)num13)), textData, iStartPos, iLength, num10 << 2);
            }
            byte[] numArray5 = textData;
            int num16 = num1 << 2;
            uint num17 = 0;
            for (int index2 = 0; index2 < 4; ++index2)
                num17 |= (uint)numArray5[index2 + num16 + iStartPos] << (index2 << 3);
            uint num18 = num17;
            byte[] numArray6 = textData;
            int num19 = 0;
            uint num20 = 0;
            for (int index2 = 0; index2 < 4; ++index2)
                num20 |= (uint)numArray6[index2 + num19 + iStartPos] << (index2 << 3);
            uint num21 = num7 = num20 - (uint)(((int)(num18 >> 5) ^ (int)num7 << 2) + ((int)(num7 >> 3) ^ (int)num18 << 4) ^ ((int)index1 ^ (int)num7) + ((int)k[(long)(num10 & 3) ^ (long)num9] ^ (int)num18));
            byte[] numArray7 = textData;
            int num22 = 0;
            for (int index2 = 0; index2 < 4; ++index2)
                numArray7[index2 + num22 + iStartPos] = (byte)(num21 >> (index2 << 3));
        }
        return textData;
    }

    public static uint[] ToUInt32Array(byte[] Data, bool IncludeLength)
    {
        int length1 = (Data.Length & 3) == 0 ? Data.Length >> 2 : (Data.Length >> 2) + 1;
        uint[] numArray;
        if (IncludeLength)
        {
            numArray = new uint[length1 + 1];
            numArray[length1] = (uint)Data.Length;
        }
        else
            numArray = new uint[length1];
        int length2 = Data.Length;
        for (int index = 0; index < length2; ++index)
            numArray[index >> 2] |= (uint)Data[index] << ((index & 3) << 3);
        return numArray;
    }
}

public delegate void SetFolderNoSave(string folderPath);

public class XFileUtil
{
    private static SetFolderNoSave m_setImpFunc;

    public static void SetFolderNoSaveImp(SetFolderNoSave imp)
    {
        XFileUtil.m_setImpFunc = imp;
    }

    public static string ReadTextFile(string path)
    {
        string errInfo = (string)null;
        byte[] bytes = XFileUtil.ReadFile(path, ref errInfo);
        if (bytes == null)
            return (string)null;
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] ReadFile(string path)
    {
        string errInfo = (string)null;
        byte[] numArray = XFileUtil.ReadFile(path, ref errInfo);
        if (!string.IsNullOrEmpty(errInfo))
            TDebug.Log("ReadFile failed: {0}", (object)errInfo);
        return numArray;
    }

    public static byte[] ReadFile(string path, ref string errInfo)
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int length = (int)fileStream.Length;
                if (length <= 0)
                {
                    errInfo = "read file failed, empty: " + path + ", expect size: " + length.ToString();
                    return (byte[])null;
                }
                byte[] buffer = new byte[length];
                fileStream.Seek(0L, SeekOrigin.Begin);
                int num = fileStream.Read(buffer, 0, length);
                if (num < length)
                {
                    errInfo = "read file failed: " + path + ", expect size: " + length.ToString() + ", read len: " + num.ToString();
                    return (byte[])null;
                }
                fileStream.Close();
                return buffer;
            }
        }
        catch (Exception ex)
        {
            errInfo = "read file[" + path + "] failed:" + ex.ToString();
        }
        return (byte[])null;
    }

    public static bool WriteToFile(string dirOut, string fileName, string content)
    {
        byte[] bytes = new UTF8Encoding().GetBytes(content);
        return XFileUtil.WriteToFile(dirOut, fileName, bytes, bytes.Length);
    }

    public static bool WriteToFile(string dirOut, string fileName, byte[] data)
    {
        return XFileUtil.WriteToFile(dirOut, fileName, data, data.Length);
    }

    public static bool WriteToFile(string dirOut, string fileName, byte[] data, int length)
    {
        try
        {
            Directory.CreateDirectory(dirOut);
            FileStream fileStream = new FileStream(dirOut + "/" + fileName, FileMode.Create);
            fileStream.Write(data, 0, length);
            fileStream.Flush();
            fileStream.Close();
            TDebug.Log("write asset[{0}] ok", (object)fileName);
            return true;
        }
        catch (Exception ex)
        {
            TDebug.Log("write asset[{0}] failed, error[{1}]", (object)fileName, (object)ex.ToString());
            return false;
        }
    }

    public static bool IsFileExist(string path)
    {
        try
        {
            return File.Exists(path);
        }
        catch (Exception ex)
        {
            TDebug.Log("Check exist exception: {0}", (object)ex.ToString());
        }
        return false;
    }

    public static bool CreateDir(string dir)
    {
        try
        {
            string directoryName = Path.GetDirectoryName(dir);
            if (directoryName != dir && !string.IsNullOrEmpty(directoryName))
                XFileUtil.CreateDir(directoryName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                if (XFileUtil.m_setImpFunc != null)
                    XFileUtil.m_setImpFunc(dir);
            }
            return true;
        }
        catch (Exception ex)
        {
            TDebug.Log("create dir[{0}] failed, error[{1}]", (object)dir, (object)ex.ToString());
            return false;
        }
    }

    public static void DeleteFile(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception ex)
        {
            TDebug.Log("DeleteFile [{0}] failed, error[{1}]", (object)path, (object)ex.ToString());
        }
    }

    public static bool DeleteDir(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            return true;
        }
        catch (Exception ex)
        {
            TDebug.Log("DeleteDir dir[{0}] failed, error[{1}]", (object)dir, (object)ex.ToString());
            return false;
        }
    }
}

