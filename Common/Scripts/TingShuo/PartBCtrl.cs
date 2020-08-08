using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
//using TMPro;
using UnityEngine.UI;

public class PartBCtrl : MonoBehaviour
{
    /// <summary>
    /// 视频播放器
    /// </summary>
    public VideoPlayer player;
    
    /// <summary>
    /// 单独的
    /// </summary>
    public AudioSource pbAudioSource;

    public GameObject xuanDu, jieShao,shortJiShiQi,longJiShiQi,P1_A,P1_B,P2_A,P2_B,videoObj;

    public Text textJieShao,textLongTimer,textShortTimer,textP1_B,textP2_B;

    [HideInInspector]
    public bool scucess = false;
    /// <summary>
    /// 初始化partB
    /// </summary>
    /// <param name="jieShao">情景介绍文本</param>
    /// <param name="videoPath">视频路径</param>

    public void SetPartB(string jieShao,string videoPath)
    {
        //Debug.Log(jieShao);
        string newLine = System.Environment.NewLine;
        jieShao = jieShao.Replace("\\n", newLine);
      //  Debug.Log(jieShao);
        textJieShao.text = jieShao;
        player.url = videoPath;


        player.errorReceived += Prepared;
        player.Prepare();
    
        // SceneAudioCtrl.sac.audioClips.Add();
        textLongTimer.text = "3";
        textShortTimer.text = "剩余时间...";
        scucess = true;
    }
    void Prepared(VideoPlayer vp,string err)
    {
        if (!vp.isPrepared)
        {
            vp.source = VideoSource.VideoClip;
            vp.clip = Resources.Load<VideoClip>("ErrorVideo");
            vp.Prepare();
            GlobalUIManager.guim.CreateNewDialogBox("PartB部分出错!将跳过!\n" + err);
            scucess = false;
        }
    }
    /// <summary>
    /// 播放视频
    /// </summary>
    /// <returns>视频时长</returns>
    public float PlayerVideo()
    {
        if(!player.isPrepared) player.Prepare();
       
        videoObj.SetActive(true);
        player.Play();
        return (float)player.length;
    }


    /// <summary>
    /// 五答
    /// </summary>
    /// <param name="id"></param>
    public float PlayFAnser(int id)
    {

        try
        {
            string[] nums = new string[] { "first", "second", "thrid", "fourth", "fifth" };
            string[] audiosNums = new string[] { "One", "Two", "Three", "Four", "Five" };
            string[] audiosCNums = new string[] { "一", "二", "三", "四", "五" };

            if (id >= nums.Length || id >= audiosNums.Length || id >= ProcessCtrl.fiveSubject.Length) return 0f;

            //计算时间
            float needT = 2f * 1.23f + 2f * (ProcessCtrl.allAudioClips[ProcessCtrl.fiveSubject[id]].length + jgTime);

            textP2_B.text = "<Color=Black>现在请准备回答第" + audiosCNums[id] + "个问题</Color>\n\n<Color=Orange>Please get ready to answer the " + nums[id] + " question.</Color>";
            StartCoroutine(IEPlayFAnser(audiosNums[id], ProcessCtrl.fiveSubject[id]));

            return needT;
        }
        catch
        {
            return 0f;
        }
    }

    /// <summary>
    /// 三问
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public float[] PlayTQuestion(int id)
    {
        try
        {
            float[] re = new float[3];
            if (id >= ProcessCtrl.treeQuestionText.Length) return null;
            string targetSubject = ProcessCtrl.treeQuestionText[id];
            textP1_B.text = targetSubject;
            StartCoroutine(IEPlayTQuestion(20f, 10f, ProcessCtrl.threeAnser[id]));
            re[0] = 20f;
            re[1] = 10f;
            re[2] = 2f * (ProcessCtrl.allAudioClips[ProcessCtrl.threeAnser[id]].length + jgTime);

            return re;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 间隔时间
    /// </summary>
    float jgTime = 2.3f;
  /// <summary>
  /// 五答
  /// </summary>
  /// <param name="audio_num">数字音频名字</param>
  /// <param name="audio_questionName">问题音频名字</param>
  /// <returns></returns>
   IEnumerator IEPlayFAnser(string audio_num,string audio_questionName)
    {
       
       
        pbAudioSource.clip = ProcessCtrl.allAudioClips[audio_questionName];
        //第一次
        pbAudioSource.Stop();
        SceneAudioCtrl.sac.PlayAudio(audio_num);
        yield return new WaitForSeconds(1.23f);
        pbAudioSource.Play();
        yield return new WaitForSeconds(pbAudioSource.clip.length+jgTime);

        //第二次
        pbAudioSource.Stop();
        SceneAudioCtrl.sac.PlayAudio(audio_num);
        yield return new WaitForSeconds(1.23f);
        pbAudioSource.Play();
        yield return new WaitForSeconds(pbAudioSource.clip.length + jgTime);
    }

    /// <summary>
    /// 三问
    /// </summary>
    /// <returns></returns>
    
    IEnumerator IEPlayTQuestion(float readyTime,float recordTime,string audio_anserName)
    {
        pbAudioSource.clip = ProcessCtrl.allAudioClips[audio_anserName];
        
        yield return new WaitForSeconds(readyTime);
       //录制
        yield return new WaitForSeconds(recordTime);
       //第一次
        pbAudioSource.Stop();
        pbAudioSource.Play();
        yield return new WaitForSeconds(ProcessCtrl.allAudioClips[audio_anserName].length+jgTime);
        //第二次
        pbAudioSource.Stop();
        pbAudioSource.Play();
        yield return new WaitForSeconds(ProcessCtrl.allAudioClips[audio_anserName].length + jgTime);
    }



    /// <summary>
    /// 重置全部
    /// </summary>
    public void RestAll()
    {
        xuanDu.SetActive(false);
        jieShao.SetActive(false);
        shortJiShiQi.SetActive(false);
        longJiShiQi.SetActive(false);
        P1_A.SetActive(false);
        P1_B.SetActive(false);
        P2_A.SetActive(false);
        P2_B.SetActive(false);
        // questionSubject.SetActive(false);
        //  questionText.SetActive(false);
        // anserText.SetActive(false);
        videoObj.SetActive(false);
    }
    /// <summary>
    /// 启动短计时器
    /// </summary>
    public void RunShortTimer(float t)
    {
        StartCoroutine(StartShortTimer(t));

    }
    /// <summary>
    /// 启动小计时器
    /// </summary>
    /// <param name="ntime"></param>
    /// <returns></returns>
    IEnumerator StartShortTimer(float ntime)
    {
        float time = Time.realtimeSinceStartup;
        while (true)
        {
            float nowTime = Time.realtimeSinceStartup - time;
           textShortTimer.text = Mathf.CeilToInt(ntime - nowTime).ToString();
            if (nowTime >= ntime)
            {
                shortJiShiQi.SetActive(false);
                break;
            }
            yield return null;
        }
    }
    public void RunLongTimer(float t)
    {
        StartCoroutine(StartLongTimer(t));

    }
    /// <summary>
    /// 启动长时间计时器
    /// </summary>
    /// <param name="ntime"></param>
    /// <returns></returns>
    IEnumerator StartLongTimer(float ntime)
    {
        float time = Time.realtimeSinceStartup;
        while (true)
        {
            float nowTime = Time.realtimeSinceStartup - time;
            textLongTimer.text ="<Color=Green>准备时间:</Color>"+ Mathf.CeilToInt(ntime - nowTime).ToString();
            if (nowTime >= ntime)
            {
                longJiShiQi.SetActive(false);
                break;
            }
            yield return null;
        }
    }
}
