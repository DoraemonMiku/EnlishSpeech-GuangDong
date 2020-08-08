using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperCheakerCELSTFinish :CheakerSceneManager.CheakingBehavior
{

    public GameObject finshObj;

    public override void Close()
    {
        finshObj.SetActive(false);
    }

    public override void Open()
    {
        finshObj.SetActive(true);
    }

    public void Back()
    {
        GlobalUIManager.guim.CreateNewSelectBox("确定退出自评系统?",delegate(bool ok) {

            if (ok)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
        });
       
    }
}
