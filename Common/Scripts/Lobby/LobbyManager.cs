using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

   

    public static LobbyManager lm;
    public GameObject listObj;
    public GameObject buttonPerfab;
    public Transform content;

    public AllPaper allPaper;
    private void Awake()
    {
        lm = this;
        
    }


    //服务器地址/Paper/GetAllPaper.php
   
    public IEnumerator LoadAllPaper()
    {
        DialogLoading dl = GlobalUIManager.guim.CreateNewLoading();
        string url = GetPermisson.GetServerAddress +
            "/Paper/GetAllPaper.php?token=" + 
            UnityWebRequest.EscapeURL(LoginToKaoShi.userLoginCallback.data.token);
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        //Debug.Log(LoginToKaoShi.userLoginCallback.data.token);
        //Debug.Log(url);
        yield return uwr.SendWebRequest();
        if (uwr.error == "" || uwr.error == null)
        {
            try
            {
                allPaper = JsonUtility.FromJson<AllPaper>(uwr.downloadHandler.text);
                // Debug.Log(uwr.downloadHandler.text);
                switch (allPaper.code) {

                    case 0:
                        PutContent();
                        break;
                    default:
                        GlobalUIManager.guim.CreateNewDialogBox(allPaper.msg);
                        break;
                        
                }
            }
            catch(System.Exception e)
            {
                GlobalUIManager.guim.CreateNewDialogBox("数据包解析错误!请联系开发者!"+e.Message);

            }

           
        }
        else
        {
            GlobalUIManager.guim.CreateNewDialogBox(uwr.error);
        }
        dl.DestoryThisLoad();
    }

    private void PutContent()
    {
        for (int i = 0; i < content.childCount; i++) {
            Destroy(content.GetChild(i).gameObject);
        }
        PaperManager.GetFileList();//取得文件列表
        List<int> allDownloadedID = PaperManager.GetAllDownloadID();
        for(int i = allPaper.data.Count - 1; i >= 0; i--)
        {
            ClassPaper cp = allPaper.data[i];
            GameObject gm = Instantiate(buttonPerfab,content);
            ExamItemCtrl eic = gm.GetComponentInChildren<ExamItemCtrl>();

            string pj = cp.SoeAllow ? "<Color=Green>支持机器评卷</Color>" : "<Color=Red>不支持机器评卷</Color>";
            //Debug.Log(cp.SoeAllow);
            eic.textItem.text =
                   string.Format(
                       "{0}\n<Color=Red>{1}<Color=Grey>_</Color>{2}</Color>",
                   cp.name,
                  cp.type,
                  pj
                   );
            
            
            if (allDownloadedID.Contains(cp.id))
            {
                LuoHaoExamPaper lhep=PaperManager.allDownloadedPaperFile[allPaper.data[i].id];
                eic.isDownloaded = true;
                eic.classPaper = lhep.paper;
                eic.textItem.text += "\n<Color=Green>已下载</Color> <Color=Orange>大小:" + lhep.size + "</Color>\n<Color=Grey>时间:" + lhep.time + "</Color>";
                eic.downloadBtnText.text = "删除";
                eic.filePath = PaperManager.allDownloadedPath[lhep.id];
            }
            else
            {
                eic.isDownloaded = false;
                eic.classPaper = allPaper.data[i];
                eic.textItem.text += "\n<Color=Orange>未下载</Color>";
                eic.downloadBtnText.text = "下载";

            }
        }
    }

    public void OpenList()
    {
        listObj.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(LoadAllPaper());
        
    }

    public void CloseList()
    {
        listObj.SetActive(false);
    }

    public GameObject loading;
    public  void ShowLoadingUI()
    {
        loading.SetActive(true);
    }
}
