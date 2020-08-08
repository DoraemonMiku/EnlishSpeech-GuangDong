using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PaperCheakerCELSTACtrl : CheakerSceneManager.CheakingBehavior
{

    public GameObject rootObj,videoImgObj,textSourceObj;

    public GameObject setScoreObj;

    public VideoPlayer video;

    public AudioSource userAS, videoAS;

    public Text textSource,textInfo,textAudioMode;

    public Image videoCtrlBtn;

    public Sprite pauseIcon, playIcon;

    public Slider timeLine;

    public Slider frequencyScorceSlider, correctScorceSlider, safeScorceSlider;
    public Text frequencyScorceText, correctScorceText, safeScorceText;

    [HideInInspector]
    public bool loadOK = false;

    public bool InitA(string vpath,string audioStr,string sourceText)
    {
        jumpFlag = false;
        if(string.IsNullOrWhiteSpace(vpath)|| string.IsNullOrWhiteSpace(sourceText))
        {
            jumpFlag = true;
            return false;
        }
        try
        {
            loadOK = false;
            byte[] bytes = CheakerTools.Base64_Decode(audioStr);

            AudioClip ac = WavUtility.ToAudioClip(bytes, 0, "UserPartA");

            userAS.clip = ac;
            timeLine.minValue = 0f;
            timeLine.value = 0f;
            //timeLine.maxValue = ac.length;

            textSource.text = sourceText;
            video.url = vpath;
            video.prepareCompleted += OnCompete;
            video.errorReceived += OnError;
            
            video.Prepare();

            
            return true;
        }
        catch
        {
            return false;
        }  
    }
    bool isSeeked = false;
    public void OnTimeLineValueChange()
    {
        if (!video.isPrepared || userAS.clip==null || !isSeeked) return;
        
        if (timeLine.value == timeLine.maxValue)
        {

            video.Stop();
            userAS.Stop();
            return;
        }

        video.Pause();
        userAS.Pause();
        video.seekCompleted += OnSeekCompleted;
       
        video.time = Mathf.Clamp(timeLine.value, 0f, (float)video.length);
        userAS.time = Mathf.Clamp(timeLine.value, 0f, userAS.clip.length);
        isSeeked = false;
    }
    private void OnSeekCompleted(VideoPlayer vp)
    {
        if (userAS.time == 0f || userAS.time == userAS.clip.length)
            userAS.Play();
        else
            userAS.UnPause();
        video.Play();
        video.seekCompleted -= OnSeekCompleted;
        isSeeked = true;
        

    }
   
    public void OnTriggerDown()
    {
        otherCtrl = true;
    }
    public void OnTriggerUp()
    {
        OnTimeLineValueChange();
        otherCtrl = false;
    }

    public void VideoMode()
    {
       // userAS.time = 0f;
       // video.time = 0f;
     //   video.Play();
        videoImgObj.SetActive(true);
        textSourceObj.SetActive(false);
        

    }
    public void TextMode()
    {
        videoImgObj.SetActive(false);
        textSourceObj.SetActive(true);
    }
    int nowAudioMode = 0;
    public void SetAudioMode()
    {
        switch (nowAudioMode)
        {
            case 0://用户声音
                userAS.mute = false;
                videoAS.mute = true;
                textAudioMode.text = "您的声音";
                nowAudioMode += 1;
                break;
            case 1://视频声音
                userAS.mute = true;
                videoAS.mute =false;
                nowAudioMode += 1;
                textAudioMode.text = "视频声音";
                break;
            case 2://混合
                userAS.mute = false;
                videoAS.mute = false;
                nowAudioMode = 0;
                textAudioMode.text = "双声混合";
                break;
        }
    }

    private void OnCompete(VideoPlayer vp)
    {
        timeLine.maxValue =(float)video.length;
       // userAS.Play();

//        video.Play();

        SetAudioMode();
        VideoMode();

        video.prepareCompleted -= OnCompete;
      // video.errorReceived -= OnError;
        isSeeked = true;
        loadOK = true;
    }

    private void OnError(VideoPlayer vp, string msg)
    {

        // Debug.Log("启动发生异常");

        if (loadOK)
            GlobalUIManager.guim.CreateNewDialogBox("视频播放错误!可能是由于您短时间大量拖动时间轴导致解码器内存溢出!请重新进入!");
        else
            GlobalUIManager.guim.CreateNewDialogBox("视频加载失败!");

    }
    /// <summary>
    /// 其他控制
    /// </summary>
    bool otherCtrl = false;
    private void Update()
    {

        if (userAS.clip != null && video.isPrepared)
        {
            if (video.isPlaying)
                videoCtrlBtn.sprite = pauseIcon;
            else
                videoCtrlBtn.sprite = playIcon;
            if (Mathf.Abs((float)video.time-userAS.time)>0.1f)
            {

                userAS.time =(float) video.time;
            }
            textInfo.text = CommonTools.SecondsToMinutes((float)video.time) + "/" + CommonTools.SecondsToMinutes((float)video.length);
        }
         if (userAS.clip != null&& !otherCtrl)
        timeLine.SetValueWithoutNotify((float)video.time);
    }


    public void ChangePlayerStatus()
    {
        if (!video.isPlaying)
        {
            
            video.Play();

            if (userAS.time == 0f||userAS.time==userAS.clip.length)
                userAS.Play();
            else
                userAS.UnPause();

        }
        else
        {
            userAS.Pause();
            video.Pause();
        }
        
    }



    TempScoreClass scoreClass = new TempScoreClass();

    public void OnScoreChange(int sst)
    {
        Text tarText=null;
        Slider tarSlider=null;
        switch ((ScoreSliderType)sst)
        {
            case ScoreSliderType.Correct:
                tarText = correctScorceText;
                tarSlider = correctScorceSlider;
                break;
            case ScoreSliderType.Safe:
                tarSlider = safeScorceSlider;
                tarText = safeScorceText;
                break;
            case ScoreSliderType.Frequency:
                tarSlider = frequencyScorceSlider;
                tarText = frequencyScorceText;
                break;
        }
        tarText.text = tarSlider.value.ToString()+"/"+tarSlider.maxValue;
        
    }
    [System.Serializable]
    public enum ScoreSliderType
    {
        Correct=0,
        Safe=1,
        Frequency=2

    }
    public void OpenScorePanel()
    {
        setScoreObj.SetActive(true);
        if (video.isPlaying) ChangePlayerStatus();
        CheakerSceneManager.cheakerSceneManager.ctrlToolRootObj.SetActive(false);
    }
    public void CloseScorePanel(bool rest)
    {
        if (rest)
        {
            GlobalUIManager.guim.CreateNewSelectBox("是否还原成未批改状态?",delegate(bool ok) {
                if (ok)
                {
                    scoreClass = new TempScoreClass();
                    CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeA = -1;
                    CheakerSceneManager.cheakerSceneManager.SaveCelstTempPaper();//保存
                   
                    
                }

            });
        }
        else
        {
            scoreClass.correct = correctScorceSlider.value;
            scoreClass.frquency = frequencyScorceSlider.value;
            scoreClass.safe = safeScorceSlider.value;
            CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeA = scoreClass.GetTotal();
            CheakerSceneManager.cheakerSceneManager.SaveCelstTempPaper();//保存
            GlobalUIManager.guim.CreateNewDialogBox("PartA得分:" + scoreClass.GetTotal().ToString() + "/20");
          //  setScoreObj.SetActive(false);
        }
        CheakerSceneManager.cheakerSceneManager.ctrlToolRootObj.SetActive(true);
        setScoreObj.SetActive(false);
    }

    public override void Open()
    {
        rootObj.SetActive(true);
        
    }

    public override void Close()
    {
        rootObj.SetActive(false);
        if (loadOK)
        {
            if (video.isPlaying) ChangePlayerStatus();
        }
    }

    private class TempScoreClass
    {
        /// <summary>
        /// 准确度，权重0.5
        /// </summary>
        public float correct = -1;
        
        /// <summary>
        /// 和字幕的适合度权重0.3
        /// </summary>
        public float safe = -1;

        /// <summary>
        /// 流畅度,权重0.2
        /// </summary>
        public float frquency = -1;

        /// <summary>
        /// 取得总分
        /// </summary>
        /// <returns></returns>
        public int GetTotal()
        {
            float end = 0;
            if (correct != -1) end += correct*0.5f;
            if (safe != -1) end += safe*0.3f;
            if (frquency != -1) end += frquency * 0.2f;
            return Mathf.RoundToInt(end*0.2f);
        }
    }
}
