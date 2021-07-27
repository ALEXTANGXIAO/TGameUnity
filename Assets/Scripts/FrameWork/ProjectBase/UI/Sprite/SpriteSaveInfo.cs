using System.Collections.Generic;
using UnityEngine;

public class SpriteSaveInfo : MonoBehaviour
{
    public List<Sprite> Sprites = new List<Sprite>();
    private Dictionary<string, Sprite> m_spriteMap = new Dictionary<string, Sprite>();

    public void CacheSprites()
    {
        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i] == null)
            {
                TDebug.Log("[{0}] cache sprite fail, miss sprite[{1}]", gameObject.name, i);
                continue;
            }

            if (m_spriteMap.ContainsKey(Sprites[i].name))
            {
                TDebug.Log("[{0}] cache sprite fail, repeat name sprite[{1}]", Sprites[i].name, i);
            }
            else
            {
                m_spriteMap[Sprites[i].name] = Sprites[i];
            }
        }
    }

    public Sprite GetSprite(string spriteName)
    {
        Sprite sprite;
        if (m_spriteMap.TryGetValue(spriteName, out sprite))
        {
            return sprite;
        }
        return null;
    }
}