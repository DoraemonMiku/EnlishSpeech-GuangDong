using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CleanTemp : MonoBehaviour
{
    [MenuItem("MyTools/Clean")]
    static void Clean() {
       
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("预设缓存清空完毕!");

    }

    [MenuItem("MyTools/Test")]
    static void Cut()
    {
        string[] strs = Selection.assetGUIDs;

        string path = AssetDatabase.GUIDToAssetPath(strs[0]);
        string errInfo = "";
        byte[] data=null;
        WavScissorHelper.GetWavFileScissor(path,path+".wav",0,100,ref errInfo,ref data);
        Debug.LogError(errInfo);
        
    }
}
