using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UploadHandleCtrl : MonoBehaviour
{
    public GameObject root,closeBtn,gradeObj;
    public Text logText, infoText, processText,gradeText;
    public Image process;


    
    public void RestAll()
    {
        logText.text = "";
        infoText.text = "准备中...";
        processText.text = "0%";
        process.fillAmount = 0f;
    }

    public void ShowCloseObject()
    {
        closeBtn.SetActive(false);
        
    }
    public void GoLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void Close()
    {
        root.SetActive(false);
    }
}
