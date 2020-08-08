using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartCCtrl : MonoBehaviour
{
    public GameObject xuanDu, gengGai, retellText, jishiQi;
    public Text textGengGai, textJiShiQi;
    public AudioSource audioSource;

    [HideInInspector]
    public bool scucess = false;

    public void SetPartC(string gengGaiT)
    {
        string newLine = System.Environment.NewLine;
        gengGaiT = gengGaiT.Replace("\\n", newLine);
        textGengGai.text = gengGaiT;
        try
        {
            audioSource.clip = ProcessCtrl.allAudioClips[ProcessCtrl.classPaper.partc_audio_name];
            
            audioSource.Stop();
            scucess = true;
            if (audioSource.clip == null)
            {
                audioSource.clip = Resources.Load("ErrorAudio") as AudioClip;
                GlobalUIManager.guim.CreateNewDialogBox("PartC部分出错!将跳过!1PC_Not");
                scucess = false;
            }
            //return true;
            
        }
        catch
        {
            GlobalUIManager.guim.CreateNewDialogBox("PartC部分出错!将跳过!2PC_Err");
            scucess = false;
            // return false;
        }
    }
    public void RunTimer(float t)
    {
        StartCoroutine(StartTimer(t));

    }
    public void RestAll()
    {
        xuanDu.SetActive(false);
        gengGai.SetActive(false);
        retellText.SetActive(false);
        jishiQi.SetActive(false);
    }
    /// <summary>
    /// 启动长时间计时器
    /// </summary>
    /// <param name="ntime"></param>
    /// <returns></returns>
    IEnumerator StartTimer(float ntime)
    {
        float time = Time.realtimeSinceStartup;
        while (true)
        {
            float nowTime = Time.realtimeSinceStartup - time;
            textJiShiQi.text = "<color=Green>准备时间:</color>" + Mathf.CeilToInt(ntime - nowTime).ToString();
            if (nowTime >= ntime)
            {
                jishiQi.SetActive(false);
                break;
            }
            yield return null;
        }

    }
    float jgTime = 5f;
    public float PlayStoryAudio()
    {
        float ret = 2f*(audioSource.clip.length + jgTime);
        StartCoroutine(IEPlayStoryAudio());
        return ret;
    }
    IEnumerator IEPlayStoryAudio()
    {
        audioSource.Stop();
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length+jgTime);
        audioSource.Stop();
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length + jgTime);
    }
}
