using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnalyzeForAnWordUICtrl : MonoBehaviour
{
    public Text wordInfo;

    public Image imgPronAccuracy, imgPronFluency;

    public Text txtPronAccuracy, txtPronFluency;


    public void SetWord(string word,int match)
    {
        string textForMat = "";
        //0：匹配单词、1：新增单词、2：缺少单词、3：错读的词、4：未录入单词。
        switch (match)
        {
            case 0:
                textForMat = "已读到~";
                break;
            case 1:
                textForMat = "画蛇添足!";
                break;
            case 2:
                textForMat = "没读到!";
                break;
            case 3:
                textForMat = "错读!";
                break;
            case 4:
                textForMat = "系统异常!";
                break;
        }

        wordInfo.text = word + "\n" + textForMat;

    }
    public void SetAccuracy(float score)
    {
        if (score == -1f)
        {
            txtPronAccuracy.text = "0.0 / 100.0";
            imgPronAccuracy.fillAmount = 0f;
        }
        else
        {
            txtPronAccuracy.text = score.ToString()+" / 100.0";
            imgPronAccuracy.DOFillAmount(score/100f,1.23f);
        }
    }
    public void SetFluency(float score)
    {
        if (score == -1f)
        {
            txtPronFluency.text = "0.0 / 100.0";
            imgPronFluency.fillAmount = 0f;
        }
        else
        {
            txtPronFluency.text = (score*100f).ToString() + " / 100.0";
            imgPronFluency.DOFillAmount(score, 2.33f);
        }
    }

}
