using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCtrl : MonoBehaviour
{
    public GameObject moudleStart,moudelPartA,moudelPartB, moudelPartC,moudleOK;

    public PartACtrl pac;

    public PartBCtrl pbc;

    public PartCCtrl pcc;

    //public CheakVoiceGrade cvg;
    public CheakVoiceGradeTemp cvgt;

    public UploadHandleCtrl uploadHandle;

    public static ClassPaper classPaper;

    public static Dictionary< string, AudioClip> allAudioClips;

    public static string[] threeAnser, fiveSubject ,treeQuestionText;

    [HideInInspector]
    public AudioClip PA;
    [HideInInspector]
    public List<AudioClip> PB_P1 = new List<AudioClip>();
    [HideInInspector]
    public List<AudioClip> PB_P2 = new List<AudioClip>();
    [HideInInspector]
    public AudioClip PC;


    private void SteupPaper(bool a,bool b,bool c)
    {
        moudleStart.GetComponent<UIEMoudleStartShow>().SetText(classPaper.name + "\n" + classPaper.type);
        
       if(a) pac.SetPartA(classPaper.parta_text,classPaper.path+"/"+classPaper.parta_video_name);
        if(b)pbc.SetPartB(classPaper.partb_text_scene, classPaper.path + "/"+ classPaper.partb_video_name);
        if(c)pcc.SetPartC(classPaper.partc_story);
    }
    
    private void Start()
    {
      //  GlobalAudioRecorder.gar.NewRecord(130f, 10f, true, false, delegate (List<AudioClip> acs) { PC = acs; });
       
       
           StartCoroutine(ExamMode());
        //StartCoroutine(PartB(null));
        //StartCoroutine(PartC(null));
        //StartCoroutine(UploadVoiceHandle.Uploader(1, "","", "", "", null, null));

    }
   
    int nowProcessID = 0;
    bool processing = false;
    /// <summary>
    /// 考试模式
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExamMode()
    {
       
        string mode = "";
        for(int i = 0; i <4- classPaper.mode.ToString().Length; i++)
        {
            mode += "0";
        }
        mode += classPaper.mode.ToString();
        SteupPaper(mode[1]=='1', mode[2] == '1', mode[3] == '1');
        bool flag = false;
        while (true)
        {
            if (!processing)
            {
                if(nowProcessID < mode.Length)
                if (mode[nowProcessID] == '0')
                {
                    nowProcessID += 1;
                    continue;
                }
                switch (nowProcessID)
                {
                    case 0:


                        processing = true;

                        StartCoroutine(StartXD(delegate ()
                        {
                            nowProcessID += 1;
                            processing = false;
                        }));
                        // nowProcessID += 1;
                        // processing = false;
                        break;
                    case 1:
                        if (!pac.scucess)
                            nowProcessID += 1;
                        else
                        {
                            processing = true;
                            StartCoroutine(PartA(delegate ()
                            {
                                nowProcessID += 1;
                                processing = false;
                            }));
                        }
                        break;
                    case 2:
                        if (!pbc.scucess)
                            nowProcessID += 1;
                        else
                        {
                            processing = true;
                            StartCoroutine(PartB(delegate ()
                            {
                                nowProcessID += 1;
                                processing = false;
                            }));
                        }
                        // nowProcessID += 1;
                        // processing = false;
                        break;
                    case 3:
                        if (!pcc.scucess)
                            nowProcessID += 1;
                        else
                        {


                            processing = true;
                            StartCoroutine(PartC(delegate ()
                            {
                                nowProcessID += 1;
                                processing = false;
                            }));
                        }
                        // nowProcessID += 1;
                        //processing = false;
                        break;
                    default:
                        flag = true;
                        break;

                }
            }

            if (flag) break;
            yield return null;
            
        }


        
        moudleOK.SetActive(true);
        yield return new WaitForSeconds(0.23f);

        if (GetDriverPermisson.nowDevice != "")
        {
            StartCoroutine(SavePaper());
        }
        else
        {
            uploadHandle.gradeObj.SetActive(true);
            uploadHandle.gradeText.text = "因音频设备未设置,该部分仅为演示不录音,点击返回!";
        }
       
       
        
       


     

        /*****                           已下代码暂时废弃           *****/


        // for (int i = 0; i < PA.Count; i++) PaperManager.SaveFile(rootPath + "PA_DD" + i + ".wav", PA[i]); 


        /*
        int allProcess = 0;
        List<DoProcessData> tempProces = new List<DoProcessData>();
        uploadHandle.infoText.text = "正在准备...队列:" + allProcess;
        if (pac.scucess)
        {
            PaperManager.SaveFile(rootPath + "PA.wav", PA);//A
            tempProces.Add(new DoProcessData() { path = rootPath + "PA.wav", time = Mathf.RoundToInt(PA.length), descript = "PartA模仿朗读", sourceText = classPaper.parta_text });
            allProcess += 1;
            uploadHandle.infoText.text = "正在准备...队列:" + allProcess;
            uploadHandle.logText.text += "缓存音频:PA";
        }*/

        /* 废弃
        cvgt.GetVoiceText(rootPath + "PA.wav",Mathf.RoundToInt(PA.length),
            delegate(bool ok,string msg) {
                if (ok)
                {
                    
                   // Debug.Log("成功.");
                    KeyValuePair<int,int> re= cvgt.CountGrade(msg, classPaper.parta_text);
                    GlobalUIManager.guim.CreateNewDialogBox("A得分"+re.Key+"/"+re.Value);
                    AOK = true;
                }
                else
                {
                    // Debug.Log("失败.");
                    GlobalUIManager.guim.CreateNewDialogBox("PartA评卷出现错误...");
                   
                }
        });
        */
        //cvg.CheakPartA(PA, rootPath + "PA.wav", classPaper.parta_text, 0.5f,100);

        /*if (pbc.scucess)
       {

           string[] strsBQ = classPaper.partb_keyword_question.Split('/');
           for (int i = 0; i < PB_P1.Count; i++)
           {
               PaperManager.SaveFile(rootPath + "PB_1A" + i + ".wav", PB_P1[i]);
               allProcess += 1;
               tempProces.Add(new DoProcessData() { path = rootPath + "PB_1A" + i + ".wav", time = Mathf.RoundToInt(PB_P1[i].length), descript = "PartB三问题号" + i, sourceText = strsBQ[i] });
               uploadHandle.infoText.text = "正在准备...队列:" + allProcess;
               uploadHandle.logText.text += "\n缓存音频:" + "PB_1A" + i;
           }
    }*/
        /* if (pbc.scucess)
         {
             string[] strsBA = classPaper.partb_keyword_anser.Split('/');
             for (int i = 0; i < PB_P2.Count; i++)
             {
                 PaperManager.SaveFile(rootPath + "PB_2A" + i + ".wav", PB_P2[i]);
                 allProcess += 1;
                 tempProces.Add(new DoProcessData() { path = rootPath + "PB_2A" + i + ".wav", time = Mathf.RoundToInt(PB_P2[i].length), descript = "PartB五答题号" + i, sourceText = strsBA[i] });
                 uploadHandle.infoText.text = "正在准备...队列:" + allProcess;
                 uploadHandle.logText.text += "\n缓存音频:" + "PB_2A" + i;
             }
    }*/
        /* if (pcc.scucess)
        {
           
            // for (int i = 0; i < PC.Count; i++) PaperManager.SaveFile(rootPath + "PC_DD" + i + ".wav", PC[i]);
            PaperManager.SaveFile(rootPath + "PC.wav", PC);
            uploadHandle.logText.text += "缓存音频:PC";

            allProcess += 1;
            tempProces.Add(new DoProcessData() { path = rootPath + "PC.wav", time = Mathf.RoundToInt(PC.length), descript = "PartC复述", sourceText = classPaper.partc_keyword_story });
        
    }*/
        /*
            uploadHandle.infoText.text = "正在准备...队列:" + allProcess;
        uploadHandle.logText.text += "开始评分...";
        uploadHandle.infoText.text = "评分中...";

        Debug.Log("文件转换完成!");
        int now = 0;
        bool nextFlag = true;
        bool hasError = false;
        int errorCount = 0;
        int maxRetry = 3;
        while (true)
        {
            if (nextFlag)
            {
                if (now == allProcess) break;//中断..
                nextFlag = false;
                cvgt.GetVoiceText(tempProces[now].path, tempProces[now].time,
               delegate (bool ok, string msg)
               {
                   if (ok)
                   {

                   // Debug.Log("成功.");
                   KeyValuePair<int, int> re = cvgt.CountGrade(msg, tempProces[now].sourceText);
                   // GlobalUIManager.guim.CreateNewDialogBox("A得分" + re.Key + "/" + re.Value);
                   tempProces[now].my = re.Key;
                       tempProces[now].max = re.Value;
                       nextFlag = true;//进行下一个
                   now += 1;
                       errorCount = 0;

                       uploadHandle.logText.text += "评分完成:"+tempProces[now].descript;
                       float fillA = (float)now / (float)allProcess;
                       uploadHandle.process.fillAmount = fillA;
                       uploadHandle.processText.text = (fillA * 100).ToString() + "%";
                   }
                   else
                   {
                       errorCount += 1;
                       if (errorCount<=maxRetry)
                       {
                           nextFlag = true;//自动重试

                           return;
                       }
                   // Debug.Log("失败.");
                   GlobalUIManager.guim.CreateNewSelectBox(tempProces[now].descript+"评卷出现错误.\n可能是请求过多批卷服务器无法处理.\n是否重试?若否则回到大厅.",delegate(bool okr) {

                       if (okr)
                       {
                           nextFlag = true;
                       }
                       else
                       {
                           UnityEngine.SceneManagement.SceneManager.LoadScene(1);//回到大厅
                       }
                   });
                       hasError = true;

                   }
               });

            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("评卷成功!");
        int dpdAll = 0,dpdMy=0;

        for(int i = 0; i < tempProces.Count; i++)
        {
           DoProcessData dpd= tempProces[i];
            dpdAll += dpd.max;
            dpdMy += dpd.my;
            uploadHandle.gradeText.text += dpd.descript+"-我的:"+dpd.my+"/总分:"+ dpd.max+ "\n";
        }
        uploadHandle.gradeText.text += "总结:我的:"+dpdMy+"/总分:"+dpdAll;
        uploadHandle.gradeObj.SetActive(true);
        */

    }
    CELSTTempPaper tempPaper = new CELSTTempPaper();
    string tempPath = "";

    IEnumerator SavePaper()
    {
        tempPaper.id = classPaper.id;
        uploadHandle.infoText.text = "正在处理录音数据...";
        string rootPath = classPaper.path + "/Memory";
         PaperManager.CreatePath(rootPath);
        string fileName="TempPaperCELSTWhen"+System.DateTime.Now.ToString("TyyyyMMddhhmmss")+"Random"+Random.Range(-23333,23333)+".lhirin";
        tempPath = rootPath + "/" + fileName;


        string partA_b64 = "";
        if (pac.scucess)
        {
            uploadHandle.infoText.text = "正在处理-PartA";
            yield return new WaitForEndOfFrame();
            partA_b64 = CheakerTools.Base64_Encode(WavUtility.FromAudioClip(PA));
            tempPaper.partA = partA_b64;
        }

        string partBA_b64 = "";
        string partBB_b64 = "";
        if (pbc.scucess)
        {
            for (int i = 0; i < PB_P1.Count; i++)
            {
                uploadHandle.infoText.text = "正在处理-PartB问题" + (i + 1).ToString();
                yield return new WaitForEndOfFrame();
                partBA_b64 += CheakerTools.Base64_Encode(WavUtility.FromAudioClip(PB_P1[i]));
                if (i != PB_P1.Count - 1) partBA_b64 += "|";
            }
            tempPaper.partBA = partBA_b64;


            for (int i = 0; i < PB_P2.Count; i++)
            {
                uploadHandle.infoText.text = "正在处理-PartB回答" + (i + 1).ToString();
                yield return new WaitForEndOfFrame();
                partBB_b64 += CheakerTools.Base64_Encode(WavUtility.FromAudioClip(PB_P2[i]));
                if (i != PB_P2.Count - 1) partBB_b64 += "|";
            }
            tempPaper.partBB = partBB_b64;
        }
        string partC_b64 = "";
        if (pcc.scucess)
        {
            uploadHandle.infoText.text = "正在处理-PartC";
            yield return new WaitForEndOfFrame();
            partC_b64 = CheakerTools.Base64_Encode(WavUtility.FromAudioClip(PA));
            tempPaper.partC = partC_b64;
        }

        tempPaper.isUpload = false;


        uploadHandle.infoText.text = "正在创建缓存...";
        yield return new WaitForEndOfFrame();
        PaperManager.SaveFile(tempPath, JsonUtility.ToJson(tempPaper));
        UserMemoryManager.InsertIntoList(new UserMemoryList.Common {dataPath=tempPath,
            time =System.DateTime.Now.ToString(),
            type =UserMemoryList.MemoryType.GD_CELST });
        uploadHandle.infoText.text = "缓存创建完毕!";

        GlobalUIManager.guim.CreateNewSelectBox("缓存完毕,是否上传音频呢?\n如果需要机器改卷或他人协助必须上传到服务器.", delegate (bool ok) {

            if (ok)
            {
                StartCoroutine(CheakPaper());
            }
            else
            {
                uploadHandle.gradeObj.SetActive(true);
                uploadHandle.gradeText.text = "用户取消上传!请点击关闭回到大厅进行自评或进行其他操作!";
            }

        });
        //PlayerPrefs.SetString("tempPaper", JsonUtility.ToJson(tempPaper));
        //PlayerPrefs.Save();
    }


   IEnumerator CheakPaper()
    {

        yield return new WaitForEndOfFrame();

        uploadHandle.infoText.text = "正在上传...";
        yield return new WaitForEndOfFrame();

        
        StartCoroutine(UploadVoiceHandle.Uploader(classPaper.id,tempPaper.partA, tempPaper.partBA, tempPaper.partBB, tempPaper.partC,
           delegate (ulong size, float process) {
               uploadHandle.process.fillAmount = process;

               uploadHandle.processText.text =
               "(" + GetFileSize(size) + "/" + GetFileSize((ulong)(float.Parse(size.ToString()) / process)) + ")" + Mathf.CeilToInt(process * 100).ToString() + "%";

           },
          OnUploadDone

           ));
    }

    void OnUploadDone(UnityEngine.Networking.UnityWebRequest uwr)
    {
        if(uwr.isHttpError || uwr.isNetworkError)
        {
            GlobalUIManager.guim.CreateNewSelectBox("上传失败!是否重试?",delegate(bool isR) {
                if (isR)
                {
                    StartCoroutine(CheakPaper());
                    
                }
                else
                {

                    uploadHandle.gradeObj.SetActive(true);
                    uploadHandle.gradeText.text = "文件上传失败!";
                    
                }
                
            });
        }
        else
        {
           

            tempPaper.isUpload = true;
            PaperManager.SaveFile(tempPath, JsonUtility.ToJson(tempPaper));
            // PlayerPrefs.DeleteKey("tempPaper");
            //PlayerPrefs.Save();
            uploadHandle.gradeObj.SetActive(true);
            uploadHandle.gradeText.text = "文件上传成功!请点击关闭回到大厅->成绩处进行改卷!";
            Debug.Log(uwr.downloadHandler.text);
            
        }
    }

    /// <summary>
    /// 开始宣读
    /// </summary>
    /// <returns></returns>
    IEnumerator StartXD(System.Action ok)
    {
        moudleStart.SetActive(true);
        yield return new WaitForSeconds(1.23f);

        yield return new WaitForSeconds(6f);
        SceneAudioCtrl.sac.PlayAudio("XuanDu");//播放宣读音频
        yield return new WaitForSeconds(60f);
        Destroy(moudleStart);

        ok?.Invoke();
    }
   /// <summary>
   /// PartA宣读
   /// </summary>
   /// <returns></returns>
    IEnumerator PartA(System.Action ok)
    {
        PA = null;
        moudelPartA.SetActive(true);
        yield return new WaitForSeconds(1.23f);
        SceneAudioCtrl.sac.PlayAudio("PartA_XuanDu");//PartA题目宣读音频
        yield return new WaitForSeconds(12f);

        pac.PlayVideo(false);//播放视频
        yield return new WaitForSeconds((float)pac.GetAudioTime());
        pac.HideAll();
        pac.StopVideo();//停止
        pac.textP1.SetActive(true);//P1开始
        SceneAudioCtrl.sac.PlayAudio("PartA_LianXi1");//P1音频
        yield return new WaitForSeconds(5f);
        pac.HideAll();
        pac.textYuanWen.SetActive(true);//原文练习1分钟
        pac.RunJiShiQi(60f);//计时器
        yield return new WaitForSeconds(60f);
        pac.HideAll();
        pac.textP2.SetActive(true);
        SceneAudioCtrl.sac.PlayAudio("PartA_LianXi2");//P2音频
        yield return new WaitForSeconds(9f);
        pac.HideAll();
        pac.textYuanWen.SetActive(true);
        pac.PlayVideo(false, false);//播放不显示图像视频
        pac.RunJiShiQi((float)pac.GetAudioTime());//计时器
        yield return new WaitForSeconds((float)pac.GetAudioTime());
        pac.StopVideo();//停止
        pac.HideAll();
        pac.textLuYin.SetActive(true);
        SceneAudioCtrl.sac.PlayAudio("PartA_LuYin");//录音开始提示
        yield return new WaitForSeconds(5f);
        SceneAudioCtrl.sac.PlayAudio("Biu");//哔
        yield return new WaitForSeconds(2f);
        float rcTime = (float)pac.GetAudioTime();
        pac.PlayVideo(true);//播放视频 
        GlobalAudioRecorder.gar.NewRecord(rcTime, true, false, delegate (AudioClip ac) { PA = ac; });
        yield return new WaitForSeconds(rcTime);
        pac.StopVideo();//停止视频
        pac.StopAllCoroutines();
        Destroy(moudelPartA);
        ok?.Invoke();
    }

    IEnumerator PartB(System.Action ok)
    {
        PB_P1.Clear();
        PB_P2.Clear();
        moudelPartB.SetActive(true);
        yield return new WaitForSeconds(1.23f);
        SceneAudioCtrl.sac.PlayAudio("PartB_XuanDu");//PartB宣读
        yield return new WaitForSeconds(23f);//等待
        pbc.RestAll();
        pbc.jieShao.SetActive(true);

        yield return new WaitForSeconds(27f);

        pbc.RestAll();
        pbc.shortJiShiQi.SetActive(true);
        pbc.RunShortTimer(3f);//3秒

        yield return new WaitForSeconds(3f);//倒计时timer
        pbc.RestAll();
        
       float t= pbc.PlayerVideo();//播放视频
        yield return new WaitForSeconds(t);
        pbc.RestAll();

        pbc.P1_A.SetActive(true);//题目A
        yield return new WaitForSeconds(1.23f);
        SceneAudioCtrl.sac.PlayAudio("PartB_Ask");//题目
        yield return new WaitForSeconds(15f);

        pbc.RestAll();
        pbc.P1_B.SetActive(true);
        int paid = 0;
        while (true)
        {
            
       float[] fls=    pbc.PlayTQuestion(paid);
            if (fls == null) break;
            pbc.longJiShiQi.SetActive(true);
            pbc.RunLongTimer(fls[0]);
            yield return new WaitForSeconds(fls[0]-1f);
            SceneAudioCtrl.sac.PlayAudio("Biu");//哔
            yield return new WaitForSeconds(1f);
            //录音开始...
            GlobalAudioRecorder.gar.NewRecord(fls[1], true,true, delegate (AudioClip ac) { PB_P1.Add(ac); });
            yield return new WaitForSeconds(fls[1]);
            //录音完成
            yield return new WaitForSeconds(fls[2]);
            //下几个
            paid += 1;
        }


        pbc.RestAll();
        pbc.P2_A.SetActive(true);//题目B
        yield return new WaitForSeconds(1.23f);
        SceneAudioCtrl.sac.PlayAudio("PartB_Anser");//题目
        yield return new WaitForSeconds(15f);
        pbc.RestAll();
        pbc.P2_B.SetActive(true);
        int pbid = 0;
        while (true)
        {

            float fl = pbc.PlayFAnser(pbid);
            if (fl == 0f) break;
            
            

            yield return new WaitForSeconds(fl);
            pbc.longJiShiQi.SetActive(true);
            pbc.RunLongTimer(10f);
            yield return new WaitForSeconds(9f);
            SceneAudioCtrl.sac.PlayAudio("Biu");//哔
            yield return new WaitForSeconds(1f);
            //录音开始...
            GlobalAudioRecorder.gar.NewRecord(10f, true, true, delegate (AudioClip ac) { PB_P2.Add(ac); });
            yield return new WaitForSeconds(10f);
            //录音完成
           
            //下几个
            pbid += 1;
        }
        pbc.StopAllCoroutines();
        Destroy(moudelPartB);
        ok?.Invoke();
    }

    IEnumerator PartC(System.Action ok)
    {
        PC = null;
        moudelPartC.SetActive(true);
        pcc.RestAll();
        pcc.xuanDu.SetActive(true);
        yield return new WaitForSeconds(1.23f);
        SceneAudioCtrl.sac.PlayAudio("PartC_XuanDu");
        yield return new WaitForSeconds(12.3f);
        pcc.RestAll();
        pcc.gengGai.SetActive(true);
        pcc.jishiQi.SetActive(true);
        pcc.RunTimer(15f);
        yield return new WaitForSeconds(15f);


        float tk = pcc.PlayStoryAudio();

        yield return new WaitForSeconds(tk);
        pcc.RestAll();
        pcc.retellText.SetActive(true);
        yield return new WaitForSeconds(1.23f);
        SceneAudioCtrl.sac.PlayAudio("PartC_Retell");
        yield return new WaitForSeconds(10f);
        SceneAudioCtrl.sac.PlayAudio("Biu");
        yield return new WaitForSeconds(1f);
        //录音开始
        GlobalAudioRecorder.gar.NewRecord(120f, true, true, delegate (AudioClip ac) { PC = ac; });
        yield return new WaitForSeconds(120f);
        //录音结束
        pcc.StopAllCoroutines();
        Destroy(moudelPartC);
        ok?.Invoke();

    }

    /*测试代码
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneAudioCtrl.sac.audioSource.clip = allAudioClips[0].Value;
            SceneAudioCtrl.sac.audioSource.Play();
        }
    }
    */
    /// <summary>
    /// 取得文件大小
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string GetFileSize(ulong num)
    {

        num = num * 100 / (1024 * 1024);
        float numa = float.Parse(num.ToString()) / 100f;
        return numa + "MB";

    }


    

   

    class DoProcessData
    {
        public string path;
        public string sourceText = "";
        public int time;
        
        public string descript = "进程描述";
        public int my=0;
        public int max = 0;
    }
}