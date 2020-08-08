using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;

public class HomeMessageItemCtrl : MonoBehaviour
{
    public HomeMessageRec.UserMessageClass.Data umcData;
    public HomeMessageRec.MessageFormat mf;


    public Image iconImg;
    public Text contentText;

    public Sprite[] sprites;


    public void SetItem(int index)
    {
        iconImg.sprite = sprites[index];
    }

    public void SetText(string txt)
    {
        contentText.text = txt;
    }
    //http://106.15.200.140/Home/SetMessageStatus.php?token=token&id=1

    public void OnClickOnOpen()
    {
        HomeMessageRec.HMR.SetMSGBox(mf,umcData);
       if(!umcData.isRead)
            StartCoroutine(SetRead());
    }
    IEnumerator SetRead()
    {
        string url = GetPermisson.GetServerAddress + "/Home/SetMessageStatus.php?token=" +
            LoginToKaoShi.userLoginCallback.data.token
            + "&id=" + umcData.id;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        if (uwr.error == "" || uwr.error == null)
        {
            try
            {
                SetReadJson srj = JsonUtility.FromJson<SetReadJson>(uwr.downloadHandler.text);
                if (srj.code == 0)
                {
                    //GlobalUIManager.guim.CreateNewDialogBox(srj.msg);
                    HomeMessageRec.HMR.OnClick();
                }
                else
                {
                    GlobalUIManager.guim.CreateNewDialogBox(srj.msg);
                }
            }
            catch
            {
                GlobalUIManager.guim.CreateNewDialogBox("解析数据包失败!");
            }


        }
        else
        {
            GlobalUIManager.guim.CreateNewDialogBox("网络异常!");
        }
    }

    public class SetReadJson
    {
        public int code;
        public string msg;
    }

}
