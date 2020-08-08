using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CheakerSceneManager : MonoBehaviour
{
    public static CheakerSceneManager cheakerSceneManager;
    /// <summary>
    /// 启动数据
    /// </summary>
    public static StartConfig preData;

    public GameObject ctrlToolRootObj;

    public GameObject ctrlToolLeftObj, ctrlToolRightObj;

    /// <summary>
    /// CELSTA控制机
    /// </summary>
    public PaperCheakerCELSTACtrl paperCheakerCELSTACtrl;

    /// <summary>
    /// CELSTB控制机
    /// </summary>
    public PaperCheakerCELSTBCtrl paperCheakerCELSTBCtrl;

    /// <summary>
    /// CELSTC控制机
    /// </summary>
    public PaperCheakerCELSTCCtrl paperCheakerCELSTCCtrl;

    public PaperCheakerCELSTFinish paperCheakerCELSTFinish;

    private void Awake()
    {
        cheakerSceneManager = this;
    }

    private void Start()
    {
        try
        {
            switch (preData.dataMemory.type)
            {
                case UserMemoryList.MemoryType.GD_CELST:
                    LoadCELST();
                    break;
                case UserMemoryList.MemoryType.CETB4B6:
                    GlobalUIManager.guim.CreateNewDialogBox("This Function not be open!");
                    break;
                default:
                    GlobalUIManager.guim.CreateNewDialogBox("改卷场景加载失败!");
                    break;
            }
        }
        catch
        {
            GlobalUIManager.guim.CreateNewDialogBox("Loading Scene Fail!");
        }
    }
    List<CheakingBehavior> cheakingBehaviors=new List<CheakingBehavior>();

    int nowID = 0;
    public void InitGlobalNav()
    {
        if (cheakingBehaviors.Count == 0)
        {
            ctrlToolLeftObj.SetActive(false);
            ctrlToolRightObj.SetActive(false);
        }
        else
        {
            CloseAll();
            nowID = 0;
            cheakingBehaviors[0].Open();
            CheakNow();
        }
    }

    public void Forward()
    {
        if (nowID < cheakingBehaviors.Count-1)
        {
            CloseAll();
            nowID += 1;//递增
            cheakingBehaviors[nowID ].Open();

            CheakNow();

        }
        
       

    }

    public void CheakNow()
    {
        if (nowID == 0)
            ctrlToolLeftObj.SetActive(false);
        else
            ctrlToolLeftObj.SetActive(true);


        if (nowID + 1 == cheakingBehaviors.Count)
            ctrlToolRightObj.SetActive(false);
        else
            ctrlToolRightObj.SetActive(true);
    }

    public void Backward()
    {
        if (nowID > 0)
        {
            CloseAll();
            nowID -= 1;
            cheakingBehaviors[nowID].Open();

            CheakNow();
           
        }
        

    }

    private void CloseAll()
    {
        for(int i = 0; i < cheakingBehaviors.Count; i++)
        {
            cheakingBehaviors[i].Close();
        }

    }



    [HideInInspector]
    public UserMemoryList.Common memoryData;
    [HideInInspector]
    public CELSTTempPaper celstTempPaper;
    
    /// <summary>
    /// 加载CELST模块
    /// </summary>
    private void LoadCELST()
    {
        // CELSTTempPaper tempPaper = preData.dataMemory;
        memoryData = preData.dataMemory;
        LuoHaoExamPaper lhep = preData.dataPaper;
        string file = File.ReadAllText(memoryData.dataPath);
        celstTempPaper = JsonUtility.FromJson<CELSTTempPaper>(file);
       
        if(paperCheakerCELSTACtrl.InitA(
            lhep.paper.path+"/"+lhep.paper.parta_video_name,
            celstTempPaper.partA, 
            lhep.paper.parta_text))//初始化A
        {
            cheakingBehaviors.Add(paperCheakerCELSTACtrl);
            
        }


        if (paperCheakerCELSTBCtrl.InitB(lhep.paper.partb_keyword_question,celstTempPaper.partBA,
            lhep.paper.partb_keyword_anser,celstTempPaper.partBB
            ))
        {
            cheakingBehaviors.Add(paperCheakerCELSTBCtrl);
        }
        if (paperCheakerCELSTCCtrl.InitC(lhep.paper.partc_keyword_story, celstTempPaper.partC))
        {
            cheakingBehaviors.Add(paperCheakerCELSTCCtrl);
        }



        cheakingBehaviors.Add(paperCheakerCELSTFinish);//完成界面
       //Forward();
       InitGlobalNav();
    }
    /// <summary>
    /// 保存
    /// </summary>
    public void SaveCelstTempPaper()
    {
        File.WriteAllText(memoryData.dataPath,JsonUtility.ToJson(celstTempPaper));
    }





    /// <summary>
    /// 改卷行为
    /// </summary>
    public abstract class CheakingBehavior:MonoBehaviour
    {
        [HideInInspector]
        /// <summary>
        /// 跳过标志
        /// </summary>
        public bool jumpFlag = false;

        /// <summary>
        /// 开启
        /// </summary>
        public abstract void Open();

       

        /// <summary>
        /// 关闭
        /// </summary>
        public abstract void Close();


       


    }

    /// <summary>
    ///启动参数
    /// </summary>
    public class StartConfig
    {
        

        public int idMemory;

        public UserMemoryList.Common dataMemory;

        public LuoHaoExamPaper dataPaper;

         
    }
}
