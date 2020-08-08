using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GetMyDoneExam : MonoBehaviour
{
    public static GetMyDoneExam GMDE;

    public Transform targetContent;
    //public GameObject xiangQing;

    public GameObject buttonObjs;

    public Dropdown cheakList;

    public GameObject cheakingObj;

    private void Awake()
    {
        GMDE = this;
        
    }
    public void OnClickCallback()
    {
        StopAllCoroutines();
        StartCoroutine(GetMyDoneExams());
    }

    public void CloseCheakingWindows()
    {
        cheakingObj.SetActive(false);
    }
    public void JumpToDetailScene()
    {
        GlobalUIManager.guim.CreateNewSelectBox("是否转到详情?\n这里可以使用机器批改or查看已批改的详解。\n这将会使用您的网络资源。<23mb。", delegate (bool ok) {
            if (ok)
            {
               // RobotCheakingBase.baseExamId=this
                SceneManager.LoadScene(4);
            }
        });
    }
    public void CleanContent()
    {
        for (int i = 0; i < targetContent.childCount; i++)
        {
            Destroy(targetContent.GetChild(i).gameObject);
        }
    }




    //Grade/GetMyDoneExam.php?token=
    // Start is called before the first frame update
    IEnumerator GetMyDoneExams()
    {
        DialogLoading dl = GlobalUIManager.guim.CreateNewLoading();
        string url = GetPermisson.GetServerAddress + "/Grade/GetMyDoneExam.php?token=" +
            LoginToKaoShi.userLoginCallback.data.token;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        if (uwr.isHttpError || uwr.isNetworkError)
        {
            GlobalUIManager.guim.CreateNewDialogBox("获取信息时连接出现异常!"+uwr.error);
        }
        else
        {
            MyDoneExamClasses mdec = JsonUtility.FromJson<MyDoneExamClasses>(uwr.downloadHandler.text);
            CleanContent();
            if (mdec.code == 0)
            {
                for (int i = 0; i < mdec.data.Length; i++)
                {
                    MyDoneExamClasses.MyDoneExams myData = mdec.data[i];
                    GameObject gm = Instantiate(buttonObjs, targetContent);
                    DoneExamItemCtrl deic = gm.GetComponent<DoneExamItemCtrl>();
                    deic.thisExam = myData;
                    string cheakedText = "";
                    if (myData.grade!=-2)
                    {
                        cheakedText = "<Color=Green>已批改</Color>";
                        deic.SetStatus(1);

                    }
                    else
                    {
                        cheakedText = "<Color=Red>未批改</Color>";
                        deic.SetStatus(0);
                    }

                    deic.descriptText.text = string.Format("试卷代号<Color=Orange>#{0}-模式{3}</Color>{5}\n{1}-{2}\n时间{4}{6}",
                        myData.paperID,
                        myData.paperName,
                        myData.paperType,
                        myData.paperMode,
                        myData.uploadTime,
                        cheakedText,
                        myData.SoeAllow?"<Color=Green>机器改卷</Color>":""
                        );
                }

            }
            else
            {
                GlobalUIManager.guim.CreateNewDialogBox("Code:" + mdec.code + "\n" + mdec.msg);
            }
        }
        dl.DestoryThisLoad();
    }
    /// <summary>
    /// 我完成的考试
    /// </summary>
    [System.Serializable]
    public class MyDoneExamClasses
    {
        public int code;
        public string msg;
        public MyDoneExams[] data;
        [System.Serializable]
        public class MyDoneExams
        {
            public int resultID;
            public int paperID;
            public int gradeA;
            public int gradeB_A;
            public int gradeB_B;
            public int gradeC;
            public int grade;
            public string uploadTime;
            public string cheakTime;
            public string paperName;
            public string paperType;
            public string paperMode;
            public bool SoeAllow;
        }
    }
    /// <summary>
    /// 提交改卷请求
    /// </summary>
    /// <param name="partName"></param>
    /// <param name="rid"></param>
    /// <param name="onOK"></param>
    /// <returns></returns>
    public  IEnumerator PostCheakRequest(string partName, int rid,System.Action<bool,RequestPostOKClasses> onOK)
    {
        DialogLoading dl = GlobalUIManager.guim.CreateNewLoading();
        string url = GetPermisson.GetServerAddress + "/SOE/CheakRequestPoster.php?token=" +
            LoginToKaoShi.userLoginCallback.data.token
            + "&part=" + partName + "&rid=" + rid.ToString();

        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        try
        {
            RequestPostOKClasses rpoc = JsonUtility.FromJson<RequestPostOKClasses>(uwr.downloadHandler.text);
            if(rpoc.code==0)
            onOK?.Invoke(true,rpoc);
            else
                onOK?.Invoke(false,rpoc);
        }
        catch
        {
            onOK?.Invoke(false,null);
        }
        dl.DestoryThisLoad();
        
    }

    /// <summary>
    /// 结果
    /// </summary>
    [System.Serializable]
    public class RequestPostOKClasses
    {
        public int code;
        public string msg;
    }

}
