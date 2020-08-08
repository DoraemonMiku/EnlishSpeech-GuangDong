using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;

public class GetPermisson : MonoBehaviour
{
    [Header("支持的服务器版本号")]
    public string supportVersion = "RrringVersion:test";
    [Header("输入框")]
    public GameObject inputArea;
    [Header("重试的模块")]
    public GameObject reloadMoudle;
    [Header("连接成功的组件")]
    public GameObject okConnect;
    [Header("输入组件")]
    public InputField addressIF, keyIF;
    
    public Toggle useHttpsToggle;
    [Header("输出的对象")]
    public GameObject output;
    /// <summary>
    /// 使用Https
    /// </summary>
    public static bool useHttps
    {
        get
        {
            if (!PlayerPrefs.HasKey("UseHttps"))
                return false;

            int type= PlayerPrefs.GetInt("UseHttps"); 
            return type == 1;

        }
    }

    public LineCtrl lineCtrl;

    private void Awake()
    {
#if UNITY_EDITOR
        PaperManager.filePath= Application.dataPath + "/../Papers/";
#endif
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//禁止灭屏
                                                      //   Time.timeScale = 5f;
        lineCtrl.SetFill(1, 3);
    }
    /// <summary>
    /// 取得服务器地址
    /// </summary>
    public static string GetServerAddress
    {
        get
        {
            if (!PlayerPrefs.HasKey("ServerAddress")) return null;
            if(useHttps)
            return "https://"+PlayerPrefs.GetString("ServerAddress")+ "/Service";
            else
                return "http://" + PlayerPrefs.GetString("ServerAddress")+"/Service";
        }
    }

    /// <summary>
    /// 取得服务器秘钥
    /// </summary>
    public static string GetServerKey
    {
        get
        {
            if (!PlayerPrefs.HasKey("ServerKey")) return null;
            return PlayerPrefs.GetString("ServerKey");
        }
    }

    /// <summary>
    /// 获取权限
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator GetPermissonFromServer(System.Action<PermissionActionCallback> callback)
    {

        WWWForm form = new WWWForm();
        form.AddField("version", supportVersion);
        form.AddField("key", GetServerKey);
        UnityWebRequest uwr = UnityWebRequest.Post(GetServerAddress+ "/ClientPermission/RegisterClient.php", form);
       
        yield return uwr.SendWebRequest();
        PermissionActionCallback pac = new PermissionActionCallback();
        if (uwr.error == "" || uwr.error == null)
        {
            try
            {
                pac.pwc = JsonUtility.FromJson<PermissionWebCallback>(uwr.downloadHandler.text);
                pac.isOK = true;
            }
            catch
            {
                pac.isOK = false;
                pac.msg = "数据包解析失败!";
            }

        }
        else
        {
            pac.isOK = false;
            pac.msg = "与服务器通讯失败!"+uwr.error;
        }
        callback.Invoke(pac);
    }
    
    /// <summary>
    /// 更改服务器信息
    /// </summary>
    public void ChangeServerInfo()
    {
        PlayerPrefs.SetString("ServerAddress",addressIF.text);
        PlayerPrefs.SetString("ServerKey", keyIF.text);
        if(useHttpsToggle.isOn)
        PlayerPrefs.SetInt("UseHttps",1);
        else
            PlayerPrefs.SetInt("UseHttps",0);
        PlayerPrefs.Save();
        inputArea.SetActive(false);
    }

    /// <summary>
    /// 连接
    /// </summary>
    public void Connect()
    {
        //    reloadMoudle.SetActive(false);
        output.SetActive(true);
        output.GetComponentInChildren<Text>().text = "";
       output.GetComponentInChildren<Text>().DOText("正在连接到服务器...",1.23f);
        
        StartCoroutine(GetPermissonFromServer(ConnectCallBack));
    }
    /// <summary>
    /// 连接回调
    /// </summary>
    /// <param name="pac"></param>
    public void ConnectCallBack(PermissionActionCallback pac)
    {
        output.SetActive(false);
        if (pac.isOK)
        {
            switch (pac.pwc.code)
            {
                case 0:
                    // GlobalUIManager.guim.CreateNewDialogBox("服务器连接成功!");
                    okConnect.SetActive(true);
                    reloadMoudle.SetActive(false);
                    lineCtrl.SetFill(2, 3);
                    AutoLoginIntoLobby.connectOK = true;
                    break;
                default:
                    AutoLoginIntoLobby.connectFail = true;
                    DisplayError(pac.pwc.msg);
                    break;

            }
        }
        else
        {
            DisplayError(pac.msg);
        }
    }
    /// <summary>
    /// 显示错误
    /// </summary>
    /// <param name="message"></param>
    public void DisplayError(string message)
    {

        GlobalUIManager.guim.CreateNewDialogBox(message);
        reloadMoudle.SetActive(true);
    }




    private void Start()
    {
        addressIF.text = PlayerPrefs.GetString("ServerAddress");
        keyIF.text = GetServerKey;
        useHttpsToggle.isOn = useHttps;
        if (GetServerAddress != null)
        {
           
            //Connect();
        }
        else 
        {
            addressIF.text = "106.15.200.140:81";
            keyIF.text = "test";
            inputArea.SetActive(true);
            GlobalUIManager.guim.CreateNewDialogBox("已填好默认服务器地址.\n可直接点击更改继续.\n正式版本1.0.6");
           // GlobalUIManager.guim.CreateNewDialogBox("全屏菜单功能暂时屏蔽.");
        }
    }
   
    
}
/// <summary>
/// 行为回调
/// </summary>
public class PermissionActionCallback
{
    public bool isOK = false;
    public string msg = "";
    public PermissionWebCallback pwc; 
}
/// <summary>
/// Web的回复
/// </summary>
public class PermissionWebCallback
{
    public int code;
    public string msg;
}
