using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSetImagUrl : MonoBehaviour
{
    public static readonly string imgUrlPName = "ImgUrl";
    public InputField imgUrlIF;
    private void Awake()
    {
        if (PlayerPrefs.HasKey(imgUrlPName))
            imgUrlIF.SetTextWithoutNotify(PlayerPrefs.GetString(imgUrlPName));
    }
    public void SetUrl()
    {
        if (string.IsNullOrWhiteSpace(imgUrlIF.text))
        {
            if (PlayerPrefs.HasKey(imgUrlPName))
            {
                PlayerPrefs.DeleteKey(imgUrlPName);
                PlayerPrefs.Save();
            }
        }
        else
        {
            
            PlayerPrefs.SetString(imgUrlPName, imgUrlIF.text);
            PlayerPrefs.Save();
        }
        GlobalUIManager.guim.CreateNewDialogTie("修改镜像地址成功!");
    }
}
