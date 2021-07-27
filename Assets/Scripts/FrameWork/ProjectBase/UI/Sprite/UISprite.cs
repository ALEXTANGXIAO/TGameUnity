using System;
using UnityEngine;
using UnityEngine.UI;

public class UISprite
{

    /// <summary>
    /// 设置图集图片。
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="spriteName">图片名称</param>
    /// <returns></returns>
    public static void SetSprite(Image image, string spriteName)
    {
        if (image == null)
        {
            return;
        }

        UISpriteHelper.Instance.SetSprite(image, spriteName);
    }

    public static void ClearCacheAtlas()
    {
        UISpriteHelper.Instance.UnloadAllAtlas();
    }

}
