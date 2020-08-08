using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DoNetworkingSEOCheaker : MonoBehaviour
{
    [System.Obsolete("这个方法已被弃用")]
    public static IEnumerator WebRequestCheaker(int aid,string partName,System.Action<CheakRespondClass> onRespond,System.Action onDone)
    {
       ;
        UnityWebRequest uwr = UnityWebRequest.Get(GetPermisson.GetServerAddress
            + "/SOE/ToGetMyGrade.php?token="
            + LoginToKaoShi.userLoginCallback.data.token+"&aid="+aid+"&part="+partName);
        uwr.SendWebRequest();
        string txt = "";
        int line = 0;
        while (true)
        {
          
            if (txt != uwr.downloadHandler.text)
            {
                string newMsg = uwr.downloadHandler.text;
                string[] all = newMsg.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
                if (line < all.Length - 1)
                {
                    onRespond?.Invoke(JsonUtility.FromJson<CheakRespondClass>(all[line]));
                    line += 1;
                }
                
                txt = uwr.downloadHandler.text;
                
                //Debug.Log(txt);
            }
            if (uwr.isDone)
            {
                onDone?.Invoke();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }



}

/// <summary>
/// 响应
/// </summary>
[System.Serializable]
public class CheakRespondClass
{
    public int code;
    public string msg;
    public float time;
    public string type;
    public Data data;
    [System.Serializable]
    public class Data
    {
        public int PronAccuracy;
        public int PronFluency;
        public int PronCompletion;
        public int SuggestedScore;
    }
}


