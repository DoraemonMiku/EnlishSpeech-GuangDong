using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
//using UnityEditorInternal;
using UnityEngine.SceneManagement;

public class AnalyzeManager : MonoBehaviour
{
    public float fadeTime = 1.23f;

    public Image imgPronAccuracy, imgPronFluency, imgPronCompletion;

    public Text txtPronAccuracy, txtPronFluency, txtPronCompletion;

    public Text txtAllCount;

    public void LoadAnalyze(CheakingRespondData.ResultClass crdrc)
    {
        imgPronAccuracy.DOFillAmount(0f, 0f);
        imgPronFluency.DOFillAmount(0f, 0f);
        imgPronCompletion.DOFillAmount(0f, 0f);

        if (crdrc.PronAccuracy == -1f) //乱读不做处理~
        {
            txtAllCount.text = "乱读?!";
            txtPronAccuracy.text = "乱读!";
            txtPronFluency.text = "乱读!";
            txtPronCompletion.text = "乱读!";

            return;
        }
        else
        {
            txtAllCount.text = "总分\n"+crdrc.SuggestedScore.ToString()+" / 100.0";
            imgPronAccuracy.DOFillAmount(crdrc.PronAccuracy / 100f, fadeTime);
            imgPronFluency.DOFillAmount(crdrc.PronFluency, fadeTime);
            imgPronCompletion.DOFillAmount(crdrc.PronCompletion, fadeTime);

            txtPronAccuracy.text = crdrc.PronAccuracy.ToString() + " / 100.0";
            txtPronFluency.text = (crdrc.PronFluency * 100f).ToString() + " / 100.0";
            txtPronCompletion.text=(crdrc.PronCompletion*100f).ToString() + " / 100.0"; 

        }

        LoadWordList(crdrc.Words);
        

    }

    public void LoadWordList(CheakingRespondData.WordClass[] words)
    {

        StartCoroutine(LoadList(words));
    }

    public GameObject contentObj;
    public GameObject partPerfab;
    IEnumerator LoadList(CheakingRespondData.WordClass[] words)
    {
        CommonTools.ClearObjectChilds(contentObj);
        for (int i = 0; i < words.Length; i++)
        {
            GameObject gm=CommonTools.NewAnObjectA(partPerfab, contentObj.transform);
            AnalyzeForAnWordUICtrl afawuic = gm.GetComponent<AnalyzeForAnWordUICtrl>();
            afawuic.SetWord(words[i].Word, words[i].MatchTag);
            afawuic.SetAccuracy(words[i].PronAccuracy);
            afawuic.SetFluency(words[i].PronFluency);
            yield return new WaitForSeconds(0.23f);
        }
    }

    public void Exit()
    {
        GlobalUIManager.guim.CreateNewSelectBox("确认回到大厅?",delegate(bool ok) { if(ok) SceneManager.LoadScene(1); });
        
    }
}
