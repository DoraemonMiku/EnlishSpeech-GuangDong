using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
//using SOE;

public class RobotCheakingBase : MonoBehaviour
{
    public static int baseExamId = -1;
    public static AudioClip partAClip;
    public static FullResultData fullResultData;


    public AnalyzeManager analyzeManager;
    public CheakingRequestPoster cheakingRequestPoster;

    private void Awake()
    {
        Load();
    }

   void Load()
    {
        StartCoroutine(GetFullResult());
    }

    void ReloadTip(string txt)
    {
        GlobalUIManager.guim.CreateNewSelectBox(txt, delegate (bool ok) {
            if (ok)
            {
                //Reload
                Load();
            }
            else
            {
                SceneManager.LoadScene(1);
            }

        });
    }
    IEnumerator GetFullResult()
    {
        DialogLoading dialogLoading= GlobalUIManager.guim.CreateNewLoading();
        GlobalUIManager.guim.CreateNewDialogTie("分析数据下载中...");
        string url = GetPermisson.GetServerAddress + "/Grade/GetFullResult.php?ID="+baseExamId+"&token=" +
            LoginToKaoShi.userLoginCallback.data.token;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        if (uwr.isHttpError || uwr.isNetworkError)
        {
           ReloadTip("获取信息时网络出现异常!是否重试?");
        }
        else
        {
            try
            {
                FullResultData frd = JsonUtility.FromJson<FullResultData>(uwr.downloadHandler.text);
                if (frd.code == 0)
                {
                  partAClip=WavUtility.ToAudioClip(CheakerTools.Base64_Decode(frd.data.audioPartA),0,"PartA");
                    fullResultData = frd;
                    // var soe = SOEWork.GetMyGrade(frd.data.audioPartA, frd.data.keywordPartA);
                    if (string.IsNullOrWhiteSpace(frd.data.jsonPartA))
                    {
                        GlobalUIManager.guim.CreateNewSelectBox("当前试卷并未批改,是否立即批改?", delegate(bool ok) {
                            if (ok)
                            {
                                cheakingRequestPoster.GoCheak();
                            }
                            else
                            {
                                SceneManager.LoadScene(1);//退出
                            }
                        
                        });
                    }
                    else
                    {
                        CheakingRespondData.ResultClass resultClass = JsonUtility.FromJson<CheakingRespondData.ResultClass>(frd.data.jsonPartA);
                        analyzeManager.LoadAnalyze(resultClass);
                    }
                }
                else
                {
                    ReloadTip(frd.msg+"是否重试?");
                }
            }
            catch (Exception err)
            {
                ReloadTip("未知异常!是否重试?\n" + err.Message);
                Debug.LogError(err.ToString());
            }
        }
        dialogLoading.DestoryThisLoad();
    }
}
[Serializable]
public class MainData
{

    public int resultID;

    public int paperID;

    public int gradeA;

    public int gradeB_A;

    public int gradeB_B;

    public int gradeC;

    public int grade;

    public string jsonPartA;

    public string uploadTime;

    public string cheakTime;

    public string paperName;

    public string paperType;

    public int paperMode;

    public bool SoeAllow;

    public string keywordPartA;

    public string audioPartA;
}
[Serializable]
public class FullResultData
{

    public int code;

    public string msg;

    public MainData data;
}
