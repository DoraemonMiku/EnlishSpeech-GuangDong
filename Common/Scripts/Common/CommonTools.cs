using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工具类
/// </summary>
public class CommonTools : MonoBehaviour
{
    /// <summary>
    /// 清空物体子对象
    /// </summary>
    /// <param name="gm"></param>
    public static void ClearObjectChilds(GameObject gm)
    {
        for (int i = 0; i < gm.transform.childCount; i++)
            Destroy(gm.transform.GetChild(i).gameObject);

    }
    /// <summary>
    /// 实例化一个物体
    /// </summary>
    /// <param name="gm"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject NewAnObjectA(GameObject gm,Transform parent)
    {
       GameObject newObj= Instantiate(gm, parent);
        newObj.transform.localPosition = Vector3.zero;
        return newObj;
    }

    /// <summary>
    /// 秒输出整数时间
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    public static string SecondsToMinutes(float sec)
    {
         string a = Mathf.FloorToInt(sec / 60f).ToString();
        string b = Mathf.RoundToInt(sec % 60f).ToString();
        if (b.Length == 1) b = "0" + b;

        return a + ":"+b;
    }
}
