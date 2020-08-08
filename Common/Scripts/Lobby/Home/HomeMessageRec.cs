using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HomeMessageRec : MonoBehaviour
{
    public static HomeMessageRec HMR;
    //Url:  
    public Transform targetContent;
    public GameObject buttonObjs;



    public Text titleText,bodyText;
    public GameObject anMsg;
    public GameObject buttonLink;
    public GameObject buttonTip;

    private void Awake()
    {
        HMR = this;
    }
    [HideInInspector]
    public MessageFormat tempMsgFormat;
    [HideInInspector]
    public UserMessageClass.Data tempUMCD;
    public void SetMSGBox(MessageFormat mf,UserMessageClass.Data umcd)
    {
        anMsg.SetActive(true);
        tempMsgFormat = mf;
        tempUMCD = umcd;
        titleText.text = mf.title;
        bodyText.text = mf.messageBody+"\n<Color=Orange>Time:"+umcd.time+"</Color>";
        switch (mf.type)
        {
            case MessageFormat.MessageType.Text:
                buttonLink.SetActive(false);
                buttonTip.SetActive(true);
                break;
            case MessageFormat.MessageType.Link:
                buttonLink.SetActive(true);
                buttonTip.SetActive(false);
                break;
            default:
                GlobalUIManager.guim.CreateNewDialogBox("出现了未知错误!");
                break;
        }
    }

    public void HideMSGBox()
    {
        anMsg.SetActive(false);
        tempMsgFormat = null;
    }

    public void OnClick()
    {
        StopAllCoroutines();
        StartCoroutine(GetMyMessage());
    }
    public void CleanContent()
    {
        for(int i = 0; i < targetContent.childCount; i++)
        {
            Destroy(targetContent.GetChild(i).gameObject);
        }
    }
    IEnumerator GetMyMessage()
    {

       DialogLoading dl= GlobalUIManager.guim.CreateNewLoading();
        string url = GetPermisson.GetServerAddress
            +"/Home/GetHomeMessage.php?token="
            +LoginToKaoShi.userLoginCallback.data.token;
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        CleanContent();
        if (uwr.error == "" || uwr.error == null)
        {

            try
            {
              UserMessageClass umc=  JsonUtility.FromJson<UserMessageClass>(uwr.downloadHandler.text);
                
                if (umc.code == 0)
                {
                    //开始读取
                    for(int i = 0; i < umc.data.Length; i++)
                    {
                        
                        UserMessageClass.Data data = umc.data[i];
                       // Debug.Log(data.message);
                        try
                        {
                            MessageFormat mf = JsonUtility.FromJson<MessageFormat>(data.message);
                            GameObject gm = Instantiate(buttonObjs,targetContent);
                            HomeMessageItemCtrl hmic = gm.GetComponent<HomeMessageItemCtrl>();
                         
                            hmic.mf = mf;
                            hmic.umcData = data;
                            switch(mf.type)
                            {
                                case MessageFormat.MessageType.Text:

                                    if (data.isRead)
                                    {
                                        hmic.iconImg.color = Color.white;
                                        hmic.SetItem(1);
                                    }
                                    else
                                    {
                                        hmic.iconImg.color = Color.yellow;
                                        hmic.SetItem(0);
                                    }

                                    hmic.SetText(mf.title);

                                    break;

                                case MessageFormat.MessageType.Link:
                                    if (data.isRead)
                                    {
                                        hmic.iconImg.color = Color.white;
                                        hmic.SetItem(3);
                                    }
                                    else
                                    {
                                        hmic.iconImg.color = Color.yellow;
                                        hmic.SetItem(2);
                                    }

                                    hmic.SetText(mf.title);
                                    break;
                                default:
                                    Destroy(gm);
                                    GlobalUIManager.guim.CreateNewDialogBox("消息类型(Type)识别错误.");
                                    break;
                            }
                        }
                        catch(System.Exception err)
                        {
                            GlobalUIManager.guim.CreateNewDialogBox("消息类型解析失败");
                            Debug.Log(err.StackTrace);
                           // Debug.Log(JsonUtility.ToJson( new MessageFormat() { type=0,messageBody="111"}));
                        }


                    }




                }
                else
                {
                    GlobalUIManager.guim.CreateNewDialogBox(umc.msg);
                }

            }
            catch(System.Exception err)
            {
                GlobalUIManager.guim.CreateNewDialogBox("解析消息数据包失败!");
                Debug.Log(uwr.downloadHandler.text);
                Debug.Log(err);
            }
           
        }
        dl.DestoryThisLoad();
    }

    [System.Serializable]
    /// <summary>
    /// 站内信格式
    /// </summary>
    public class MessageFormat // {type:0,messageBody:"测试内容啦啦啦" }
    {
        [System.Serializable]
        public enum MessageType
        {
            /// <summary>
            /// 文本
            /// </summary>
            Text,
            /// <summary>
            /// 链接
            /// </summary>
            Link
        }

        public MessageType type;
        public string title;
        public string messageBody;
    }

    [System.Serializable]
    /// <summary>
    /// 用户站内信
    /// </summary>
    public class UserMessageClass
    {
        public int code;
        public string msg;
        public Data[] data;
        [System.Serializable]
        public class Data
        {
            public string id;
            public bool isRead;
            public string senderID;
            public string targetID;
            public string time;
            public string message;
        }
    }
}


