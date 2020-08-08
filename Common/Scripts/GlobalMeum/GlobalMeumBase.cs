using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMeumBase : MonoBehaviour
{
    public void QuitAPP()
    {
        GlobalUIManager.guim.CreateNewSelectBox(
            "确定退出系统嘛?\n若正在考试你的<Color=\"Red\">进度不会保留</Color>!",
            delegate (bool ok) {if(ok) Application.Quit(); });
        //Application.Quit();

    }
}
