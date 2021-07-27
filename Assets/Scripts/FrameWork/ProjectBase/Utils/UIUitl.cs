using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class UIUitl
{
    public static void RegUnityUIClickSound(Action<string> onClick)
    {
        UIButtonSound.AddPlaySoundAction(onClick);
    }
}
