using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSetDis : MonoBehaviour
{
    public Slider sliderL, sliderR;
    public SetDistance sd;
    private void Awake()
    {
        
        sliderL.maxValue = Mathf.RoundToInt(Screen.width / 4f);
        sliderR.maxValue = Mathf.RoundToInt(Screen.width / 4f);
        if (PlayerPrefs.HasKey("LeftDis"))
        {
          sliderL.value= PlayerPrefs.GetFloat("LeftDis");
        }
        if (PlayerPrefs.HasKey("RightDis"))
        {
            sliderR.value=-PlayerPrefs.GetFloat("RightDis");

        }
    }

    public void SetPrefabLeft()
    {
        PlayerPrefs.SetFloat("LeftDis", sliderL.value);
        sd.SetLeft(sliderL.value);
        OnDone();
    }
    public void SetPrefabRight()
    {
        PlayerPrefs.SetFloat("RightDis", -sliderR.value);
        sd.SetRight(-sliderR.value);
        OnDone();
    }
    public void OnDone()
    {
        PlayerPrefs.Save();
    }
}
