namespace LuoHaoLab
{
    /// <summary>
    /// 字词模型
    /// </summary>
    public class WordsModels
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string modelsName = "女声";
        /// <summary>
        /// 版本
        /// </summary>
        public string version = "1";
        /// <summary>
        /// 帧长度
        /// </summary>
        public int frameLength = 512;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="modelsName"></param>
        /// <param name="version"></param>
        /// <param name="frameLength"></param>
        public WordsModels(string modelsName,string version,int frameLength=512)
        {
            this.modelsName = modelsName;
            this.version = version;
            this.frameLength = frameLength;
        }

    }

    /// <summary>
    /// 单一单词模型
    /// </summary>
    public class MonoWordModel
    {
        /// <summary>
        /// 过零次数
        /// </summary>
        public int crossZeroCount=0;

        /// <summary>
        /// 帧列表
        /// </summary>
        public Frame[] frames;
       

    }
    /// <summary>
    /// 帧
    /// </summary>
    public class Frame
    {
        public float[] points;
    }
}
