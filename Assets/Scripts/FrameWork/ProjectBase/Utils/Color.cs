using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum ColorType
{
    PINK = 0,
    BLACK = 1,
    QualityOrange = 2,
    QualityBrightRed = 3,
}

public class ColorUtil
{
    private static Dictionary<int, string> ColorDic = new Dictionary<int, string>();
    public static string R(ColorType color,string text)
    {
        if (ColorDic.Count == 0)
        {
            InitDic();
        }

        string Dis;
        ColorDic.TryGetValue((int)color,out Dis);

        return "<color=#" + Dis + ">" + text + "</color>";
    }

    public static void InitDic()
    {
        ColorDic.Add(0, "CC6699");
        ColorDic.Add(1, "27241C");
        ColorDic.Add(2, "ff7e00");
        ColorDic.Add(3, "d70000");
    }
}
