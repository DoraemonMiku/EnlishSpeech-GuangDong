using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudioCtrl : MonoBehaviour
{
    public static SceneAudioCtrl sac;
    
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    private Dictionary<string, int> audios = new Dictionary<string, int>();
   
    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        sac = this;
        for(int i = 0; i < audioClips.Count; i++)
        {
            if (audioClips[i] != null) audios.Add(audioClips[i].name,i);
        }
   
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="n">名称</param>
    public void PlayAudio(string n)
    {
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = audioClips[audios[n]];
        audioSource.Play();
    }

    /// <summary>
    /// 播放不在这个代码上的音频
    /// </summary>
    /// <param name="ac"></param>
    public void PlayerOutsizeClip(AudioClip ac)
    {
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = ac;
        audioSource.Play();
    }
}
