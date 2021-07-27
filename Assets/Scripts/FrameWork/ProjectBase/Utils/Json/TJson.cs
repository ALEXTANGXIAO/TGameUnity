using System;
using System.Collections.Generic;

using System.Text;


public class TJson : BaseClsTemplate<IJson>
{
    /// <summary>
    /// 序列化JSON
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static DJsonData Deserialize(string json)
    {
        if (m_imp != null)
        {
            return m_imp.Deserialize(json);
        }

        return null;
    }

    /// <summary>
    /// 反序列化JSON
    /// </summary>
    /// <param name="jsonData"></param>
    /// <returns></returns>
    public static string Serialize(DJsonData jsonData)
    {
        if (m_imp != null)
        {
            return m_imp.Serialize(jsonData);
        }

        return string.Empty;
    }
}
