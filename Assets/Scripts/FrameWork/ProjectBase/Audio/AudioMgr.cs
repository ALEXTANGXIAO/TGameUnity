using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioMgr : Singleton<AudioMgr>
{
    private AudioSource bgAudio;

    private float bgValue = 0.5f;

    private GameObject soundObject;

    private List<AudioSource> soundList = new List<AudioSource>();

    private float soundValue = 0.5f;

    public float BGVALUE
    {
        get { return bgValue; }
    }

    public float SOUNDVALUE
    {
        get { return soundValue; }
    }

    public void SetBgValue(float value)
    {
        bgValue = value;
        if (bgAudio == null)
        {
            return;
        }
    }


    public void SetSoundValue(float value)
    {
        bgAudio.volume = bgValue;

        soundValue = value;

        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = soundValue;
        }
    }

    public void PlayBackGroundAudio(string name)
    {
        if (bgAudio == null)
        {
            GameObject obj = new GameObject();
            obj.name = "bgAudio";
            bgAudio = obj.AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(obj);
        }
        ResourcesManager.Instance.LoadAsync<AudioClip>("Audio/BG/" + name, (clip) =>
        {
            bgAudio.clip = clip;
            bgAudio.volume = bgValue;
            bgAudio.loop = true;
            bgAudio.Play();
        });
    }

    public void PauseBackGroundMusic()
    {
        if (bgAudio = null)
        {
            return;
        }
        bgAudio.Pause();
    }

    public void StopBackGroundAudio()
    {
        if (bgAudio = null)
        {
            return;
        }
        bgAudio.Stop();
    }

    public void PlaySound(string name, bool isloop = false, UnityAction<AudioSource> callback = null)
    {
        if(soundObject == null)
        {
            soundObject = new GameObject();
            soundObject.name = "Sound_"+ name;
        }

        ResourcesManager.Instance.LoadAsync<AudioClip>("Audio/" + name, (clip) =>
        {
            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = soundValue;
            source.loop = isloop;
            source.Play();
            soundList.Add(source);
            if(callback != null)
            {
                callback(source);
            }
        });
    }

    public void StopSound(AudioSource audioSource)
    {
        if (soundList.Contains(audioSource))
        {
            soundList.Remove(audioSource);
            audioSource.Stop();
            GameObject.Destroy(audioSource);
        }
    }

    public AudioMgr()
    {
        var saveData = ClientSaveData.Instance.GetSaveData<SettingSaveData>();
        if (saveData.m_saved != 0)
        {
            soundValue = saveData.m_soundvalue;
            bgValue = saveData.m_bgvalue;
        }
        MonoManager.Instance.AddUpdateListener(Update);
    }

    private void Update()
    {
        for(int i = soundList.Count - 1; i > 0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }
}
