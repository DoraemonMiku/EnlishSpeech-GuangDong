using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserInfoManager : MonoBehaviour
{
    public Text userDescription;

    public void GetInfo()
    {
        UserLoginCallback ulc = LoginToKaoShi.userLoginCallback;
        userDescription.text = string.Format("用户身份码(ID):{0}\n用户名:{1}\nQQ号:{2}\n注册时间:{3}\n最后登录时间:{4}",
            ulc.data.id,ulc.data.name,ulc.data.qq,ulc.data.reg_time,ulc.data.last_login_time);
        
    }
    public void GetHash()
    {
        StartCoroutine(GetMyHashCode());
    }
    private IEnumerator GetMyHashCode()
    {
        DialogLoading dl = GlobalUIManager.guim.CreateNewLoading();
        string url = GetPermisson.GetServerAddress
            + "/User/GetMyQQBindCode.php?token="
            + LoginToKaoShi.userLoginCallback.data.token;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        if(uwr.isNetworkError || uwr.isHttpError)
        {
            GlobalUIManager.guim.CreateNewDialogTie("获取失败!服务器未响应!");
        }
        else
        {
            try
            {
            HashCodeRecData hcrd=    JsonUtility.FromJson<HashCodeRecData>(uwr.downloadHandler.text);
                if (hcrd.code == 0)
                {
                    GUIUtility.systemCopyBuffer = "BindMyAccountByRandomCode:"+hcrd.hcode;//剪切板
                    GlobalUIManager.guim.CreateNewDialogTie("此条内容已复制到剪切板!");

                }
                else
                {
                    GlobalUIManager.guim.CreateNewDialogTie(hcrd.msg);
                }
                
            }
            catch
            {
                Debug.Log(uwr.downloadHandler.text);
                GlobalUIManager.guim.CreateNewDialogTie("未知异常!请联系管理员!");
            }
            
        }
        dl.DestoryThisLoad();
    }
    [System.Serializable]
    class HashCodeRecData
    {
        public int code=-10086;
        public string msg = "";
        public string hcode="";
    }
}
