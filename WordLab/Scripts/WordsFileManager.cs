using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsFileManager : MonoBehaviour
{

}
[System.Serializable]
public class WordListClass
{
    /// <summary>
    /// 描述
    /// </summary>
    public string decsripts;
    /// <summary>
    /// 单词
    /// </summary>
    public List<AnWord> words = new List<AnWord>();

    [System.Serializable]
    public class AnWord
    {
        /// <summary>
        /// 单词
        /// </summary>
        public string word = "";
        /// <summary>
        /// 中文
        /// </summary>
        public string[] chinese;
    }
}