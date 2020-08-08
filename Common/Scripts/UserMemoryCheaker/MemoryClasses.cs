using System;
using System.Collections.Generic;

[Serializable]
public class UserMemoryList
{
   
    public List<Common> allMemorys=new List<Common>();
    [Serializable]
    public class Common
    {
        public MemoryType type = MemoryType.GD_CELST;
        public string time = "";
        public string dataPath="";

    }
    public enum MemoryType
    {
        /// <summary>
        /// 广东高考
        /// </summary>
        GD_CELST,
        /// <summary>
        /// 大学四六级
        /// </summary>
        CETB4B6
    }
}