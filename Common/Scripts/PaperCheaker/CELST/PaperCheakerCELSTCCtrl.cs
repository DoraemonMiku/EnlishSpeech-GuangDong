/*
 * 慢慢知道可以写个控制音频的类同意控制
 * 但我也不知道为什么我要单读写音频控制QWQ
 * (ｷ｀ﾟДﾟ´)!!
 * WriteIn:2020/5/4 6:40 AM 青年节快乐~~~
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperCheakerCELSTCCtrl :CheakerSceneManager.CheakingBehavior
{
    public GameObject rootObj;

    public AudioSource audioSource;

    public Text sourceT;

    public Slider audioSlider;

    public Text audioInfoText;

    public Image audioButttonImg;

    public Sprite playIcon, pauseIcon;

    public GameObject cheakPanel;

    public Slider keySlider, proSlider, freSlider;
    public Text keyText, proText, freText;

    public bool InitC(string sourceText,string sourceAudio)
    {
        if (string.IsNullOrWhiteSpace(sourceAudio)) return false;
        try
        {
            AudioClip myAudio = WavUtility.ToAudioClip(CheakerTools.Base64_Decode(sourceAudio));
            audioSource.clip = myAudio;
            audioSlider.maxValue = myAudio.length;
            audioSlider.value = 0f;
            sourceT.text = sourceText;
            return true;
        }
        catch
        {
            return false;
        }
    }
  

    public void OnPointerDown()
    {
        otherCtrl = true;
    }

    public void OnPointerUp()
    {
        if (audioSource.clip != null)
            audioSource.time = audioSlider.value;


        otherCtrl = false;
    }


    private bool otherCtrl = false;
    private void Update()
    {
        if (audioSource.clip == null) return;
        if (!otherCtrl) audioSlider.SetValueWithoutNotify(audioSource.time);
        audioInfoText.text = CommonTools.SecondsToMinutes(audioSource.time) 
            + "/" + CommonTools.SecondsToMinutes(audioSource.clip.length);
        if (audioSource.isPlaying)
            audioButttonImg.sprite = pauseIcon;
        else
            audioButttonImg.sprite = playIcon;

    }

    public void ChangeAudioStatus()
    {

        if (audioSource.clip == null) return;
        if (audioSource.isPlaying)
        {
            //  if (audioPlayer.time == 0f || audioPlayer.time == audioPlayer.clip.length)
            //        audioPlayer.Stop();
            //  else
            audioSource.Pause();

        }
        else
        {
            if (audioSource.time == 0f || audioSource.time == audioSource.clip.length)
            {
                audioSource.time = 0f;
                audioSource.Play();
            }
            else
            {
                audioSource.UnPause();
            }
        }

    }


    public void OpenCheakPanel()
    {
        if (audioSource.isPlaying) ChangeAudioStatus();
        cheakPanel.SetActive(true);
        ChangeGrade(0);
        ChangeGrade(1);
        ChangeGrade(2);

    }

    TempGradeData tgd = new TempGradeData();
    public void ChangeGrade(int id)
    {
        
        switch (id)
        {
            case 0:
                tgd.keyword = keySlider.value;
               keyText.text= keySlider.value.ToString()+"/100";
                break;
            case 1:
                tgd.prounciation = proSlider.value;
                proText.text=proSlider.value.ToString() + "/100";
                break;
            case 2:
                tgd.frequency = freSlider.value;
                freText.text=freSlider.value.ToString() + "/100";
                break;

        }
        
    }

    public void Back()
    {
       
            GlobalUIManager.guim.CreateNewSelectBox("是否重置该题为未批改?",delegate(bool ok) {

                if (ok)
                {
                    CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeC = -1;
                    CheakerSceneManager.cheakerSceneManager.SaveCelstTempPaper();
                    GlobalUIManager.guim.CreateNewDialogBox("重置成功!");
                }
            });
        
        cheakPanel.SetActive(false);

    }

    public void Save()
    {
        CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeC = tgd.GetTotal();
        CheakerSceneManager.cheakerSceneManager.SaveCelstTempPaper();
        GlobalUIManager.guim.CreateNewDialogBox("PartC得分:"+tgd.GetTotal().ToString()+"/"+"24");
        cheakPanel.SetActive(false);
    }

    public override void Close()
    {
        if (audioSource.isPlaying) ChangeAudioStatus();
        rootObj.SetActive(false);   
    }
    public override void Open()
    {
        rootObj.SetActive(true);  
    }


    private class TempGradeData
    {
        public float keyword=-1;
        public float prounciation = -1;
        public float frequency = -1;
        public int GetTotal()
        {
             float reInt = 0;
            if (keyword != -1) reInt += keyword * 0.7f;
            if (prounciation != -1) reInt += prounciation * 0.2f;
            if (frequency != -1) reInt += frequency * 0.1f;
            return Mathf.RoundToInt(reInt*0.24f);
        }
    }
}
