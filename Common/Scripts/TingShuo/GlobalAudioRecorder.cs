using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class GlobalAudioRecorder : MonoBehaviour
{

    public static GlobalAudioRecorder gar;

    public GameObject upTextObj, downTextObj,levelObj;


    public Text upText, downText;
    public Image level;


    private void Awake()
    {
        gar = this;
    }

    public bool NewRecord(float time,bool isShowLevel,bool useDownText, System.Action<AudioClip> ok)
    {
     


        
        Text targetText;
        GameObject targetObj;
        if (useDownText)
        {
            targetText = downText;
            targetObj = downTextObj;
        }
        else
        {
            targetText = upText;
            targetObj = upTextObj;
        }


        if (GetDriverPermisson.nowDevice == "")
        {
            GlobalUIManager.guim.CreateNewDialogTie("因为没有设置音频设备,目前仅计时不录音~");
            StartCoroutine(TextTimer(targetObj, targetText, time));
            return true;
        }

        List<string> strs = new List<string>(Microphone.devices);
        if (!strs.Contains(GetDriverPermisson.nowDevice)) return false;

        StartCoroutine(RCTimer(time, delegate (AudioClip ac) { ok?.Invoke(ac); }));
        if (isShowLevel) StartCoroutine(LevelChange(time));
        StartCoroutine(TextTimer(targetObj, targetText, time));
        return true;
    }
    /*
    public bool NewRecord(float time,float qgTime, bool isShowLevel, bool useDownText, System.Action<List<AudioClip>> ok)
    {
        List<string> strs = new List<string>(Microphone.devices);
        if (!strs.Contains(GetDriverPermisson.nowDevice)) return false;
        StartCoroutine(RCTimer(time,qgTime, delegate (List<AudioClip> acs) { ok?.Invoke(acs); }));
        Text targetText;
        GameObject targetObj;
        if (useDownText)
        {
            targetText = downText;
            targetObj = downTextObj;
        }
        else
        {
            targetText = upText;
            targetObj = upTextObj;
        }
        if (isShowLevel) StartCoroutine(LevelChange(time));
        StartCoroutine(TextTimer(targetObj, targetText, time));
        return true;
    }
    */
    AudioClip nowRecord;

    IEnumerator RCTimer(float eltTime,System.Action<AudioClip> ok)
    {
        nowRecord = null;
        nowRecord= Microphone.Start(GetDriverPermisson.nowDevice, false,Mathf.FloorToInt(eltTime), 16000); 
        yield return new WaitForSeconds(eltTime);
        Microphone.End(GetDriverPermisson.nowDevice);
        ok?.Invoke(nowRecord);
     
    }
    /*
    List<AudioClip> nowRecordList = new List<AudioClip>();
    IEnumerator RCTimer(float eltTime,float qgTime, System.Action<List<AudioClip>> ok)
    {
        nowRecord = null;
        nowRecordList.Clear();
        float tempTime = eltTime;
        while (true)
        {
            if (tempTime>qgTime)
            {
                nowRecord = Microphone.Start(GetDriverPermisson.nowDevice, false, Mathf.FloorToInt(qgTime), 16000);
                yield return new WaitForSeconds(qgTime);
                Microphone.End(GetDriverPermisson.nowDevice);
                nowRecordList.Add(nowRecord);
                tempTime -= qgTime;
                nowRecord = null;
            }
            else
            {
                nowRecord = Microphone.Start(GetDriverPermisson.nowDevice, false, Mathf.FloorToInt(eltTime), 16000);
                yield return new WaitForSeconds(eltTime);
                Microphone.End(GetDriverPermisson.nowDevice);
                
                nowRecordList.Add(nowRecord);
                nowRecord = null;
                break;
            }
        }
        ok?.Invoke(nowRecordList);
    }*/

    IEnumerator TextTimer(GameObject gm,Text text,float elt)
    {
        gm.SetActive(true);
        float time = Time.time;
        while (true)
        {
            float nowTime = Time.time - time;
            text.text = "录音中:" + Mathf.CeilToInt(elt - nowTime).ToString();
            if (nowTime >= elt)
            {
                gm.SetActive(false);
                text.text = "";
                break;
            }
            yield return null;
        }
    }

    IEnumerator LevelChange(float elt)
    {
        levelObj.SetActive(true);
        float time = Time.time;
        while (true)
        {
            float nowTime = Time.time - time;
            
            float exmpleX = 0f;
            float[] exmple =new float[23];
            int offest = 0;
           if (nowRecord!=null) 
                offest = Microphone.GetPosition(GetDriverPermisson.nowDevice)-23+1;
            if (offest > 0)
            {
                nowRecord.GetData(exmple, offest);
                for (int i = 0; i < exmple.Length; i++) exmpleX += exmple[i];
                exmpleX=(exmpleX/ exmple.Length)*2.3f; 
            }
            else
            {
                exmpleX = 0f;
            }
            level.DOFillAmount(exmpleX,0.23f);
            if (nowTime >= elt)
            {
                levelObj.SetActive(false);
                break;
            }
            yield return null;
        }
       
    }
/*
    public static byte[] ConvertClipToBytes(AudioClip clip)
    {
        //clip.length;
        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        byte[] bytesData = new byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = new byte[2];
            byteArr = System.BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
    }
    */

}
