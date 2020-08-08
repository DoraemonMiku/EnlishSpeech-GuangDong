using System;
using System.Collections.Generic;

namespace AILH.HandWrite
{
    public class HandWriteShiBie 
    {

        
    }

    /// <summary>
    /// 手写模型类
    /// </summary>
    [Serializable]
    public class HandWriteModelClass
    {
        public List<string> targetText = new List<string>();
        public List<List<int>> targetExpOutput = new List<List<int>>();
    }
}