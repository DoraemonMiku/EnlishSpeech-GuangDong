using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace BaiDuAsr
{
    public class BaiDuAsrBaseTools : MonoBehaviour
    {
        const string baiduSpeechAPI = "https://vop.baidu.com/server_api";
        /*
        public AudioClip ac;
        private void Awake()
        {
           // StartCoroutine(GetMessageFromAudio("24.9697e51849ad1ce9e5fa2406dc5ef55c.2592000.1582830922.282335-18311308",WavUtility.FromAudioClip(ac)));  
        }*/
        public static IEnumerator GetMessageFromAudio( byte[] voice, System.Action<bool,int,ClassBaiDuAsrCallback> callBack,int mid=0)
        {
            UnityWebRequest uwr = UnityWebRequest.Put(
                baiduSpeechAPI + "?dev_pid=1737&cuid=User_" +
                LoginToKaoShi.userLoginCallback.data.id +
                "&token=" +
                LoginToKaoShi.userLoginCallback.data.asr_token,
                voice);
            uwr.SetRequestHeader("Content-Type", "audio/wav;rate=16000");
            uwr.SendWebRequest();
            while (true)
            {
                //  Debug.Log(uwr.uploadProgress);
                yield return null;
                if (uwr.isDone)
                {
                    break;
                }
            }
            if (uwr.error == "" || uwr.error == null)
            {
                try
                {

                    ClassBaiDuAsrCallback cbac = JsonUtility.FromJson<ClassBaiDuAsrCallback>(uwr.downloadHandler.text);
                    Debug.Log(uwr.downloadHandler.text+ LoginToKaoShi.userLoginCallback.data.asr_token);

                    switch (cbac.err_no)
                    {
                        case 0:
                            callBack?.Invoke(true,mid, cbac);
                            break;
                        case 3301:
                            cbac.result = new string[1] {""};
                            callBack?.Invoke(true, mid, cbac);
                            break;
                        case 3307:
                            cbac.result = new string[1] { "" };
                            callBack?.Invoke(true, mid, cbac);
                            break;
                        default:
                            callBack?.Invoke(false, mid, cbac);
                            break;
                    }
                   

                        

                }
                catch
                {
                    callBack?.Invoke(false, mid, null);
                    Debug.LogError("未知错误!");
                }
            }
            else
            {
                callBack?.Invoke(false, mid, null);
                Debug.LogError("网络错误!");
            }
            // Debug.Log(uwr.downloadHandler.text);
        }


    }
    /// <summary>
    /// 百度Asr回调类
    /// </summary>
    public class ClassBaiDuAsrCallback
    {
        public string corpus_no;
        public string err_msg;
        public int err_no;
        public string[] result;
        public string sn;


        public string mid = "";
        
    }

}