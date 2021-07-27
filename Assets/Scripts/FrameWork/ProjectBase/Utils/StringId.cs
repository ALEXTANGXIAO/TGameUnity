using System.Collections.Generic;

public class StringId
{
    private static Dictionary<string, int> m_eventTypeHashMap = new Dictionary<string, int>();
    private static Dictionary<int, string> m_eventHashToStringMap = new Dictionary<int, string>();
    private static int m_currId = 0;

    public static int StringToHash(string val)
    {
        int id;
        if (m_eventTypeHashMap.TryGetValue(val, out id))
        {
            return id;
        }

        id = ++m_currId;
        m_eventTypeHashMap[val] = id;
        m_eventHashToStringMap[id] = val;

        return id;
    }

    public static string HashToString(int hash)
    {
        string val;
        if (m_eventHashToStringMap.TryGetValue(hash, out val))
        {
            return val;
        }

        return string.Empty;
    }
}
