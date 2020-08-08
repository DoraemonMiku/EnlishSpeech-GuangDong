using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PartACtrl : MonoBehaviour
{
    public GameObject textXuanDu,textP1, textP2,textLuYin,textYuanWen,jiShiQi,videoPlayer;
    public VideoPlayer player;

    [HideInInspector]
    public bool scucess = false;
     /// <summary>
     /// 初始化PartA
     /// </summary>
     /// <param name="sourceText">原文</param>
     /// <param name="videoPath">视频路径</param>
    public void SetPartA(string sourceText,string videoPath)
    {
        
       
        textYuanWen.GetComponentInChildren<Text>().text = sourceText;
        player.url = videoPath;

        player.errorReceived += Prepared;
        player.Prepare();
        scucess = true;
       
    }
    void Prepared(VideoPlayer vp,string err)
    {
        if (!vp.isPrepared)
        {
            vp.source = VideoSource.VideoClip;
            vp.clip = Resources.Load<VideoClip>("ErrorVideo");
            vp.Prepare();
            GlobalUIManager.guim.CreateNewDialogBox("PartA部分出错!将跳过!\n"+err);
            scucess = false;
        }
    }
    public void PlayVideo(bool isMute,bool isShow=true)
    {
        
     if(isShow)   videoPlayer.SetActive(true);
        SceneAudioCtrl.sac.audioSource.mute = isMute;
        player.Play();
     
        
    }
    public void StopVideo()
    {
        player.time = 0f;
        player.Stop();
      
        player.targetTexture.Release();
        
        player.Prepare();
        videoPlayer.SetActive(false);
        SceneAudioCtrl.sac.audioSource.mute = false;
        
    }
    public double GetAudioTime()
    {
        
        return player.length;
    }
    public void HideAll()
    {
        textXuanDu.SetActive(false);
        textP1.SetActive(false);
        textP2.SetActive(false);
        textLuYin.SetActive(false);
        textYuanWen.SetActive(false);
    }
    public void RunJiShiQi(float t)
    {
        jiShiQi.SetActive(true);
        StartCoroutine(StartJiShiQi(t));
    }
    IEnumerator StartJiShiQi(float t)
    {
        Text text = jiShiQi.GetComponentInChildren<Text>();
        float time = Time.time;
        while (true)
        {
            float nowTime = Time.time - time;
            text.text = Mathf.FloorToInt(t-nowTime)+"秒";
            if (nowTime >= t) {
                jiShiQi.SetActive(false);
                break;
            }
            yield return null;
        }
       
    }
}
