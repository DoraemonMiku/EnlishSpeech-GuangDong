using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeumButtonsManager : MonoBehaviour
{

    public MeumButtonItem startRun;
    // Start is called before the first frame update
    void   Start()
    {
       
        startRun.button.onClick.Invoke();
        //Debug.Log("run");
    }


    public void OnAnItemClick(MeumButtonItem mbi)
    {
        MeumButtonItem[] mbis =GameObject.FindObjectsOfType<MeumButtonItem>();
        for(int i = 0; i < mbis.Length; i++)
        {
            mbis[i].ClearColor();
            if(mbis[i].targetObj!=null)mbis[i].targetObj.SetActive(false);
            ///Debug.Log("aaa");
        }

        mbi.SetSelect();
        if (mbi.targetObj != null) mbi.targetObj.SetActive(true);
    }
}
