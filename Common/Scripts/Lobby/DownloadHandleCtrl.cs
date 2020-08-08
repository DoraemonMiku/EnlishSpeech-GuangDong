using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DownloadHandleCtrl : MonoBehaviour
{

    public static DownloadHandleCtrl dhc;

    public GameObject dhcObj,closeBtn;
    public Text textProcess, textInfo,textLog;
    public Image imgProcess;

    //public float time = 0.23f;
    APaper apaper;

    /// <summary>
    /// 临时大小
    /// </summary>
    public static ulong tempSize = 0;

    private void Awake()
    {
        dhc = this;
    }

    public void StartToDownload(ClassPaper classPaper)
    {
        closeBtn.SetActive(false);
        textLog.text = "";
        RestUI();
        StartCoroutine(GetAPaperInfo(classPaper));
    }

    public void RestUI()
    {
        textProcess.text = "0%";
        textInfo.text = "";
        
        imgProcess.fillAmount = 0f;
    }


    //List<KeyValuePair<int,string>>   
    List<KeyValuePair<string, string>> allAudiosFiles = new List<KeyValuePair<string, string>>();
    IEnumerator GetAPaperInfo(ClassPaper classPaper)
    {
        
        string url = GetPermisson.GetServerAddress +
            "/Paper/GetAPaper.php?token=" +
            UnityWebRequest.EscapeURL(LoginToKaoShi.userLoginCallback.data.token) +
            "&paperID=" + classPaper.id;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        //Debug.Log(LoginToKaoShi.userLoginCallback.data.token);
        //Debug.Log(url);
        textLog.text += "\n请求试卷信息...";
        textInfo.text = "正在寻找试卷信息...";
         uwr.SendWebRequest();
        while (true)
        {
            textProcess.text = GetProcess(uwr.downloadProgress);
            imgProcess.fillAmount = uwr.downloadProgress;
            if (uwr.isDone) break;
            yield return new WaitForSeconds(0.02f);
        }
      
        if (uwr.error == "" || uwr.error == null)
        {
            try
            {
                 apaper = JsonUtility.FromJson<APaper>(uwr.downloadHandler.text);
                // Debug.Log(uwr.downloadHandler.text);
                switch (apaper.code)
                {

                    case 0:
                        textLog.text += "\n试卷信息请求成功!";



                        /*创建文件夹*/
                        
                        PaperManager.CreateBaseDir(apaper.data.id.ToString() + "EP");
                        textLog.text += "\n创建基文件夹成功!";
                        StartCoroutine(FileDownloadManager());

                        break;
                    default:
                        GlobalUIManager.guim.CreateNewDialogBox(apaper.msg);
                        ShowCloseUI();
                        break;

                }
            }
            catch (System.Exception e)
            {

                textLog.text += "\n试卷信息请求失败!"+e.Message;
                GlobalUIManager.guim.CreateNewDialogBox("发生异常!请联系开发者!" + e.Message);
                ShowCloseUI();
            }


        }
        else
        {
            GlobalUIManager.guim.CreateNewDialogBox(uwr.error);
            textLog.text += "\n发生异常!" + uwr.error;
            ShowCloseUI();
        }

    }

   
    /// <summary>
    /// 下载视频
    /// </summary>
    /// <param name="url"></param>
    /// <param name="vname"></param>
    /// <param name="okCallback"></param>
    /// <returns></returns>
    IEnumerator DownloadVideo(string url,string vname,System.Action okCallback)
    {
       
        processing = true;
        RestUI();
        processing = true;
        textLog.text += "\n开始下载视频:"+vname;
        textInfo.text = "下载视频:"+vname;
        
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        
        uwr.SendWebRequest();
       
        while (true)
        {
            textProcess.text = "(" +
                GetFileSize(uwr.downloadedBytes) +
                "/" + GetFileSize((ulong)(float.Parse(uwr.downloadedBytes.ToString())/uwr.downloadProgress)) +
                ")"+
                GetProcess(uwr.downloadProgress);
            imgProcess.fillAmount = uwr.downloadProgress;
          //  Debug.Log(123);
            if (uwr.isDone)
            {
                break;
                
            }
            yield return new WaitForSeconds(0.02f);
        }
        if (uwr.error == "" || uwr.error==null)
        {
            PaperManager.CreateFile(apaper.data.id.ToString() + "EP", vname, uwr.downloadHandler.data);
            textLog.text += "\n视频:" + vname + " 下载成功!";
            tempSize += uwr.downloadedBytes;
            okCallback?.Invoke();
        }
        else
        {
            textLog.text += "\n视频:" + vname + " 下载失败!";
            ShowCloseUI();
        }

    }
    /// <summary>
    /// 下载音频
    /// </summary>
    /// <param name="url"></param>
    /// <param name="aname"></param>
    /// <param name="okCallback"></param>
    /// <returns></returns>
    IEnumerator DownloadAudio(string url, string aname, System.Action okCallback)
    {
        processing = true;
        RestUI();
        
        textLog.text += "\n开始下载音频:" + aname;
        textInfo.text = "下载音频:" + aname;
        UnityWebRequest uwr = UnityWebRequest.Get(url);

        uwr.SendWebRequest();

        while (true)
        {
            textProcess.text = "(" +
                GetFileSize(uwr.downloadedBytes) +
                "/" + GetFileSize((ulong)(float.Parse(uwr.downloadedBytes.ToString()) / uwr.downloadProgress)) +
                ")" +
                GetProcess(uwr.downloadProgress);
            imgProcess.fillAmount = uwr.downloadProgress;
            //  Debug.Log(123);
            if (uwr.isDone)
            {
                break;

            }
            yield return new WaitForSeconds(0.02f);
        }
        if (uwr.error == "" || uwr.error == null)
        {
            PaperManager.CreateFile(apaper.data.id.ToString() + "EP", aname, uwr.downloadHandler.data);
            textLog.text += "\n音频:" + aname + " 下载成功!";
            tempSize += uwr.downloadedBytes;
            okCallback?.Invoke();
        }
        else
        {
            textLog.text += "\n音频:" + aname + " 下载失败!";
            ShowCloseUI();
        }

    }

 
    int processCount=0;
    bool processing = false;
    IEnumerator FileDownloadManager()
    {

        allAudiosFiles.Clear();
        processCount = 0;
        tempSize = 0;

       
        string basePath = "";
        if (PlayerPrefs.HasKey(SettingSetImagUrl.imgUrlPName)&& !string.IsNullOrWhiteSpace(PlayerPrefs.GetString(SettingSetImagUrl.imgUrlPName)))
        {
            basePath = PlayerPrefs.GetString(SettingSetImagUrl.imgUrlPName) + apaper.data.path;//使用镜像站
            GlobalUIManager.guim.CreateNewDialogTie("使用镜像站加速中~如出现错误可能是由于镜像源异常~可在设置调整~");
        }
            
        else
            basePath = GetPermisson.GetServerAddress + "/Paper/PerperFiles/" + apaper.data.path;

        string[] strs = apaper.data.partb_audio_anser.Split('/');
        string[] strs1= apaper.data.partb_audio_ask.Split('/');
        for (int i = 0; i < strs.Length; i++)
        {
          string str=  strs[i].Trim();
           // Debug.Log(str);
            if (!string.IsNullOrEmpty(str)) allAudiosFiles.Add(new KeyValuePair<string, string>(basePath +str , str));
        }
        for (int i = 0; i < strs1.Length; i++)
        {
            string str = strs1[i].Trim();
            if (!string.IsNullOrEmpty(str)) allAudiosFiles.Add(new KeyValuePair<string, string>(basePath + str, str));
        }
        if (!string.IsNullOrEmpty(apaper.data.partc_audio_name.Trim()))
            allAudiosFiles.Add(new KeyValuePair<string, string>(basePath + apaper.data.partc_audio_name.Trim(), apaper.data.partc_audio_name.Trim()));
        textLog.text += "\n开始下载关键文件...";
        
   
        processing = false;
        bool ok = false;
        while (true)
        {
            if (processing)
                yield return null;

            else

            switch (processCount)
            {
                case 0:
                if(string.IsNullOrEmpty(apaper.data.parta_video_name.Trim()))
                        processCount += 1;
                    else
                                StartCoroutine(DownloadVideo(
                            basePath + apaper.data.parta_video_name,
                            apaper.data.parta_video_name,
                            delegate () {
                                processing = false;
                                processCount += 1;
                            }));
                    break;
                case 1:

                    if (string.IsNullOrEmpty(apaper.data.partb_video_name.Trim()))
                        processCount += 1;
                    else
                        StartCoroutine(DownloadVideo(
                basePath + apaper.data.partb_video_name,
                apaper.data.partb_video_name,
                delegate () {
                    processing = false;
                    processCount += 1;
                }));

                    break;
                case 2:

                        
                       
                      if (allAudiosFiles.Count == 0)
                      {
                                /*创建文件*/
                                textLog.text += "\n创建基础文件...";


                                LuoHaoExamPaper lhep = new LuoHaoExamPaper()
                                {
                                    id = apaper.data.id,
                                    time = System.DateTime.Now.ToString(),
                                    paper = apaper.data,
                                    size = GetFileSize(tempSize)

                                };

                                PaperManager.CreateBaseFile(apaper.data.id.ToString() + "EP", lhep);


                                textLog.text += "\n文件写入成功!";
                                textLog.text += "\n试卷已就绪!";
                                textInfo.text = "试卷下载完毕!祝您答题愉快!";
                                ShowCloseUI();
                            ok = true;
                       }
                       
                      else
                     StartCoroutine(DownloadAudio(allAudiosFiles[0].Key, allAudiosFiles[0].Value,delegate() {

                        processing = false;
                        allAudiosFiles.RemoveAt(0);
                       
                    }));

                    break;
                    
                   
            }
            if (ok) break;
            yield return null;

        }
    }


    public void ShowCloseUI()
    {
        StartCoroutine(GetComponent<LobbyManager>().LoadAllPaper());
        closeBtn.SetActive(true);
    }

    public void Close()
    {
        StopAllCoroutines();
        dhcObj.SetActive(false);
    }

    /// <summary>
    /// 获取进度百分比
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public string GetProcess(float k)
    {
        return Mathf.FloorToInt(k*100f).ToString()+"%";
    }
    /// <summary>
    /// 取得文件大小
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string GetFileSize(ulong num)
    {
    
        num = num*100 / (1024*1024);
        float numa = float.Parse(num.ToString()) / 100f;
        return numa+"MB";

    }

}