using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using BaiDuAsr;
using System.Text.RegularExpressions;

[System.Obsolete("这个项目存在问题,姑且被弃用.")]
public class CheakVoiceGrade : MonoBehaviour
{
    //  List<string> PAallWord = new List<string>();

    // public string TestT = "Who often took part in the lectures on Chinese tea culture";
    private void Awake()
    {
        // KeyValuePair<List<string>,List<string>>  strs= GetListFromTwoText("At the heart of being human is our culture, and something that goes hand in hand with human culture is our ability to co-operate. But co-operation in the chimp world is a fragile thing. Chimps will co-operate, but only for selfish ends.Human children did something that no other ape will do.In that small act of sharing, they reveal something that  really does lie at the heart of what it is to be human. It's  a tiny but profound difference between us and the other apes, and it's a way of thinking that underpins our ability to co-operate and create human culture."
        //   , "At the of being goes hand in hand with human culture is our ability to co-operate JSWA HelloWorld OKK Here legends");//16 OK
        //    Debug.Log(strs.Value.Count);
        //  Debug.Log( CheakPartA(Resources.Load<AudioClip>("PA")
        //     , @"D:\UnityProgram\EnglishLern\Papers\1EP\TEMP\20200131074042\PA.wav", ProcessCtrl.classPaper.parta_text,0.5f,100));
        //Debug.Log(PAError);

    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            KeyValuePair<int, int> ka= GetMyGradeFromString("***Who *often *(took part in)=join=attend=(participate in) the lectures[lecture] on Chinese *tea *culture", TestT);
            Debug.Log(ka.Value + "/" + ka.Key);
        }
    }*/
    #region PartA
    /// <summary>
    /// 改卷_PartA
    /// </summary>
    /// <param name="audioClip">录音</param>
    /// <param name="sourceText">原文</param>
    /// <param name="examLevel">考试难度</param>
    public bool CheakPartA(AudioClip ac, string path, string sourceText, float examLevel, int maxGrade,System.Action<CheakExamRespondClass> onOK, System.Action<string> onError)
    {
        try
        {
            if (!File.Exists(path)) return false;
            if (ac.length >= 120f) return false;
            PAsourceText = sourceText;
            PALevel = examLevel;
            PAmaxGrade = maxGrade;
            okA = true;
            tempAStr = "";

            if (ac.length > 60f)
            {
                bool ok = true;
                string error = "";
                byte[] bytesA = null;
                byte[] bytesB = null;

                processACount = 2;


                ok = WavScissorHelper.GetWavFileScissor(path, path + ".tempA", 0, Mathf.FloorToInt(ac.length / 2f), ref error, ref bytesA);
                ok = WavScissorHelper.GetWavFileScissor(path, path + ".tempB", Mathf.FloorToInt(ac.length / 2f), Mathf.FloorToInt(ac.length), ref error, ref bytesB);
                if (!ok)
                {
                    Debug.LogError(error);
                    GlobalUIManager.guim.CreateNewDialogBox("PartA改卷时出现异常!\nError:" + error);

                }
                else
                {
                   //err StartCoroutine(BaiDuAsrBaseTools.GetMessageFromAudio(bytesA, PartAMsgCallback, "PartA1"));
                   //err StartCoroutine(BaiDuAsrBaseTools.GetMessageFromAudio(bytesB, PartAMsgCallback, "PartA2"));
                    StartCoroutine(PAJianKong(onOK,onError));
                }
                //

                return ok;
            }
            else
            {
                processACount = 1;

              //err  StartCoroutine(BaiDuAsrBaseTools.GetMessageFromAudio(File.ReadAllBytes(path), PartAMsgCallback, "PartA"));
                StartCoroutine(PAJianKong(onOK, onError));
                return true;
            }
        }
        catch (System.Exception e)
        {
            PAError = "抓取错误:" + e.Message + "\n" + e.StackTrace;
            return false;
        }
    }

    string PAsourceText;
    float PALevel;
    int PAmaxGrade = 0;
    string tempAStr = "";
    int processACount = 0;
    bool okA = true;
    string PAError = "";
    /// <summary>
    /// 改卷_PartA文本请求回调方法
    /// </summary>
    /// <param name="ok"></param>
    /// <param name="cbac"></param>
    private void PartAMsgCallback(bool ok, string mid, ClassBaiDuAsrCallback cbac)
    {
        if (!ok)
        {
            if (cbac == null)
                PAError = "网络错误!";
            else
                PAError = cbac.err_msg;
            okA = false;

            return;
        }

        string str = "";
        for (int i = 0; i < cbac.result.Length; i++)
        {
            str += cbac.result[i] + " ";

        }
        str = str.Substring(0, str.Length - 1);
        if (tempAStr != "") tempAStr += " ";

        tempAStr += str;
        processACount -= 1;
        //  KeyValuePair<List<string>, List<string>> CODE= GetListFromTwoText(PAsourceText,str);
    }
    /// <summary>
    /// 改卷监控
    /// </summary>
    /// <returns></returns>
    private IEnumerator PAJianKong(System.Action<CheakExamRespondClass> onOK, System.Action<string> onError)
    {
        while (true)
        {
            if (!okA)
            {

                onError?.Invoke("PartA改卷时出现异常!\nError:" + PAError);

                break;
            }
            if (processACount == 0)
            {
                KeyValuePair<List<string>, List<string>> CODE = GetListFromTwoText(PAsourceText, tempAStr);
                int grade = GetGrade(CODE.Value.Count, CODE.Key.Count, PALevel, PAmaxGrade);
                CheakExamRespondClass CERC = new CheakExamRespondClass() {
                    mid="PartA",
                    keywordText = PAsourceText,
                    level = PALevel,
                    maxGrade = PAmaxGrade,
                    voiceText = tempAStr,
                    myGrade=grade

                };
                onOK?.Invoke(CERC);
                //GlobalUIManager.guim.CreateNewDialogBox("PartA得分:" + grade + "/" + PAmaxGrade);

                break;
            }
            yield return null;
        }
    }


    #endregion
    
    /// <summary>
    /// 改卷partB
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sourceText"></param>
    /// <param name="examLevel"></param>
    /// <param name="maxGrade"></param>
    /// <returns></returns>
    public bool CheakPartB(List<string> paths, List<string> sourceTexts, float examLevel, int maxGrade, System.Action<List<CheakExamRespondClass>> onOK, System.Action<string> onError)
    {
        //检查文件状态
        for (int i = 0; i < paths.Count; i++)
        {
            if (!File.Exists(paths[i])) return false;
        }

        try
        {
            for (int i = 0; i < paths.Count; i++)
            {
               //err StartCoroutine(BaiDuAsrBaseTools.GetMessageFromAudio(File.ReadAllBytes(paths[i]), PartBMsgCallback, paths[i]));
            }
            return true;

        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
            return false;
        }
    }


    
    string PBError = "";
    bool okB = true;
    int processBCount = 0;
    Dictionary<string,CheakExamRespondClass> okayFiles = new Dictionary<string, CheakExamRespondClass>();

    private void PartBMsgCallback(bool ok, string mid, ClassBaiDuAsrCallback cbac)
    {
        if (!ok)
        {
            if (cbac == null)
                PBError = "网络错误!";
            else
                PBError = cbac.err_msg;
            okB = false;

            return;
        }

        string str = "";
        string tempB = "";
        for (int i = 0; i < cbac.result.Length; i++)
        {
            str += cbac.result[i] + " ";

        }
        str = str.Substring(0, str.Length - 1);
        if (tempB != "") tempB += " ";


        //  KeyValuePair<List<string>, List<string>> CODE= GetListFromTwoText(PAsourceText,str);
    }


    private IEnumerator PartBJianKong()
    {
        while (true)
        {

            yield return null;
        }
    }


    /// <summary>
    /// 取得关键词和等级
    /// </summary>
    /// <param name="sourceText"></param>
    /// <returns></returns>
    private SentenceClass GetLevels(string sourceText)
    {
        SentenceClass sc = new SentenceClass();
        List<SentenceKeywordClass> levels = new List<SentenceKeywordClass>();


        Dictionary<string, string> xkhValue = new Dictionary<string, string>();
        ReadXiaoKuoHao(sourceText, ref xkhValue, ref sourceText);//处理小括号
        sc.XKHValue = xkhValue;
        //  Debug.Log(sourceText);
        Dictionary<string, string> zkhValue = new Dictionary<string, string>();
        ReadZhongKuoHao(sourceText, ref zkhValue, ref sourceText);//处理中括号
        sc.ZKHValues = zkhValue;
        //   Debug.Log(sourceText);
        string[] strs = sourceText.ToLower().Split(' ');//切割字符串 Qia!!!

        //List<string> onceStrList = new List<string>();//一级缓冲池

        for (int i = 0; i < strs.Length; i++)
        {
            string str = strs[i];//获取源
            if (string.IsNullOrEmpty(str.Trim())) continue;//舍去异常的源
            string[] oneStrs = str.Split('=');//分割源
            if (oneStrs.Length == 0) continue;

            SentenceKeywordClass skc = new SentenceKeywordClass();
            skc.words = new List<string>(oneStrs);
            string reT = "";
            skc.sorce = GetStarCount(oneStrs[0], ref reT) + 1;
            //  Debug.Log(reT);
            skc.words[0] = reT;



            levels.Add(skc);

            // onceStrList.Add(str);//载入一级缓冲池

        }
        sc.sentenceKeywordClasses = levels;
        //一级缓冲池填装完毕!
        return sc;
    }

    /// <summary>
    /// 取得成绩(Key为总,Value为自己的成绩)
    /// </summary>
    /// <param name="sourceText"></param>
    /// <param name="voiceText"></param>
    /// <returns></returns>
    private KeyValuePair<int, int> GetMyGradeFromString(string sourceText, string voiceText)
    {
        sourceText = sourceText.ToLower();
        voiceText = voiceText.ToLower();

        SentenceClass sc = GetLevels(sourceText);

        //替换小括号
        List<string> strs = new List<string>(sc.XKHValue.Keys);
        for (int i = 0; i < strs.Count; i++)
        {
            string str = sc.XKHValue[strs[i]];
            voiceText = voiceText.Replace(str + " ", strs[i] + " ");
        }
        //  Debug.Log(voiceText);
        //替换中括号
        List<string> strs1 = new List<string>(sc.ZKHValues.Keys);
        for (int i = 0; i < strs1.Count; i++)
        {
            string str = sc.ZKHValues[strs1[i]];
            voiceText = voiceText.Replace(str + " ", strs1[i] + " ");

        }
        // Debug.Log(voiceText);


        List<string> vTs = new List<string>(voiceText.Split(' '));


        List<SentenceKeywordClass> sentenceKeywordClasses = sc.sentenceKeywordClasses;
        int all = 0;
        int my = 0;
        for (int i = 0; i < sentenceKeywordClasses.Count; i++)
        {

            SentenceKeywordClass skc = sentenceKeywordClasses[i];
            all += skc.sorce;
            for (int j = 0; j < skc.words.Count; j++)
            {
                //识别
                string str = skc.words[j];
                /*
               int start= voiceText.IndexOf(str);
                int end = voiceText.IndexOf(" ",start) ;
                if (end == -1) end = voiceText.Length - 1;
                if (start == -1  || end < start) continue;
                if ((end - start) == str.Length)
                    my += skc.sorce;
                else
                    my +=Mathf.Clamp(skc.sorce - 1,0,10);
                voiceText = voiceText.Remove(start, end-start);
              */
                //KeyValuePair<int, string> word = skc.words[j];
                //if(j==0)all+=
                // Debug.Log("信息:" + str);
                if (vTs.Contains(str))
                {
                    my += skc.sorce;
                    vTs.RemoveAt(vTs.IndexOf(str));
                    //Debug.Log("存在" + str);
                    break;
                }

                for (int k = 0; k < vTs.Count; k++)
                {
                    if (vTs[k].Contains(str))
                    {
                        my += Mathf.Clamp(skc.sorce - 1, 0, 10);
                        vTs.RemoveAt(i);
                        Debug.Log("半存在" + str);
                        break;
                    }
                }

            }
        }
        return new KeyValuePair<int, int>(all, my);
    }



    /// <summary>
    /// 取得星级
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private int GetStarCount(string str, ref string okstr)
    {
        okstr = str.Replace("*", "");
        if (str.Contains("*"))
        {
            return str.Length - okstr.Length;
        }
        else
            return 0;

    }

    /// <summary>
    /// 读取小括号
    /// </summary>
    /// <param name="targetStr"></param>
    /// <param name="strs"></param>
    /// <param name="okTargetText"></param>
    private void ReadXiaoKuoHao(string targetStr, ref Dictionary<string, string> strs, ref string okTargetText)
    {
        Dictionary<string, string> tempXKHvalue = new Dictionary<string, string>();

        while (true)
        {
            if (targetStr == null) break;
            int start = targetStr.IndexOf('(');
            int end = targetStr.IndexOf(')');
            if (start == -1) break;
            if (end == -1) break;
            if (end < start) break;

            string str = targetStr.Substring(start + 1, end - start - 1);
            string newStr = "{$$" + tempXKHvalue.Count.ToString() + "}";
            if (end != targetStr.Length - 1)

                targetStr = targetStr.Substring(0, start) + newStr + targetStr.Substring(end + 1);

            else

                targetStr = targetStr.Substring(0, start) + newStr;

            //  Debug.Log("括号内容" + str);
            tempXKHvalue.Add(newStr, str);
        }
        okTargetText = targetStr;

        strs = tempXKHvalue;
    }

    /// <summary>
    /// 读取中括号
    /// </summary>
    /// <param name="targetStr"></param>
    /// <param name="strs"></param>
    /// <param name="okTargetText"></param>
    private void ReadZhongKuoHao(string targetStr, ref Dictionary<string, string> strs, ref string okTargetText)
    {
        Dictionary<string, string> tempZKHvalue = new Dictionary<string, string>();

        while (true)
        {
            if (targetStr == null) break;
            int start = targetStr.IndexOf('[');
            int end = targetStr.IndexOf(']');
            if (start == -1) break;
            if (end == -1) break;
            if (end < start) break;

            int lastStart = targetStr.LastIndexOf(' ', start);
            if (lastStart == -1) lastStart = 0;
            int lastEnd = start - 1;
            string str = targetStr.Substring(lastStart + 1, lastEnd - lastStart);
            //Debug.Log(str);
            string newStr = "{$" + tempZKHvalue.Count.ToString() + "}";
            if (end != targetStr.Length - 1)

                targetStr = targetStr.Substring(0, lastStart + 1) + newStr + targetStr.Substring(end + 1);

            else

                targetStr = targetStr.Substring(0, lastStart + 1) + newStr;


            tempZKHvalue.Add(newStr, str);
        }
        okTargetText = targetStr;
        Debug.LogError(targetStr);
        strs = tempZKHvalue;
    }
    //***Who *often **(took part in)=join=attend=(participate in) the *lectures[lecture] on Chinese *tea *culture/***Did you **invite *other *people *too=also/***Which lecture[lectures] **impressed[impress] you *most






    /// <summary>
    /// 获取词组列表（key为原文,value为匹配中的词数）
    /// </summary>
    /// <param name="sourceText"></param>
    /// <param name="voiceText"></param>
    /// <returns></returns>
    private KeyValuePair<List<string>, List<string>> GetListFromTwoText(string sourceText, string voiceText)
    {

        //去除标点
        /*sourceText=Regex.Replace(sourceText,
            "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]",
            " ");*/
        // sourceText = sourceText.Trim(',','.','。');

        //切割字符串
        string[] strs = sourceText.Split(' ', ',', '.', '，', '。');

        List<string> strList = new List<string>();
        Dictionary<string, int> dicStrCount = new Dictionary<string, int>();//用于计数
        for (int i = 0; i < strs.Length; i++)
        {
            string str = strs[i].Trim().ToLower().Trim('-');
            if (str != "")
            {
                strList.Add(str);
                if (dicStrCount.ContainsKey(str))//如果存在
                {
                    dicStrCount[str] += 1;
                }
                else
                {
                    dicStrCount.Add(str, 1);
                }
            }
        }
        List<string> vstrList = new List<string>(voiceText.Split(' '));
        List<string> matchList = new List<string>();
        for (int i = 0; i < vstrList.Count; i++)
        {
            string str = vstrList[i].Trim().ToLower().Trim('-');
            if (dicStrCount.ContainsKey(str))
            {
                matchList.Add(str);
                dicStrCount[str] -= 1;
                if (dicStrCount[str] == 0) dicStrCount.Remove(str);
            }
        }
        return new KeyValuePair<List<string>, List<string>>(strList, matchList);
    }

    /// <summary>
    /// 取得成绩
    /// </summary>
    /// <param name="my">我的</param>
    /// <param name="total">总的</param>
    /// <param name="level">等级</param>
    /// <returns></returns>
    private int GetGrade(int my, int total, float level, int maxGrade)
    {
        float rawGrade = (float)my / (float)total;

        if (rawGrade >= level)
        {
            return maxGrade;
        }
        else
        {
            return Mathf.RoundToInt((float)maxGrade * (rawGrade / level));
        }
    }

}
/// <summary>
/// 橘子的类
/// </summary>
public class SentenceClass
{
    public List<SentenceKeywordClass> sentenceKeywordClasses = new List<SentenceKeywordClass>();
    public Dictionary<string, string> ZKHValues = new Dictionary<string, string>();
    public Dictionary<string, string> XKHValue = new Dictionary<string, string>();
}
/// <summary>
/// 橘子关键词的类
/// </summary>
public class SentenceKeywordClass
{
    /// <summary>
    /// 级别
    /// </summary>
    public int sorce = 0;
    /// <summary>
    /// 单词列表
    /// </summary>
    public List<string> words = new List<string>();

}
/// <summary>
/// 回调的类
/// </summary>
public class CheakExamRespondClass
{
    /// <summary>
    /// 身份ID
    /// </summary>
    public string mid = "";
    /// <summary>
    /// 关键词文本
    /// </summary>
    public string keywordText = "";
    /// <summary>
    /// 声音读取出来的文本
    /// </summary>
    public string voiceText = "";
    /// <summary>
    /// 我的分数
    /// </summary>
    public int myGrade = 0;
    /// <summary>
    /// 满分
    /// </summary>
    public int maxGrade = 0;
    /// <summary>
    /// 级别
    /// </summary>
    public float level = 0.23f;
}