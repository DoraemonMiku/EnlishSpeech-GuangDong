using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class GetDriverPermisson : MonoBehaviour
{
    public Transform content;
    public GameObject buttonPerfab;
    public GameObject enterButtons, tokenUI;

    public LineCtrl lineCtrl;

    private void Awake()
    {
        GetDriverList();
    }
    public void GetDriverList()
    {
#if UNITY_ANDROID
        // Permission.RequestUserPermission(Permission.Microphone);
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            GlobalUIManager.guim.CreateNewDialogBox("请您授权麦克风.\n这个权限将用于考试中录音.");
            Permission.RequestUserPermission(Permission.Microphone);
           
        }
        
#endif
        string[] strs = Microphone.devices;
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        for (int i = 0; i < strs.Length; i++)
        {
            GameObject gm = Instantiate(buttonPerfab, content);
            gm.GetComponentInChildren<Text>().text = strs[i];

            gm.GetComponentInChildren<DeveiceButtonCallback>().deveiceName = strs[i];
            gm.GetComponentInChildren<DeveiceButtonCallback>().callback += SelectMicroPhone;
           
            // Debug.Log(strs[i]);
        }
    }


    public Text chooseText;
    public Transform picsParent;
    public static string nowDevice = "";
    AudioClip record=null;
    /// <summary>
    /// 选择麦克风
    /// </summary>
    /// <param name="deviceName">麦克风名字</param>
    public void SelectMicroPhone(string deviceName)
    {
       
        nowDevice = deviceName;
        chooseText.text = "当前选择:\n" + deviceName+"\n上面的波形图是测试麦克风的图形.";
        try
        {
            if (Microphone.IsRecording(nowDevice)) return;
            record = Microphone.Start(deviceName, true, 999, 16000);
        }
        catch(System.Exception err)
        {
            chooseText.text = "设备:"+deviceName+"连接失败!";
            throw err;
        }
        
    }
    private void Update()
    {
        if (nowDevice != "" && Microphone.IsRecording(nowDevice)) ShowMusicPic();
    }
    
    /// <summary>
    /// 显示音乐波形图
    /// </summary>
    private void ShowMusicPic()
    {
        float[] musicData = new float[256];
        int offest = Microphone.GetPosition(nowDevice) - 256 + 1;
        if (offest < 0) return;
        record.GetData(musicData, offest);
        
        for(int i = 0; i < 256; i++)
        {
            if (i % 8 == 0)
            {
                picsParent.GetChild(i / 8).transform.localScale = new Vector3(0.5f, 0.5f + 10f * musicData[i], 0.5f);
            }
        }
        //Debug.Log(maxValue);
        

    }
    public void SelectOK()
    {
        if (nowDevice != "")
        {
            if (Microphone.IsRecording(nowDevice)) Microphone.End(nowDevice);
            record = null;
            enterButtons.SetActive(true);
            tokenUI.SetActive(false);
            lineCtrl.SetFill(3, 3);
            AutoLoginIntoLobby.data.microPhoneName = nowDevice;
        }
        else
        {
            GlobalUIManager.guim.CreateNewSelectBox("您未选择音频设备,确认继续吗(录音功能不可用,下次无法自动登录)?",delegate(bool ok) {
                if (ok)
                {
                    enterButtons.SetActive(true);
                    tokenUI.SetActive(false);
                    lineCtrl.SetFill(3, 3);
                }
            });
        }
    }
    public void Jump()
    {
      
    }
}
