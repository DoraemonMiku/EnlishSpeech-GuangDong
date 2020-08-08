using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperCheakerCELSTBItemCtrl : MonoBehaviour
{
    public Text text;
    public Image cheakImg;
    public Sprite trueIcon, falseIcon;

    [HideInInspector]
    public string nowTitle = "";
    [HideInInspector]
    public string tarText="";
    [HideInInspector]
    public AudioClip clip;

    [HideInInspector]
    public string partName = "A";
    [HideInInspector]
    public int partID = 0;

   // public System.Action OnClickAction;

    public void OnClick()
    {
        PaperCheakerCELSTBCtrl.paperCheakerCELSTB?.SetTarget(clip, tarText);
        PaperCheakerCELSTBCtrl.paperCheakerCELSTB?.InitScoreValue(partName, partID,this);
    }

    public void SetStatus(bool ok)
    {
        if (ok)
        {
            cheakImg.sprite = trueIcon;
            cheakImg.color = Color.green;
        }
        else
        {
            cheakImg.sprite = falseIcon;
            cheakImg.color = Color.red;
        }
    }
}
