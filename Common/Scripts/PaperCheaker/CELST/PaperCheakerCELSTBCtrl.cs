using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperCheakerCELSTBCtrl : CheakerSceneManager.CheakingBehavior
{
    public static PaperCheakerCELSTBCtrl paperCheakerCELSTB;

    public GameObject partBrootObj;

    public AudioSource audioPlayer;

    public Slider audioCtrl;

    public Text daAnText;

    public GameObject perfabItemBtn;

    public Transform tarContent;

    public Image statusIcon;

    public Sprite playIcon, pauseIcon;

    public Text audioPlayerInfo;


    public Slider scoreSlider;

    public Text scoreText;

    //public List<System.Action> buttonsOnclikDelege;
    private void Awake()
    {
        paperCheakerCELSTB = this;
    }



    public bool InitB(string partBA_Ansers,string partBA_Audios,string partBB_Ansers,string partBB_Audios)
    {
        
        try
        {
            //  System.Action tempAction = null;
            PaperCheakerCELSTBItemCtrl tempItemCtrl=null;
            if (!string.IsNullOrWhiteSpace(partBA_Ansers) && !string.IsNullOrWhiteSpace(partBA_Audios))//PartA不为空
            {
                string[] partBA_Ansers_strs = partBA_Ansers.Split('/');
                string[] partBA_Audios_strs = partBA_Audios.Split('|');


               // AudioClip[] clips = new AudioClip[partBA_Audios_strs.Length];
                for (int i = 0; i < partBA_Audios_strs.Length; i++)
                {
                   AudioClip clip = WavUtility.ToAudioClip(CheakerTools.Base64_Decode(partBA_Audios_strs[i]), 0, "ParBA_Audio_" + i);
                    PaperCheakerCELSTBItemCtrl pccbic = CommonTools.NewAnObjectA(perfabItemBtn, tarContent).GetComponent<PaperCheakerCELSTBItemCtrl>();
                    if (i < CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBA.Length)
                    {

                        if (CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBA[i] == -1)
                        {
                            pccbic.SetStatus(false);
                            pccbic.nowTitle = "三问第" + (i + 1).ToString() + "题";
                            pccbic.text.text = pccbic.nowTitle;

                        }
                        else
                        {
                            pccbic.SetStatus(true);
                            pccbic.nowTitle = "三问第" + (i + 1).ToString() + "题";
                            pccbic.text.text = pccbic.nowTitle+"-"+
                                CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBA[i].ToString();
                        }



                        pccbic.clip = clip;
                            string anser = "";
                            if (i < partBA_Ansers_strs.Length) anser = partBA_Ansers_strs[i];
                        pccbic.tarText = "三问第" + (i + 1).ToString() + "题答案:\n"
                            + "<Color=Orange>" + anser + "</Color>"
                            + "\n请依据关键词给分.";
                        pccbic.partName = "A";
                        pccbic.partID = i;

                        if (tempItemCtrl == null) tempItemCtrl = pccbic;

                    }
                    else
                    {
                        GlobalUIManager.guim.CreateNewDialogBox("PartB改卷系统出现异常!");
                    }

                }

            }



            if (!string.IsNullOrWhiteSpace(partBB_Ansers) && !string.IsNullOrWhiteSpace(partBB_Audios))//PartB不为空
            {


                string[] partBB_Ansers_strs = partBB_Ansers.Split('/');
                string[] partBB_Audios_strs = partBB_Audios.Split('|');


               // AudioClip[] clips = new AudioClip[partBB_Audios_strs.Length];
                for (int i = 0; i < partBB_Audios_strs.Length; i++)
                {
                    AudioClip clip = WavUtility.ToAudioClip(CheakerTools.Base64_Decode(partBB_Audios_strs[i]), 0, "ParBB_Audio_" + i);
                    PaperCheakerCELSTBItemCtrl pccbic = CommonTools.NewAnObjectA(perfabItemBtn, tarContent).GetComponent<PaperCheakerCELSTBItemCtrl>();
                    if (i < CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBB.Length)
                    {

                        if (CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBB[i] == -1)
                        {
                            pccbic.SetStatus(false);
                            pccbic.nowTitle= "五答第" + (i + 1).ToString() + "题";
                            pccbic.text.text = pccbic.nowTitle;
                        }
                        else
                        {
                            pccbic.SetStatus(true);
                            pccbic.nowTitle = "五答第" + (i + 1).ToString() + "题";
                            pccbic.text.text = pccbic.nowTitle+"-"+ CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBB[i].ToString();
                        }

                        pccbic.clip = clip;

                        string anser = "";
                        if (i < partBB_Ansers_strs.Length)
                            anser = partBB_Ansers_strs[i];
                        pccbic.tarText = "五答第" + (i + 1).ToString() + "题答案:\n"
                        + "<Color=Orange>" + anser + "</Color>"
                        + "\n请依据关键词给分.";
                        pccbic.partName = "B";
                        pccbic.partID = i;

                        if (tempItemCtrl == null) tempItemCtrl = pccbic;
                    }
                    else
                    {
                        GlobalUIManager.guim.CreateNewDialogBox("PartB改卷系统出现异常!");
                    }

                }


            }
            tempItemCtrl?.OnClick();
            if (tempItemCtrl != null) 
              return true;
            else
                return false;
        }
        catch(System.Exception err)
        {
            Debug.Log(err.ToString());
            return false;
        }
        
       
    }


    public void SetTarget(AudioClip clip,string tarText)
    {
        SetAudio(clip);
        
        daAnText.text = tarText;
    }

    private void SetAudio(AudioClip ac)
    {
        if (audioPlayer.isPlaying) audioPlayer.Stop();
        audioPlayer.clip = ac;
        audioPlayer.time = 0f;
        audioCtrl.minValue = 0f;
        audioCtrl.maxValue = ac.length;

    }
    private bool otherCtrl = false;
    public void OnSliderDown()
    {
        otherCtrl = true;
    }
    public void OnSliderUp()
    {
        otherCtrl = false;
        if (audioPlayer.clip == null) return;
        audioPlayer.time = audioCtrl.value;
        if (!audioPlayer.isPlaying) ChangeAudioStatus();
    }
    public void ChangeAudioStatus()
    {
        if (audioPlayer.clip == null) return;
        if (audioPlayer.isPlaying)
        {
          //  if (audioPlayer.time == 0f || audioPlayer.time == audioPlayer.clip.length)
        //        audioPlayer.Stop();
          //  else
                audioPlayer.Pause();

        }
        else
        {
            if (audioPlayer.time == 0f || audioPlayer.time == audioPlayer.clip.length)
            {
                audioPlayer.time = 0f;
                audioPlayer.Play();
            }
            else
            {
                audioPlayer.UnPause();
            }
        }
        

    }


    private void Update()
    {
        if (audioPlayer.clip == null) return;
        if(!otherCtrl)audioCtrl.SetValueWithoutNotify(audioPlayer.time);

        audioPlayerInfo.text = CommonTools.SecondsToMinutes(audioPlayer.time) + 
            "/"
            + CommonTools.SecondsToMinutes(audioPlayer.clip.length);
        if (audioPlayer.isPlaying)
        {
            statusIcon.sprite = pauseIcon;
        }
        else
        {
            statusIcon.sprite = playIcon;
            
        }
    }
    
    public void OnScoreValueChange()
    {
        scoreText.text = ((float)scoreSlider.value / 10f).ToString() + "/2";
    }

    private string nowPartName = "";
    private int nowPartIndex = -1;
    private PaperCheakerCELSTBItemCtrl nowPCCIC;
    public void ChangeScoreAndSave()
    {
        if (audioPlayer.isPlaying) ChangeAudioStatus();
        if (nowPartIndex == -1 || nowPartName=="") return;

        switch (nowPartName)
        {
            case "A":
                CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBA[nowPartIndex]
                    =Mathf.RoundToInt(scoreSlider.value / 9.9f);//防止float误差导致四舍五入偏差
                break;
            case "B":
                CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBB[nowPartIndex]
                    = Mathf.RoundToInt(scoreSlider.value / 9.9f);//防止float误差导致四舍五入偏差
                break;
        }
        nowPCCIC?.SetStatus(true);
        nowPCCIC.text.text = nowPCCIC.nowTitle + "-"+ Mathf.RoundToInt(scoreSlider.value / 9.9f);
        //防止float误差导致四舍五入偏差
        CheakerSceneManager.cheakerSceneManager.SaveCelstTempPaper();
        GlobalUIManager.guim.CreateNewDialogTie("PartB当前小题保存成功!");
    }
    public void RestScoreAndSave()
    {
        if (audioPlayer.isPlaying) ChangeAudioStatus();
        if (nowPartIndex == -1 || nowPartName == "") return;

        switch (nowPartName)
        {
            case "A":
                CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBA[nowPartIndex]
                    = -1;
                break;
            case "B":
                CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBB[nowPartIndex]
                    = -1;
                break;
        }
        nowPCCIC?.SetStatus(false);
        nowPCCIC.text.text = nowPCCIC.nowTitle ;
        CheakerSceneManager.cheakerSceneManager.SaveCelstTempPaper();
        GlobalUIManager.guim.CreateNewDialogTie("PartB当前小题重置成功!");
    }
    public void InitScoreValue(string part, int index,PaperCheakerCELSTBItemCtrl pccbic)
    {
        nowPartName = part;
        nowPartIndex = index;
        nowPCCIC = pccbic;
        switch (part)
        {
            case "A":
                if (CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBA[index] != -1)
                    scoreSlider.value = CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBA[index]*10f;
                else
                    scoreSlider.value = 10f;


                break;
            case "B":
                if (CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBB[index] != -1)
                    scoreSlider.value = CheakerSceneManager.cheakerSceneManager.celstTempPaper.gradeBB[index]*10f;
                else
                    scoreSlider.value = 10f;
                break;
        }
    }


    public override void Close()
    {
        partBrootObj.SetActive(false);
        if (audioPlayer.isPlaying) ChangeAudioStatus();
    }

    public override void Open()
    {
        partBrootObj.SetActive(true);
    }

    
}
