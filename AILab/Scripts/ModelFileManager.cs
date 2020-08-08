using System;


namespace LuoHaoLab
{
    [Obsolete("废弃")]
    public class ModelFileManager
    {
        public const float PI =(float)Math.PI;
        public const int sampleRate = 16000;//采样率
        public const int frameLength = 512;//帧长度
        public const int framePreRatio = 2;//预留=帧长度/此
        public  const float preRmphasisRatio= 0.97f;//预加重系数,0.90-1.00,一般取0.97
        public const float hammingWindowRatio = 0.46f;//汉明窗系数,一般取0.46
        /// <summary>
        /// 预加重(使音频变平坦)
        /// </summary>
        /// <param name="audiosData"></param>
        /// <returns></returns>
        public static float[] PreEmphasis(float[] audiosData)
        {
            float[] returnData =new float[audiosData.Length];//返回

           for(int i=0; i < audiosData.Length; i++)
            {
                float z = audiosData[i];
               if(z>0) returnData[i] = 1 - (preRmphasisRatio/z);
                if (z == 0) returnData[i] = 0;
               if (z<0) returnData[i] = -(1 - (preRmphasisRatio / -z));//滤波器
            }
            return returnData;
        }
        
        /// <summary>
        /// 获取帧并加汉明窗
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        public static Frame[] CutAudioToFrames(float[] audio)
        {
            int frameCount = audio.Length/frameLength;
            if (audio.Length % frameLength != 0) frameCount += 1;//计算帧数


            Frame[] frames = new Frame[frameCount];

            for(int i = 0; i < frameCount; i++)
            {
                float[] nowFrameValues = new float[frameLength];//这一帧的音频
                int startIndex = i * frameLength;//开始
                int endIndex = ((i+1) * frameLength)-1;//终点
                if (endIndex >= audio.Length) endIndex = audio.Length - 1;
               // Array.Copy(audio, startIndex, nowFrameValues, 0, endIndex - startIndex+1);//copyTo
                for(int ij = 0; ij <= endIndex-startIndex; ij++)
                {
                    nowFrameValues[ij] = audio[ij+startIndex]
                        *(1f - hammingWindowRatio) 
                        -(hammingWindowRatio * Cos(PI * 2f * ij / (frameLength - 1)));
                }
                frames[i] = new Frame();
                frames[i].points = nowFrameValues;
            }

            
            return frames;
            
        } 


        /// <summary>
        /// Cos函数
        /// </summary>
        /// <param name="ola"></param>
        /// <returns></returns>
        private static float Cos(float ola)
        {
           return (float)Math.Cos(ola);
        }


    }
}