using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;

namespace LuoHaoLab
{
    public class PingCeBeta
    {
        /// <summary>
        /// 频率比重
        /// </summary>
        private const float freRatio = 0.7f;
        /// <summary>
        /// 等级比重
        /// </summary>
        private const float levelRatio = 0.1f;
        /// <summary>
        /// 能量比重
        /// </summary>
        private const float powerRatio = 0.2f;


        /// <summary>
        /// 取得成绩
        /// </summary>
        /// <param name="source"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int GetGrade(float[] source,float[] user)
        {

           float[] sourceAudioInfo= GetDataInfo(source);
            float[] userAudioInfo = GetDataInfo(user);
            float freLerp = Mathf.Log( Mathf.Abs(userAudioInfo[0] - sourceAudioInfo[0])*10e5f);
            float levelLerp= Mathf.Abs(userAudioInfo[1] - sourceAudioInfo[1])*10e5f;
            float powerLerp= Mathf.Abs(userAudioInfo[2] - sourceAudioInfo[2])*10e5f;

            float lerpJi = freLerp /(levelLerp * powerLerp+1);
            Debug.Log("差值积" + lerpJi);
            // Debug.Log("频率差值" + freLerp);
            //Debug.Log("等级差值" + levelLerp);
            // Debug.Log("能量差值" + powerLerp);
            return 0;
        }
        /// <summary>
        /// 取得数据信息
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static float[] GetDataInfo(float[] source)
        {
            source = MFCC.PreEmphasis(source);
            float[][] frames = MFCC.Framing(source);

            float[] returnObj = new float[3] { 0f,0f, 0f };
            int okCount = 0;
            for (int ja = 0; ja < frames.Length; ja++) {

                Complex32[] cp32source = MFCC.FFT(frames[ja]);





                //数学期望
                float sourcePowerEX = 0f;
                float sourceFrequancyEX = 0f;
                float sourceLevelEX = 0f;


                //滤波后的数组
                float[] newSourcePowerData = new float[cp32source.Length];

                float[] newSourceFreData = new float[cp32source.Length];

                float[] newSourceLevelData = new float[cp32source.Length];

                for (int i = 0; i < cp32source.Length; i++)
                {
                    sourceFrequancyEX += cp32source[i].Real;
                    sourceLevelEX += cp32source[i].Imaginary;
                    sourcePowerEX += cp32source[i].MagnitudeSquared;
                }
                sourceFrequancyEX = sourceFrequancyEX / (float)cp32source.Length;//频率期望值
                sourceLevelEX = sourceLevelEX / (float)cp32source.Length;//幅度期望值
                sourcePowerEX = sourcePowerEX / (float)cp32source.Length;//能量期望值



                for (int i = 0; i < cp32source.Length; i++)
                {
                    if (cp32source[i].Real >= sourceFrequancyEX)
                    {
                        newSourceFreData[i] = cp32source[i].Real;

                    }
                    if (cp32source[i].Imaginary >= sourceLevelEX)
                    {
                        newSourceLevelData[i] = cp32source[i].Imaginary;
                    }
                    if (cp32source[i].MagnitudeSquared >= sourcePowerEX)
                    {
                        newSourcePowerData[i] = cp32source[i].MagnitudeSquared;
                    }

                }
                //数学期望归零,重新运算
                sourcePowerEX = 0f;
                sourceFrequancyEX = 0f;
                sourceLevelEX = 0f;
                for (int i = 0; i < cp32source.Length; i++)
                {
                    sourceFrequancyEX += newSourceFreData[i];
                    sourceLevelEX += newSourceLevelData[i];
                    sourcePowerEX += newSourcePowerData[i];
                }
                sourceFrequancyEX = sourceFrequancyEX / (float)cp32source.Length;//频率期望值最终
                sourceLevelEX = sourceLevelEX / (float)cp32source.Length;//幅度期望值
                sourcePowerEX = sourcePowerEX / (float)cp32source.Length;//能量期望值
                if (sourceFrequancyEX == 0) continue;
                okCount += 1;
                returnObj[0] += sourceFrequancyEX;
                returnObj[1] += sourceLevelEX;
                returnObj[2] += sourcePowerEX;
            }
            returnObj[0] = returnObj[0] / okCount;
            returnObj[1] = returnObj[0] / okCount;
            returnObj[2] = returnObj[0] / okCount;
            //float[] returnObj = new float[3] {sourceFrequancyEX,sourceLevelEX,sourcePowerEX };
            return returnObj;
        }


    }
}