using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ExamItemCtrl : MonoBehaviour
{
    public ClassPaper classPaper;
    public Text textItem;
    public Text downloadBtnText;
    public bool isDownloaded = false;
    public string filePath = "";

    public void Download()
    {
        if (isDownloaded)
        {
            // GlobalUIManager.guim.CreateNewDialogBox("您已经下载该试卷!");
            GlobalUIManager.guim.CreateNewSelectBox("是否删除试卷:"+classPaper.name+classPaper.type+"\n这会将您考试过程中的录音缓存删除.",delegate(bool sel) {
                if (sel)
                {
                    try
                    {
                        System.IO.Directory.Delete(filePath,true);
                        GlobalUIManager.guim.CreateNewDialogBox("操作成功!");
                    }
                    catch
                    {
                        GlobalUIManager.guim.CreateNewDialogBox("操作失败!"+filePath);
                    }
                   StartCoroutine( LobbyManager.lm.LoadAllPaper());
                }
                

            });
            return;
        }
        DownloadHandleCtrl.dhc.dhcObj.SetActive(true);
        DownloadHandleCtrl.dhc.StartToDownload(classPaper);
    }

    public void ClickToRun()
    {
        if (!isDownloaded)
        {
            GlobalUIManager.guim.CreateNewDialogBox("您没有下载该试卷!");
            return;
        }
        GlobalUIManager.guim.CreateNewSelectBox("是否开始考试?\n<Color='Yellow'>完成后录音可选择上传到服务器加密储存,您的音频仅有该账户可见.</Color>", delegate (bool ok) {
            if (!ok) return;
            LobbyManager.lm.ShowLoadingUI();
            ProcessCtrl.classPaper = classPaper;
            ProcessCtrl.allAudioClips?.Clear();
            classPaper.path = PaperManager.allDownloadedPath[classPaper.id];
            StartCoroutine(PaperManager.pm.LoadAnPaperAllAudios(classPaper, delegate (Dictionary<string, AudioClip> acs) {

                ProcessCtrl.allAudioClips = acs;
                ProcessCtrl.treeQuestionText = ProcessCtrl.classPaper.partb_text_question.Split('/');
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);//开始答题...
            }));
        });

       

    }
   
}
