using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaiDuAsr;
using System.IO;

public class CheakVoiceGradeTemp : MonoBehaviour
{
    public int oneLenth = 50;


   public void GetVoiceText(string path,int time, System.Action<bool, string> result)
    {

        /*
        for (int i = 0; i < paths.Count; i++)
        {

            CheakVoice(paths[i].Key, paths[i].Value,GetVoiceCallback);
            
        }
        */

        CheakVoice(path, time, result);
    }

 

    public KeyValuePair<int,int> CountGrade(string voiceT,string sourceT)
    {
        string[] strsV = voiceT.Split(' ');
        string[] strsS = sourceT.Split('.', ' ',',');
        List<string> listV = new List<string>();
        List<string> listS = new List<string>();
        for(int i = 0; i < strsV.Length; i++)
        {
            string str = strsV[i].Trim(' ').ToLower();
            if (!string.IsNullOrEmpty(str)) listV.Add(str);
        }

        for (int i = 0; i < strsS.Length; i++)
        {
            string str = strsS[i].Trim('-', ' ').ToLower();
            if (!string.IsNullOrEmpty(str)) listS.Add(str);
        }
        int grade =0;
        int all = listS.Count;
        for(int i = 0; i < listV.Count; i++)
        {
            string str = strsV[i];
            if (listS.Contains(str))
            {
                grade += 1;
                listS.RemoveAt(listS.IndexOf(str));
            }
        }
        return new KeyValuePair<int, int>(grade,all);
    }




    #region 取得音频...
    static int nowId = 0;
    Dictionary<List<int>, System.Action<bool, string>> tempActions = new Dictionary<List<int>, System.Action<bool, string>>();

    List<List<int>> classes = new List<List<int>>();

    private bool CheakVoice(string path, int length,System.Action<bool,string> result)
    {
        try
        {
            if (!File.Exists(path)) return false;
            List<int> myIds = new List<int>();
            byte[] audioA = File.ReadAllBytes(path);
            if (length <= oneLenth)
            {

                StartCoroutine(BaiDuAsrBaseTools.GetMessageFromAudio(audioA, AsrCallBack, nowId));
                myIds.Add(nowId);

                classes.Add(myIds);
                tempActions.Add(myIds, result);
                nowId += 1;
                return true;
            }
            int dl = length / oneLenth;
            //int edl = length % oneLenth;
            for(int i = 0; i < dl+1; i++)
            {
                byte[] bytei = null ;
                string err="";
                bool ok=WavScissorHelper.GetWavFileScissor(path,path+".ep",i*oneLenth,Mathf.Clamp((i+1) * oneLenth,0,length),ref err,ref bytei);
                if (ok)
                {
                    StartCoroutine(BaiDuAsrBaseTools.GetMessageFromAudio(bytei, AsrCallBack, nowId));
                    myIds.Add(nowId);
                    
                    nowId += 1;
                }
                else 
                {
                    GlobalUIManager.guim.CreateNewDialogBox("处理音频时出错!请截图联系开发者!");
                    break;
                }
            }

            classes.Add(myIds);
            tempActions.Add(myIds, result);
            return true;
        }
        catch (System.Exception err)
        {
            GlobalUIManager.guim.CreateNewDialogBox("处理音频时出错!请截图联系开发者!\n"+err.Message);
            return false;
        }
        // WavScissorHelper.GetWavFileScissor();
        //   StartCoroutine(BaiDuAsrBaseTools.GetMessageFromAudio())
    }

    Dictionary<int, string> resultsStr = new Dictionary<int, string>();
    List<List<int>> failInts = new List<List<int>>();
    private void AsrCallBack(bool ok,int mid,ClassBaiDuAsrCallback cbac)
    {
        if (ok)
        {
            string tempStr = "";
            for(int i = 0; i < cbac.result.Length; i++)
            {
                tempStr += cbac.result[i]+" ";
            }
            resultsStr.Add(mid, tempStr);
           List<int> order =FindOrderList(mid);
            string all = "";
            bool reFlag = false;
            for(int i = 0; i < order.Count; i++)
            {
                if (!resultsStr.ContainsKey(order[i]))
                {
                    reFlag = true;
                    return;
                }
                all += resultsStr[order[i]];
            }
            if (reFlag) return;
            tempActions[order]?.Invoke(true,all);
        }
        else
        {
            List<int> order = FindOrderList(mid);
            if (failInts.Contains(order)) return;
            failInts.Add(order);
            tempActions[order]?.Invoke(false, "语音服务器异常!请稍后再试...");
         
            //  GlobalUIManager.guim.CreateNewDialogBox("评卷服务器连接失败!");
        }
    }

    private List<int> FindOrderList(int mid)
    {
        for(int i = 0; i < classes.Count; i++)
        {
            List<int> kint = classes[i];


            if (kint.Contains(mid))
                return kint;
           
                
        }
        return null;
    }
    #endregion
}
