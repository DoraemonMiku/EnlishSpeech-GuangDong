using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class PaperManager : MonoBehaviour
{
    public static PaperManager pm;

#if UNITY_EDITOR
    public static string filePath = "";
#elif UNITY_ANDROID
    public static string filePath=Application.persistentDataPath+"/Papers/";

#else
    public static string filePath=Application.streamingAssetsPath+"/Papers/";
#endif

  
    /// <summary>
    /// 已下载的路径
    /// </summary>
    public static List<KeyValuePair<string, LuoHaoExamPaper>> allDownloaded = new List<KeyValuePair<string, LuoHaoExamPaper>>();

    /// <summary>
    /// 已下载的位置
    /// </summary>
    public static Dictionary<int, string> allDownloadedPath = new Dictionary<int, string>();

    /// <summary>
    /// 已下载的试卷
    /// </summary>
    public static Dictionary<int, LuoHaoExamPaper> allDownloadedPaperFile = new Dictionary<int, LuoHaoExamPaper>();



    private void Awake()
    {
        pm = this;
    }
    /// <summary>
    /// 获取所有已经下载的ID
    /// </summary>
    /// <returns></returns>
    public static List<int> GetAllDownloadID()
    {
        List<int> re = new List<int>();
        for(int i = 0; i < allDownloaded.Count; i++)
        {
            re.Add(allDownloaded[i].Value.id);
        }
        
        return re;
    }
    /// <summary>
    /// 获取文件列表
    /// </summary>
    public static void GetFileList()
    {
        allDownloaded.Clear();
        allDownloadedPath.Clear();
        allDownloadedPaperFile.Clear();

        if (Directory.Exists(filePath))
        {
            string nfp = filePath.Substring(0, filePath.Length - 1);
            DirectoryInfo directoryInfo = new DirectoryInfo(nfp);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            
            for (int i = 0; i < fileInfos.Length; i++)
            {
               
                FileInfo fi = fileInfos[i];
               // Debug.Log(fi.Extension);
               if (fi.Extension != ".lhep") continue;
               
             string text=   File.ReadAllText(fi.FullName,System.Text.Encoding.UTF8);
                try
                {
                    LuoHaoExamPaper lhep = JsonUtility.FromJson<LuoHaoExamPaper>(text);
                    allDownloaded.Add(new KeyValuePair<string, LuoHaoExamPaper>(fi.Directory.FullName,lhep));
                    allDownloadedPath.Add(lhep.id,fi.Directory.FullName);
                    //Debug.Log(fi.Directory.FullName);
                    allDownloadedPaperFile.Add(lhep.id, lhep);
                }
                catch
                {
                    Debug.LogError("文件"+fi.FullName+"读取错误!");
                }
            }
        }
    }

    /// <summary>
    /// 创建基文件
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="lhep"></param>
    public static void CreateBaseFile(string projectName,LuoHaoExamPaper lhep)
    {
        string path = filePath + projectName;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        File.WriteAllText(path + "/PaperInfo.lhep", JsonUtility.ToJson(lhep),System.Text.Encoding.UTF8);
      
    }
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="projectName"></param>
    public static void CreateBaseDir(string projectName)
    {
        string path = filePath + projectName;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    public static void CreateFile(string projectName,string fname,byte[] file)
    {
        string path = filePath + projectName+"/"+fname;
        File.WriteAllBytes(path,file);
    }

    /// <summary>
    /// 加载全部音频
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public  IEnumerator LoadAnPaperAllAudios(ClassPaper cp,System.Action< Dictionary<string,AudioClip> > callback)
    {
        Dictionary<string, AudioClip> audioClips =new Dictionary<string, AudioClip>();
        string[] stra = cp.partb_audio_ask.Split('/');
        string[] strb = cp.partb_audio_anser.Split('/');
        int acount = stra.Length;
        int bcount = strb.Length;
        ///传值
        ProcessCtrl.threeAnser = strb;
        ProcessCtrl.fiveSubject = stra;

        for (int i = 0; i < stra.Length; i++)
            if (!string.IsNullOrEmpty(stra[i].Trim()))
            {
                StartCoroutine(GetAudio(cp.path + "/" + stra[i], stra[i],
          delegate (string aname, AudioClip ac)
            {
                audioClips.Add(aname, ac);
            }
            ));
            }
            else
                acount -= 1;

        for (int i = 0; i < strb.Length; i++)
            if (!string.IsNullOrEmpty(strb[i].Trim()))
            {
                StartCoroutine(GetAudio(cp.path + "/" + strb[i], strb[i],
          delegate (string aname, AudioClip ac)
          {
              audioClips.Add(aname, ac);
          }
            ));
            }
            else
                bcount -= 1;
        //PartC
        int ccount = 1;
        if (!string.IsNullOrEmpty(cp.partc_audio_name.Trim()))
        {
            StartCoroutine(GetAudio(cp.path + "/" + cp.partc_audio_name, cp.partc_audio_name,
           delegate (string aname, AudioClip ac)
           {
               audioClips.Add(aname, ac);
           }
             ));
        }
        else
            ccount -= 1;

        while (true)
        {
            if (audioClips.Count == acount+bcount +ccount)
            {
                callback?.Invoke(audioClips);
                break;
            }
            yield return null;
        }
    }

  
    /// <summary>
    /// 取得音频
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ac"></param>
    /// <returns></returns>
    public static IEnumerator GetAudio(string path,string aname,System.Action<string,AudioClip> ac)
    {

        yield return new WaitForSeconds(0.1f);
#if UNITY_STANDALONE_WIN
        NAudioTools.LAudioConver conver=null;
        try
        {
             conver = new NAudioTools.LAudioConver(path);
            path = conver.GetWavAudioPath;
            
        }
        catch
        {
            GlobalUIManager.guim.CreateNewDialogTie("转码出现异常!");
        }


        UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);
#endif

#if UNITY_ANDROID
        UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://"+path, AudioType.UNKNOWN);
#endif
            yield return uwr.SendWebRequest();

            //  Debug.Log(path);

            if (uwr.error == "" || uwr.error == null)
            {
                ac?.Invoke(aname, DownloadHandlerAudioClip.GetContent(uwr));

            }
            else
                ac?.Invoke(aname, null);
#if UNITY_STANDALONE_WIN
        conver.DestoryThis();
#endif
    }
    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ac"></param>
    public static void SaveFile(string path,string str)
    {

        File.WriteAllText(path, str);
    }





    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="path"></param>
    public static void CreatePath(string path)
    {
       if(!Directory.Exists(path))Directory.CreateDirectory(path);
    }
}
[System.Serializable]
public class LuoHaoExamPaper
{
    public int id = -1;
    public string time = "时间";
    public string size = "2333333MB";
    public ClassPaper paper;
}