using System;
using MathNet.Numerics.IntegralTransforms;

namespace LuoHaoLab
{
    [Obsolete("废弃")]
    public class AudioFFT
    {
        public static void ToFFT()
        {
           
        }
    }

    /*
    public class FFT
    {
        //waveData：离散数据，长度是2的N次访
        //windowFuctionType：窗函数(1：Hamming、2：Hanning、3：矩形、4：无)
        //sampleFreq：采样频率

        /// <summary>
        /// 傅里叶转换
        /// </summary>
        /// <param name="waveData">离散数据，长度是2的N次访</param>
        /// <param name="windowFuctionType">窗函数(1：Hamming、2：Hanning、3：矩形、4：无)</param>
        /// <param name="sampleFreq">采样频率</param>
        /// <returns>0频率|1音频数据</returns>
        public double[][] FftRun(double[] waveData, byte windowFuctionType, int sampleFreq)
        {
            double[] dataReal = (double[])waveData.Clone();

            //加窗
            WindowFuctionCross(ref dataReal, windowFuctionType);

            //重新排序
            DataSort(ref dataReal);

            //FFT变换

            FFT_butterfly(ref dataReal);

            // FFT结果转化

            double[][] fftResultArray = new double[2][];
            FFTResult_Change(dataReal, sampleFreq, ref fftResultArray);

            return fftResultArray;
        }


        //加窗
        private void WindowFuctionCross(ref double[] dataReal, byte windowFuctionType)
        {
            switch (windowFuctionType)
            {
                case 1:
                    //Hamming                    
                    WindowFuction_Hamming(ref dataReal);
                    break;

                case 2:
                    //Hanning
                    WindowFuction_Hanning(ref dataReal);
                    break;

                case 3:
                    //矩形

                    WindowFuction_Rectangular(ref dataReal);
                    break;

                default:
                    //无
                    break;
            }
        }

        //加Hamming窗
        private void WindowFuction_Hamming(ref double[] dataReal)
        {
            int len = dataReal.Length;

            dataReal[0] = 0;
            for (int i = 1; i < len - 1; i++)
            {
                dataReal[i] = dataReal[i] * (0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (len - 1)));
            }
            dataReal[len - 1] = 0;
        }

        //加Hanning窗
        private void WindowFuction_Hanning(ref double[] dataReal)
        {
            int len = dataReal.Length;

            dataReal[0] = 0;
            for (int i = 1; i < len - 1; i++)
            {
                dataReal[i] = dataReal[i] * (0.5 - 0.5 * Math.Cos(2 * Math.PI * i / (len - 1)));
            }
            dataReal[len - 1] = 0;
        }

        //加矩形窗
        private void WindowFuction_Rectangular(ref double[] dataReal)
        {
            int len = dataReal.Length;
            dataReal[0] = 0;
            dataReal[len - 1] = 0;
        }

        //重新排序
        private void DataSort(ref double[] dataReal)
        {
            int len = dataReal.Length;
            int[] count = new int[len];
            int M = (int)(Math.Log(len) / Math.Log(2));

            double[] temp_r = new double[len];
            dataReal.CopyTo(temp_r, 0);

            for (int l = 0; l < M; l++)
            {
                int space = (int)Math.Pow(2, l);
                int add = (int)Math.Pow(2, M - l - 1);
                for (int i = 0; i < len; i++)
                {
                    if ((i / space) % 2 != 0)
                        count[i] += add;
                }
            }
            for (int i = 0; i < len; i++)
            {
                dataReal[i] = temp_r[count[i]];
            }
        }



        private void FFT_butterfly(ref double[] dataReal)
        {
            if (dataReal.Length == 0) return;
            int len = dataReal.Length;
            double[] tempReal = new double[len];
            double[] tempImaginary = new double[len];
            double data_max;

            for (int i = 0; i < len; i++)
            {
                tempReal[i] = dataReal[i];
                tempImaginary[i] = 0;
            }

            double WN_r, WN_i;
            int M = (int)(Math.Log(len) / Math.Log(2));
            for (int l = 0; l < M; l++)
            {
                int space = (int)Math.Pow(2, l);
                int num = space;
                double temp1Real, temp1Imaginary, temp2Real, temp2Imaginary;
                for (int i = 0; i < num; i++)
                {
                    int p = (int)Math.Pow(2, M - 1 - l);
                    WN_r = Math.Cos(2 * Math.PI / len * p * i);
                    WN_i = -Math.Sin(2 * Math.PI / len * p * i);
                    for (int j = 0, n = i; j < p; j++, n += (int)Math.Pow(2, l + 1))
                    {
                        temp1Real = tempReal[n];
                        temp1Imaginary = tempImaginary[n];
                        temp2Real = tempReal[n + space];
                        temp2Imaginary = tempImaginary[n + space];
                        tempReal[n] = temp1Real + temp2Real * WN_r - temp2Imaginary * WN_i;
                        tempImaginary[n] = temp1Imaginary + temp2Imaginary * WN_r + temp2Real * WN_i;
                        tempReal[n + space] = temp1Real - temp2Real * WN_r + temp2Imaginary * WN_i;
                        tempImaginary[n + space] = temp1Imaginary - temp2Imaginary * WN_r - temp2Real * WN_i;
                    }
                }
            }

            data_max = 0.0;
            for (int i = 0; i < len; i++)
            {
                dataReal[i] = Math.Sqrt(tempReal[i] * tempReal[i] + tempImaginary[i] * tempImaginary[i]);

                if (dataReal[i] >= data_max) data_max = dataReal[i];
            }


            for (int i = 0; i < len; i++)
            {
                dataReal[i] = Math.Log(dataReal[i] / data_max);
            }
        }

        //FFT结果转化
        private void FFTResult_Change(double[] dataReal, int sampleFreq, ref double[][] fftResultArray)
        {
            int len = dataReal.Length;
            double[] freqArray = (double[])dataReal.Clone();

            fftResultArray[0] = freqArray;
            fftResultArray[1] = dataReal;

            for (int i = 0; i < len; i++)
            {
                fftResultArray[0][i] = (double)sampleFreq * (i + 1) / len;
            }
        }
    }*/
}
