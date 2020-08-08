using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager guim;
    [Header("画布")]
    public Transform canvas;
    private void Awake()
    {
        guim = this;
        DontDestroyOnLoad(this); 
    }
    [Header("预制体-提示盒子")]
    public GameObject dialogBox;
    [Header("预制体-可选提示盒子")]
    public GameObject dialogBoxChoose;
    [Header("预制体-加载提示")]
    public GameObject dialogLoading;
    [Header("预制体-提示条")]
    public GameObject dialogTie;


    /// <summary>
    /// 创建新的提示框
    /// </summary>
    /// <param name="text"></param>
    public void CreateNewDialogBox(string text)
    {
      GameObject gm=  Instantiate(dialogBox, canvas);
        gm.GetComponent<DialogBox>().ChangeText(text);
        gm.transform.localPosition = Vector3.zero;
    }
    /// <summary>
    /// 创建新的提示框
    /// </summary>
    /// <param name="text"></param>
    /// <param name="onSelect"></param>
    public void CreateNewSelectBox(string text,System.Action<bool> onSelect)
    {
        GameObject gm = Instantiate(dialogBoxChoose, canvas);
        gm.GetComponent<DialogBoxChoose>().ChangeText(text);
        gm.GetComponent<DialogBoxChoose>().onSelect = onSelect;
        gm.transform.localPosition = Vector3.zero;
       
    }
    /// <summary>
    /// 创建新的加载提示
    /// </summary>
    /// <param name="text"></param>
    public DialogLoading CreateNewLoading()
    {
        GameObject gm = Instantiate(dialogLoading, canvas);
       
        gm.transform.localPosition = Vector3.zero;
        return gm.GetComponent<DialogLoading>();
    }
    /// <summary>
    /// 创建提示条
    /// </summary>
    /// <param name="text">文本</param>
    public void CreateNewDialogTie(string text)
    {
        GameObject gm = Instantiate(dialogTie, canvas);

        gm.transform.localPosition = Vector3.zero;
        gm.GetComponent<DialogTie>().SetText(text);
    }

    //n!/(n-m)!  +   m[n! / (n - m + 1)!]=(n+1)!/(n+1-m)!

}
