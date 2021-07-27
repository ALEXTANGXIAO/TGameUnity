public class TextureHelper
{
    /// <summary>
    /// 根据文件路径，返回图集名称
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public static string GetPackageTag(string fullName)
    {
        fullName = fullName.Replace("\\", "/");
        int idx = fullName.LastIndexOf("Assets/");
        if (idx == -1)
        {
            return "";
        }
        string str = fullName.Substring(idx);
        str = str.Substring(0, str.LastIndexOf("/")).Replace("Assets/", "").Replace("/", "_");

        return str;
    }

}