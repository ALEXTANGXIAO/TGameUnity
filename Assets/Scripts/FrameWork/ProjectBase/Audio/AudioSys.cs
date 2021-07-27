using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class AudioSys : BaseLogicSys<AudioSys>
{
    private GameObject m_audioListener;
    private Transform m_audioListenerTrans;

    public override bool OnInit()
    {
        UIUitl.RegUnityUIClickSound(OnClickButtonSound);

        return true;
    }

    private void OnClickButtonSound(string clickAudioType)
    {
        GameMgr.PlaySound(clickAudioType);
    }


    public static AudioMgr GameMgr
    {
        get { return AudioMgr.Instance; }
    }
}
