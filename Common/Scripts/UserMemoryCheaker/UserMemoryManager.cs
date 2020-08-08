using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserMemoryManager : MonoBehaviour
{
    public static readonly string perfabsName = "UserMemorys";
    /// <summary>
    /// 读取列表
    /// </summary>
    /// <returns></returns>
    public static UserMemoryList ReadList()
    {
        if (PlayerPrefs.HasKey(perfabsName))
        {
            return JsonUtility.FromJson<UserMemoryList>(PlayerPrefs.GetString(perfabsName));
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 写入列表
    /// </summary>
    /// <param name="uml"></param>
    public static void WriteList(UserMemoryList uml)
    {
        PlayerPrefs.SetString(perfabsName, JsonUtility.ToJson(uml));
        PlayerPrefs.Save();
    }
    /// <summary>
    /// 插入一个数据
    /// </summary>
    /// <param name="uml"></param>
    public static void InsertIntoList(UserMemoryList.Common umlc)
    {
        UserMemoryList source = ReadList();
        if (source == null)
        {
            source = new UserMemoryList();
        }
        source.allMemorys.Add(umlc);
        WriteList(source);
    }
    /// <summary>
    /// 删除一个数据
    /// </summary>
    /// <param name="index"></param>
    public static void DeleteOneData(int index)
    {
        UserMemoryList source = ReadList();
        if (File.Exists(source.allMemorys[index].dataPath)) File.Delete(source.allMemorys[index].dataPath);
        source.allMemorys.RemoveAt(index);
        WriteList(source);
    }
}

