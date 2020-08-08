using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackGroundCtrl : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    // Start is called before the first frame update
    private void Awake()
    {
        audioSource.volume = 0f;
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length - 1)];
        audioSource.Play();
        DoVoiceSize(1f,2.33f);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length - 1)];
            audioSource.Play();
        }
        else
        {
            if (!tw.active &&audioSource.volume == 1f && GetDriverPermisson.nowDevice!="" && Microphone.IsRecording(GetDriverPermisson.nowDevice))
            {
                DoVoiceSize(0.23f, 1.23f);
            }
            if (!tw.active &&audioSource.volume != 1f  && !Microphone.IsRecording(GetDriverPermisson.nowDevice))
            {
                DoVoiceSize(1f, 1.23f);
            }
       }
    }
    Tweener tw;
    private void DoVoiceSize(float end,float time)
    {

        tw=audioSource.DOFade(end, time);
        
    } 
    
}
