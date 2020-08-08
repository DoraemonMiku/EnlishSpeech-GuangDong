using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MeumButtonItem : MonoBehaviour
{
    public Button button;
    public Image icon,selectPic;
    public Text desText;
    public GameObject targetObj;

    Color clickedColor;
    private void Start()
    {
        clickedColor = button.colors.selectedColor;
    }
    /// <summary>
    /// 设置颜色
    /// </summary>
    public void SetSelect()
    {
        
        ColorBlock copy = button.colors;
        copy.normalColor =clickedColor;
        button.colors = copy;
        selectPic.gameObject.SetActive(true);
    }
    /// <summary>
    /// 重置颜色
    /// </summary>
    public void ClearColor()
    {
        ColorBlock copy = button.colors;
        copy.normalColor = new Color(1f,1f,1f,0f);
        button.colors = copy;
        selectPic.gameObject.SetActive(false);

    }
}
