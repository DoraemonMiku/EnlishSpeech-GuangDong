using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;

namespace LuoHaoLab
{
    public class MFCC
    {
        /// <summary>
        /// 预加重高通滤波器常数
        /// </summary>
        public const float filterLevel = 0.95f;
        /// <summary>
        /// 分帧长度
        /// </summary>
        public const short frameLength = 512;
        /// <summary>
        /// 能量谱图
        /// </summary>
        public const short powerCount = 257;
        /// <summary>
        /// 三角带通滤波器常数
        /// </summary>
        public const byte triangularBandpassFilterConst=23;

        /// <summary>
        /// 预制加重
        /// </summary>
        /// <param name="audioData"></param>
        /// <returns></returns>
        public static float[] PreEmphasis(float[] audioData)
        {
            if (audioData.Length == 0)
            {
                throw new ArgumentNullException("audioData", "Audio Data Length is Zero!");//异常
            }
            float[] newData = new float[audioData.Length];
            newData[0] = audioData[0];
            for (int i = 1; i < audioData.Length; i++)
            {
                newData[i] = audioData[i] - filterLevel * audioData[i - 1];
            }
            return newData;
        }

        /// <summary>
        /// 分帧加窗
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static float[][] Framing(float[] data)
        {
            if (data.Length < frameLength)
            {
                throw new Exception("兄弟啊你(的帧长度)太短了!");
            }
            short frameLengthx2 = (frameLength / 2);//1/2的帧长度
            int count = Mathf.CeilToInt((float)(data.Length - frameLength) / (float)frameLengthx2);//计算帧数
            float[][] framesDataAll = new float[count][];
            for (int i = 0; i < count; i++)
            {
                float[] frameData = new float[frameLength];//一帧
                int startIdx = i * frameLengthx2;

                for (int ij = 0; ij < frameLength; ij++)
                {
                    float nowPositionData = 0f;
                    if (startIdx + ij < data.Length)
                    {
                        nowPositionData = data[startIdx + ij];
                    }
                    else
                    {
                        nowPositionData = 0f;
                    }

                    frameData[ij] = nowPositionData * (0.54f - (0.46f * Mathf.Cos(
                        (Mathf.PI * 2f * ij) / (frameLength - 1)
                        )));
                }



                framesDataAll[i] = frameData;
            }
            //Debug.Log(framesDataAll.Length * frameLengthx2+frameLength);
            //Debug.Log(data.Length);
            return framesDataAll;
        }

        /// <summary>
        /// 快速傅里叶变换
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static Complex32[] FFT(float[] datas)
        {
            Complex32[] complices = new Complex32[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                complices[i] = new Complex32(datas[i], 0f);
            }
            Fourier.Forward(complices);

            return complices;
        }

        /// <summary>
        /// 获得能量图谱
        /// </summary>
        /// <param name="fftData"></param>
        /// <returns></returns>
        public static float[] GetPower(Complex32[] fftData)
        {
            if (fftData.Length <= powerCount)
            {
                throw new Exception("FFT长度有误!");
            }
            float[] reData = new float[powerCount];
            for (int i = 0; i < powerCount; i++)
            {
                reData[i] = fftData[i].MagnitudeSquared / (float)frameLength;
            }
            return reData;
        }

        

        //TODO 三角滤波器
      /*  public static float[] TriangularBandpassFilters(float[] powerImg)
        {

        }
        */
    }
}