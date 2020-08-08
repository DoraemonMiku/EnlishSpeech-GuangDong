using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDistance : MonoBehaviour
{
    public RectTransform rootRect;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("LeftDis"))
        {
            SetLeft(PlayerPrefs.GetFloat("LeftDis"));
        }
        if (PlayerPrefs.HasKey("RightDis"))
        {
            SetRight(PlayerPrefs.GetFloat("RightDis"));
            
        }
    }


    public void SetLeft(float num)
    {
        Vector2 v2 = rootRect.offsetMin;
        v2.x = num;
        rootRect.offsetMin = v2;
    }

    public void SetRight(float num)
    {
        Vector2 v2 = rootRect.offsetMax;
        v2.x = num;
        rootRect.offsetMax = v2;
    }
}
