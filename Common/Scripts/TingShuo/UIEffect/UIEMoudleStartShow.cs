using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIEMoudleStartShow : MonoBehaviour
{
    public GameObject jianShu;
    public float doFadeTime = 0.23f;
    private void Start()
    {
        Invoke("FadeUI", 1.23f);
       
    }
    public void SetText(string text)
    {
        jianShu.GetComponentInChildren<Text>().text = text;
    }
    void FadeUI()
    {
        jianShu.GetComponentInChildren<Image>().DOFade(0f, doFadeTime);
        jianShu.GetComponentInChildren<Text>().DOFade(0f, doFadeTime);
        Destroy(jianShu, doFadeTime);
    }

}

