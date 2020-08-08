using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CheakingRequestPoster : MonoBehaviour
{
    public AnalyzeManager analyzeManager;
    public void GoCheak()
    {
        StartCoroutine(GoCheaking(RobotCheakingBase.baseExamId));
    }
    private IEnumerator GoCheaking(int ID)
    {

        DialogLoading loading = GlobalUIManager.guim.CreateNewLoading();
        GlobalUIManager.guim.CreateNewDialogTie("服务器正在批改~请等待~等待时间不会超过120秒~");
        string url = GetPermisson.GetServerAddress + "/SOE/AudioReadCheaker.php?ID=" + ID + "&token=" +
               LoginToKaoShi.userLoginCallback.data.token;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
      
        uwr.timeout = 360;
        yield return uwr.SendWebRequest();
        
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            GlobalUIManager.guim.CreateNewDialogBox("网络异常!");
            Debug.LogError(uwr.error);
//            Debug.LogError(uwr.downloadHandler.text);
        }
        else
        {
            try
            {
                CheakingRespondData crd = JsonUtility.FromJson<CheakingRespondData>(uwr.downloadHandler.text);
                if (crd.code == 0)
                {
                    
                    analyzeManager.LoadAnalyze(crd.data.res);
                    GlobalUIManager.guim.CreateNewDialogBox(crd.msg);

                }
                else
                {
                    GlobalUIManager.guim.CreateNewDialogBox(crd.msg);
                    SceneManager.LoadScene(1);
                }
            }
            catch(Exception err)
            {
                GlobalUIManager.guim.CreateNewDialogBox("数据包解析异常!请联系管理员!");
                Debug.LogError(err);
                SceneManager.LoadScene(1);
            }
        }

        loading.DestoryThisLoad();

    }

    
}

[Serializable]
/// <summary>
/// 返回的数据包
/// </summary>
public class CheakingRespondData
{
    public int code;
    public string msg;
    public Data data;

    [Serializable]
    public class Data
    {
        public Init init;
        public ResultClass res;
    }
    [Serializable]
    public class Init
    {
        public string SessionId;
        public string RequestId;
    }
    [Serializable]
    public class ResultClass
    {
        public float PronAccuracy;
        public float PronFluency;
        public float PronCompletion;
        public WordClass[] Words;
        public string SessionId;
        public string AudioUrl;
        public Sentenceinfoset[] SentenceInfoSet;
        public string Status;
        public float SuggestedScore;
        public string RequestId;
    }
    [Serializable]
    public class WordClass
    {
        public int MemBeginTime;
        public int MemEndTime;
        public float PronAccuracy;
        public float PronFluency;
        public string Word;
        public int MatchTag;
        //public object[] PhoneInfos;
        public string ReferenceWord;
    }
    [Serializable]
    public class Sentenceinfoset
    {
        public int SentenceId;
        public WordClass[] Words;
        public float PronAccuracy;
        public float PronFluency;
        public float PronCompletion;
        public int SuggestedScore;
    }


}

