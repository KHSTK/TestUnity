using System;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;         // 音频剪辑的名称
    public AudioClip clip;      // 音频剪辑
    [Range(0f, 1f)]
    public float volume = 0.7f; // 音量大小
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    //定义音乐和音效的Sound数组
    public Sound[] musicSounds, sfxSounds;
    //音乐和音效的AudioSource
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //在场景切换时不销毁该对象
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //播放音乐的方法，参数为音乐名称
    public void PlayMusic(string name)
    {
        //从音乐Sounds数组中找到名字匹配的Sound对象
        Sound s = Array.Find(musicSounds, x => x.name == name);
        //如果找不到对应的Sound，输出错误信息
        if (s == null)
        {
            Debug.Log("没有找到音乐");
        }
        //否则将音乐源的clip设置为对应Sound的clip并播放
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    //播放音效的方法，参数为音效名称
    public void PlaySFX(string name)
    {
        //从音效Sounds数组中找到名字匹配的Sound对象
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        //如果找不到对应的Sound，输出错误信息
        if (s == null)
        {
            Debug.Log("没有找到音效");
        }
        //否则播放对应Sound的clip
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
