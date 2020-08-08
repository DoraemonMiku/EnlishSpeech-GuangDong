using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMeumCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Image preLoadImg;
    float pressTime = 0f;
    float preLoadTime = 0.23f;
    float goingTime = 2.3f;
    bool isMeumOpen = false;
    bool isPreloadImgShow = false;
    // Update is called once per frame
    void Update()
    {
    
        if (isMeumOpen) return;//如果已经打开

        if (Input.GetMouseButton(0))
        {
            pressTime +=Time.deltaTime;
            if (pressTime > goingTime)
            {
                pressTime = goingTime;
            }

        }
        else
        {
            pressTime = 0f;
            if (isPreloadImgShow)
            {
                preLoadImg.gameObject.SetActive(false);
                isPreloadImgShow = false;

            }
        }
        if (pressTime >= preLoadTime)
        {
            if (!isPreloadImgShow)
            {
                preLoadImg.gameObject.SetActive(true);
                isPreloadImgShow = true;
            }
            preLoadImg.fillAmount = (pressTime - preLoadTime) / (goingTime - preLoadTime);
        }
        ///判断
        if (pressTime == goingTime)
        {
            if (!isMeumOpen)
            {
                ShowMeum();
                
            }

        }
       
    }
    public GameObject meumObj;
    void ShowMeum()
    {
       /// Handheld.Vibrate();
        meumObj.SetActive(true);
        preLoadImg.gameObject.SetActive(false);
        isPreloadImgShow = false;
        isMeumOpen = true;
        preLoadImg.fillAmount = 0f;
    }
    public void CloseMeum()
    {
        meumObj.SetActive(false);
        isMeumOpen = false;
        //preLoadImg.gameObject.SetActive(false);
    }
}
