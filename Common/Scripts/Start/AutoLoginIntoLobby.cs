using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoLoginIntoLobby : MonoBehaviour
{
    public static AutoLoginInfoClass data=new AutoLoginInfoClass();

    public static bool connectFail = false, connectOK = false;

    public GetPermisson getPermisson;
    public GetDriverPermisson driverPermisson;
    public LoginToKaoShi login;


    private void Start()
    {
        if (!PlayerPrefs.HasKey("AutoLoadInfo"))
        {
            data = new AutoLoginInfoClass();
        }
        else
        {
            GlobalUIManager.guim.CreateNewSelectBox("是否依据上次登录信息自动登录?",delegate(bool ok) {

                if (ok) StartCoroutine(AutoLoad());

            });
        }
    }

   
    IEnumerator AutoLoad()
    {
        connectFail = false;
        connectOK = false;
        data = JsonUtility.FromJson<AutoLoginInfoClass>(PlayerPrefs.GetString("AutoLoadInfo"));
        getPermisson.Connect();
        while (true)
        {

            if (connectFail)
            {

                GlobalUIManager.guim.CreateNewSelectBox("自动登录失败,是否重试?", delegate (bool ok)
                {

                    if (ok) StartCoroutine(AutoLoad());


                });
                StopAllCoroutines();
                break;
            }

            if (connectOK) break;

            yield return new WaitForEndOfFrame();

        }
        try
        {

            driverPermisson.SelectMicroPhone(data.microPhoneName);
            driverPermisson.SelectOK();
        }
        catch
        {
            GlobalUIManager.guim.CreateNewDialogBox("音频输入设备校验失败!请手动操作!");
            StopAllCoroutines();
        }
        login.ipID.SetTextWithoutNotify(data.id);
        login.ipPWD.SetTextWithoutNotify(data.pwd);
        login.GO();
    }

    public static void Save()
    {
        if (GetDriverPermisson.nowDevice == "")
        {
            GlobalUIManager.guim.CreateNewDialogTie("您的登录配置因为您未选择音频输入设备故没有保存！");
            return;
        }
        PlayerPrefs.SetString("AutoLoadInfo",JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public class AutoLoginInfoClass
    {
        //public string serverAddress = "";
        public string microPhoneName = "";
        public string id = "";
        public string pwd = "";
    }
}
