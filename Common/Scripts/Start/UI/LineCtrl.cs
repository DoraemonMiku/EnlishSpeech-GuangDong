using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LineCtrl : MonoBehaviour
{
    public Image lineImg;
    // Start is called before the first frame update
    void Start()
    {
        lineImg.fillAmount = 0f;
    }

    public void SetFill(int a,int b)
    {
        float rank = (float)a / (float)b;
        lineImg.DOFillAmount(rank, 1.23f);
    }
}
