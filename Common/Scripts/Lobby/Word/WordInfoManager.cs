using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WordInfoManager : MonoBehaviour
{
    public GameObject newPlan,wordPlane;
    public void JoinPlan()
    {
        StartCoroutine(SendJoinPlanDataToServer());
    }

    public void LoadPlan()
    {
        newPlan.SetActive(false);
        wordPlane.SetActive(false);
        StartCoroutine(GetMyWordTask());
    }




    public Text wordText;
    private int nowIndex = 0;
    private int allCount=0;
    public void MoveNextWord()
    {
        if (loadingFlag)
        {
            GlobalUIManager.guim.CreateNewDialogTie("请等音频加载完成!");
            return;
        }
        if (nowIndex + 1 == allCount)
        {
            GlobalUIManager.guim.CreateNewDialogTie("已经到最后一个了~");
            return;
        }
        nowIndex += 1;
        SetWord(FindAnWord(nowIndex));
     
    }
    public void MoveLastWord()
    {
        if (loadingFlag)
        {
            GlobalUIManager.guim.CreateNewDialogTie("请等音频加载完成!");
            return;
        }
        if (nowIndex - 1 == -1)
        {
            GlobalUIManager.guim.CreateNewDialogTie("已经到第一个了~");
            return;
        }

        nowIndex -= 1;
        SetWord(FindAnWord(nowIndex));

    }
    private WordListClass.AnWord FindAnWord(int idx)
    {
        int temp = idx+1;
        if((temp - wordsTaskData.today.words.Count) <= 0)
        {
            topicText.text =titleModelStr+ "今日任务:"+temp.ToString()+"/"+ wordsTaskData.today.words.Count;
            return wordsTaskData.today.words[temp-1];
        }
       
        for (int i=0;i< wordsTaskData.review.Length; i++)
        {
            if ((temp  - wordsTaskData.review[i].words.Count) <= 0)
            {
                topicText.text = titleModelStr + "复习任务"+(i+1).ToString()+":" + temp.ToString() + "/" + wordsTaskData.review[i].words.Count;
                return wordsTaskData.review[i].words[temp-1];
            }
            else
                temp=temp - wordsTaskData.review[i].words.Count;
        }
        return null;
    }
    string titleModelStr = "";
    private void SetWord(WordListClass.AnWord anWord)
    {

        if (anWord == null)
        {
            GlobalUIManager.guim.CreateNewDialogTie("出现异常!");
            return;

        }
       
        audioSource.clip =null;
        
        //PlayAudio();
        wordText.text = string.Format("<Color=Orange>{0}</Color>\n",anWord.word);
        for(int i = 0; i < anWord.chinese.Length; i++)
        {
            wordText.text += anWord.chinese[i];
            if (i != anWord.chinese.Length - 1) wordText.text += "\n";
        }
    }

    public Text topicText;
    private GetWordPlanRespondData tempWordPaper;
    private void LoadList()
    {
        if (wordsTaskData == tempWordPaper) return;
        tempWordPaper = wordsTaskData;
        allDownloadAudio.Clear();
        nowIndex = 0;
        allCount = wordsTaskData.today.words.Count;
        
        for (int i = 0; i < wordsTaskData.review.Length; i++)
        {
            allCount += wordsTaskData.review[i].words.Count;
        }
        if (allCount == 0)
        {
            GlobalUIManager.guim.CreateNewDialogBox("!!!解析消息数据包失败!");

            return;
        }
        titleModelStr = string.Format("今日{0}个,复习{1}个,",wordsTaskData.today.words.Count,allCount- wordsTaskData.today.words.Count);
        SetWord(FindAnWord(nowIndex));
    }
    private IEnumerator SendJoinPlanDataToServer()
    {
        DialogLoading dl = GlobalUIManager.guim.CreateNewLoading();
        string url = GetPermisson.GetServerAddress
            + "/Word/NewPlan.php?token="
            + LoginToKaoShi.userLoginCallback.data.token;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
     
        if (!uwr.isHttpError&&!uwr.isNetworkError)
        {

            try
            {
                WordPlanRespondData wprd = JsonUtility.FromJson<WordPlanRespondData>(uwr.downloadHandler.text);
                if (wprd.code == 0)
                {
                    GlobalUIManager.guim.CreateNewDialogBox("加入计划成功!");
                    LoadPlan();
                }
                else
                {
                    GlobalUIManager.guim.CreateNewDialogBox(wprd.msg);
                }
            }
            catch (System.Exception err)
            {
                GlobalUIManager.guim.CreateNewDialogBox("解析消息数据包失败!");
                Debug.Log(uwr.downloadHandler.text);
                Debug.Log(err);
            }

        }
        else
        {
            GlobalUIManager.guim.CreateNewDialogTie("服务器未响应!");
        }
        dl.DestoryThisLoad();
    }
    //[SerializeField]
    private GetWordPlanRespondData wordsTaskData;
    private IEnumerator GetMyWordTask()
    {
        DialogLoading dl = GlobalUIManager.guim.CreateNewLoading();
        string url = GetPermisson.GetServerAddress
            + "/Word/GetPlan.php?token="
            + LoginToKaoShi.userLoginCallback.data.token;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        if (!uwr.isHttpError && !uwr.isNetworkError)
        {

            try
            {
               GetWordPlanRespondData gwprd = JsonUtility.FromJson<GetWordPlanRespondData>(uwr.downloadHandler.text);
                if (gwprd.code == 0)
                {
                    // GlobalUIManager.guim.CreateNewDialogBox("成功!");
                    wordsTaskData = gwprd;
                    LoadList();
                    newPlan.SetActive(false);
                    wordPlane.SetActive(true);
                }
                else if (gwprd.code == 10000)
                {
                    wordsTaskData = gwprd;
                    GlobalUIManager.guim.CreateNewDialogBox("您已经完成所有任务啦!当前内容为该计划的全部单词~");
                    LoadList();
                    newPlan.SetActive(false);
                    wordPlane.SetActive(true);
                }
                else
                {
                    
                    GlobalUIManager.guim.CreateNewDialogTie(gwprd.msg);
                    newPlan.SetActive(true);
                    wordPlane.SetActive(false);
                }
            }
            catch (System.Exception err)
            {
                GlobalUIManager.guim.CreateNewDialogBox("解析消息数据包失败!");
                Debug.Log(uwr.downloadHandler.text);
                Debug.Log(err);
            }

        }
        else
        {
            GlobalUIManager.guim.CreateNewDialogTie("服务器未响应!");
        }
        dl.DestoryThisLoad();
    }


    private Dictionary<int, AudioClip> allDownloadAudio = new Dictionary<int, AudioClip>();
    
    public void PlayAudio()
    {
        if (allDownloadAudio.ContainsKey(nowIndex))
        {
            audioSource.clip = allDownloadAudio[nowIndex];
            audioSource.time = 0f;
            audioSource.Play();
        }
        else
        {
            StartCoroutine(GetAudio(FindAnWord(nowIndex).word,nowIndex));
        }
    }
    public Image playerStatusIcon;
    public Sprite pauseIcon, playIcon;
    private void Update()
    {
        if (audioSource.isPlaying)
        {
            playerStatusIcon.sprite = pauseIcon;
        }
        else
        {
            playerStatusIcon.sprite = playIcon;
        }
        
    }
    public void ChangeStatus()
    {
       // if (audioSource.clip == null) return;
        if (!audioSource.isPlaying)
            PlayAudio();
        else
            audioSource.Stop();
    }

    public AudioSource audioSource;
    public GameObject loadingObj;
    public Image loadingImg;
    private bool loadingFlag = false;
    private IEnumerator GetAudio(string audioText,int idx)
    {
        loadingFlag = true;
        loadingObj.SetActive(true);
        loadingImg.fillAmount = 0f;
        
        string url = GetPermisson.GetServerAddress
            + "/AIQQ/TTS.php?text="
            +UnityWebRequest.EscapeURL( audioText);
        UnityWebRequest uwr = UnityWebRequest.Get(url);
         uwr.SendWebRequest();
        while (true)
        {
            loadingImg.fillAmount = uwr.downloadProgress;
            if (uwr.isDone) break;
            yield return new WaitForEndOfFrame();
        }

        if(uwr.isHttpError || uwr.isNetworkError)
        {
            GlobalUIManager.guim.CreateNewDialogTie("音频获取失败~");
        }
        else
        {
            try
            {
               AudioClip ac= WavUtility.ToAudioClip(uwr.downloadHandler.data, 0, audioText);
                allDownloadAudio.Add(idx, ac);
                PlayAudio();
                loadingObj.SetActive(false);
            }
            catch
            {
                GlobalUIManager.guim.CreateNewDialogTie("音频获取失败~请重试~");
                loadingObj.SetActive(false);
            }
        }

        loadingFlag = false;
    }
}
/// <summary>
/// 响应数据
/// </summary>
[System.Serializable]
public class WordPlanRespondData
{
    public int code = -1;
    public string msg = "";
    
}
/// <summary>
/// 获得单词
/// </summary>
[System.Serializable]
public class GetWordPlanRespondData
{
    public int code = -1;
    public string msg = "";
    public int day = -1;
    public WordListClass today;
    public WordListClass[] review;
}