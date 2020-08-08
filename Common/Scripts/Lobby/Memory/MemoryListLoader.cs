using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MemoryListLoader : MonoBehaviour
{
    public static MemoryListLoader memoryListLoader;
    public GameObject infoPrefab;
    public GameObject content;
    private void Awake()
    {
        memoryListLoader = this;
    }
    /// <summary>
    /// 加载列表
    /// </summary>
    public void LoadList()
    {
        CommonTools.ClearObjectChilds(content);
        

        PaperManager.GetFileList();//取得文件列表

       UserMemoryList uml= UserMemoryManager.ReadList();
        if (uml == null)
        {
            
        }
        else
        {
            for(int i = uml.allMemorys.Count-1; i >= 0; i--)
            {
                MemoryInfoItemCtrl miic= CommonTools.NewAnObjectA(infoPrefab, content.transform).GetComponent<MemoryInfoItemCtrl>();

                UserMemoryList.Common common = uml.allMemorys[i];//读取

                miic.common = common;//文件
                miic.memoryID = i;//储存ID

                string endText = "";
                switch (common.type)
                {
                    case UserMemoryList.MemoryType.GD_CELST://CELST
                        endText= "<Color='Orange'>#广东高考口语#"+i+"</Color>\n";
                        
                        
                        break;

                    case UserMemoryList.MemoryType.CETB4B6://CETB4B6
                       endText = "<Color='Orange'>#英语四六级#"+i+"</Color>\n";


                        break;
                }
                endText+= "时间:" + common.time;
                miic.mainText.text = endText;
                
            }
        }
    }


    public GameObject ctrlPanel;
    public Text reportText;

    private UserMemoryList.Common tempObject;
    private LuoHaoExamPaper thisExamPaper;
    private int tempObjectID = -1;

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="miic"></param>
    public void OpenWindowsToRead(MemoryInfoItemCtrl miic)
    {
        try
        {
            ctrlPanel.SetActive(true);
            // reportText.text = "正在加载报告...";
            reportText.text = "";
            tempObject = miic.common;
            tempObjectID = miic.memoryID;
            switch (miic.common.type)
            {
                case UserMemoryList.MemoryType.GD_CELST:
                    string file = File.ReadAllText(miic.common.dataPath);

                    
                    CELSTTempPaper tempPaper = JsonUtility.FromJson<CELSTTempPaper>(file);
                    LuoHaoExamPaper lhep= PaperManager.allDownloadedPaperFile[tempPaper.id];
                    lhep.paper.path= PaperManager.allDownloadedPath[tempPaper.id];
                    thisExamPaper = lhep;
                    int totalGrade = 0;
                    int maxGrade = 0;
                    reportText.text += "#"+lhep.paper.id+"#" + lhep.paper.name +"-"+lhep.paper.type +"\n";//头部信息
                    if (!string.IsNullOrEmpty(tempPaper.partA))
                    {
                        reportText.text += GetGradeDescript("PartA模仿朗读", tempPaper.gradeA) + "\n";
                        if (tempPaper.gradeA != -1) totalGrade += tempPaper.gradeA;
                        maxGrade += 20;
                    }

                    string[] audioBA = tempPaper.partBA.Split('|');
                    for (int i = 0; i < tempPaper.gradeBA.Length; i++)
                    {
                        if (audioBA.Length>i &&!string.IsNullOrEmpty(audioBA[i]))
                        {
                            reportText.text += GetGradeDescript("PartB三问第" + (i + 1).ToString() + "题", tempPaper.gradeBA[i]) + "\n";
                            if (tempPaper.gradeBA[i] != -1) totalGrade += tempPaper.gradeBA[i];
                            maxGrade += 2;
                        }
                    }
                    string[] audioBB= tempPaper.partBB.Split('|');
                    for (int i = 0; i < tempPaper.gradeBB.Length; i++)
                    {
                        if (audioBB.Length > i && !string.IsNullOrEmpty(audioBB[i]))
                        {
                            reportText.text += GetGradeDescript("PartB五答第" + (i + 1).ToString() + "题", tempPaper.gradeBB[i]) + "\n";
                            if (tempPaper.gradeBB[i] != -1) totalGrade += tempPaper.gradeBB[i];
                            maxGrade += 2;
                        }
                    }
                    if (!string.IsNullOrEmpty(tempPaper.partC))
                    {
                        reportText.text += GetGradeDescript("PartC故事复述", tempPaper.gradeC) + "\n";
                        if (tempPaper.gradeC != -1) totalGrade += tempPaper.gradeC;
                        maxGrade += 24;
                    }
                    reportText.text += "总分(左所得/右满分):" + totalGrade+" / "+maxGrade;

                    break;
                case UserMemoryList.MemoryType.CETB4B6:
                    GlobalUIManager.guim.CreateNewDialogBox("Sorry!该模式还在开发中!");

                    break;
                default:
                    GlobalUIManager.guim.CreateNewSelectBox("读取本地内存记录时出现错误!是否删除?",delegate(bool ok) {
                        if (ok)
                        {
                            DeleteObj();
                        }

                    });
                    break;
            }

            reportText.text += "";
        }
        catch
        {
           // CloseWindows();
            GlobalUIManager.guim.CreateNewSelectBox("读取本地内存记录失败!可能是由于试卷已被删除导致记录丢失!是否删除?", delegate (bool ok) {
                if (ok)
                
                    DeleteObj();
                
                else
                    CloseWindows();

            });
        }
    }
    public void DeleteObj()
    {
        GlobalUIManager.guim.CreateNewSelectBox("确认删除曾经数据?数据一旦删除无法恢复!", delegate (bool ok) {

            if (ok)
            {
                if (tempObjectID != -1)
                {
                    UserMemoryManager.DeleteOneData(tempObjectID);
                    GlobalUIManager.guim.CreateNewDialogTie("删除成功!");

                }
                else
                {
                    GlobalUIManager.guim.CreateNewDialogTie("未知错误删除失败!");
                }
                CloseWindows();
                LoadList();
            }
            

        });
       
    }

    public void CloseWindows()
    {
        reportText.text = "";
        ctrlPanel.SetActive(false);
        tempObject = null;
        thisExamPaper = null;
        tempObjectID = -1;
    }

    public void GoIntoCheakPaper()
    {
        GlobalUIManager.guim.CreateNewSelectBox("确认进入自评系统?", delegate (bool ok)
        {
            if (ok)
            {
                CheakerSceneManager.preData = new CheakerSceneManager.StartConfig()
                {
                    idMemory = tempObjectID,
                 //   mode = CheakerSceneManager.StartConfig.Mode.CELST_GD,
                    dataMemory = tempObject,
                    dataPaper = thisExamPaper
                };


                UnityEngine.SceneManagement.SceneManager.LoadScene(3);

            }
        });
        
    }

    private string GetGradeDescript(string partName,int grade)
    {
        if (grade == -1)
        {
            return partName + "暂无评改";

        }
        else
        {
            return partName + ":" + grade+"分";
        }
    }
}
