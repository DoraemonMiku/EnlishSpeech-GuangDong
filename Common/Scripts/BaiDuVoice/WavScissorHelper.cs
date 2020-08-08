using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 文件截取处理
/// </summary>
public class WavScissorHelper
{
    public static bool GetWavFileScissor(string OriginalAudioFilePath, string DestinedAudioFilePath, int beginCutTime, int endCutTime, ref string errorInfo,ref byte[] data)
    {
        if (!File.Exists(OriginalAudioFilePath))
        {
            errorInfo = string.Format("源文件:[{0}]不存在!", OriginalAudioFilePath);
            return false;
        }
        if (!Directory.Exists(Path.GetDirectoryName(DestinedAudioFilePath)))
        {
            errorInfo = string.Format("文件夹:[{0}]不存在!", Path.GetDirectoryName(DestinedAudioFilePath));
            return false;
        }
        if ((beginCutTime < 0) || (endCutTime < 0)
            || (beginCutTime > endCutTime))
        {
            errorInfo = string.Format("截取时间[{0}][{1}]有误!", beginCutTime, endCutTime);
            return false;
        }
        try
        {
            WavFile oldFile = new WavFile(OriginalAudioFilePath);
            if (endCutTime > oldFile.PlayTime)
            {
                errorInfo = string.Format("结束截取时间[{0}]大于音频文件播放时间[{1}]!", endCutTime, oldFile.PlayTime);
                return false;
            }
            int timeSpan = endCutTime - beginCutTime;

            WavFile newFile = new WavFile(DestinedAudioFilePath);
            newFile.WavFormat = oldFile.WavFormat;

            byte[] newAudioBytes = new byte[oldFile.WavFormat.ByteRate * timeSpan];
            Array.Copy(oldFile.AudioDataBytes, oldFile.WavFormat.ByteRate * beginCutTime, newAudioBytes, 0, newAudioBytes.Length);
            data = newAudioBytes;
            newFile.WriteWavFile(newAudioBytes);
            return true;
        }
        catch (Exception e)
        {
            errorInfo = e.Message;
            return false;

        }
    }

}

/// <summary>
/// 淡入淡出处理
/// </summary>
public class WavFadeHelper
{
    private WavFile _wavFile;

    public WavFadeHelper(string wavFilePath)
    {
        _wavFile = new WavFile(wavFilePath);
    }


    public bool FadeAtFirstSec(FadeMode fadeMode, long fadeLength)
    {
        return FadeAtSec(fadeMode, 0, fadeLength);
    }

    public bool FadeAtEndSec(FadeMode fadeMode, long fadeLength)
    {
        long totalTime = _wavFile.PlayTime;
        if (totalTime < fadeLength)
            fadeLength = totalTime;
        long startTime = totalTime - fadeLength;

        return FadeAtSec(fadeMode, startTime, startTime + fadeLength);
    }

    public bool FadeAtSec(FadeMode fadeMode, long startSec, long endSec)
    {
        //总播放时间
        long totalTime = _wavFile.PlayTime;

        //采样位数        
        // 16 bit 纵坐标为采样系数 细化为 2^16= 65535份 16位二进制表示 [ 0000 0000 0000 0000 ]
        // 即为2Byte (每单位时间内产生 2Byte 的音频数据)

        int byteNumPerSample = _wavFile.WavFormat.BitsPerSample / 8;

        //采样频率
        // 11025Hz 横坐标为采样频率 单位时间为: 1/11025 s           
        //单位时间内产生的的音频数据大小为 2Byte * 11025Hz * 声道数           


        //audioData 是按字节处理 16bit采样 每次获取两个字节进行处理

        //开始位置 存储单元索引
        long startIndex = startSec * byteNumPerSample * _wavFile.WavFormat.SampleRate;
        if (startIndex % byteNumPerSample > 0)
            startIndex -= startIndex % byteNumPerSample;

        //结束位置 存储单元索引
        long endIndex = endSec * byteNumPerSample * _wavFile.WavFormat.SampleRate;
        if (endIndex > _wavFile.AudioDataBytes.Length - 1)
            endIndex = _wavFile.AudioDataBytes.Length - 1;

        //字节数组 音频数据
        byte[] audioDatas = _wavFile.AudioDataBytes;

        //初始衰减率
        double rate = 1.0;

        //对每个单位时间内的 样点幅值 进行衰减
        //从淡化开始时间到结束时间共有 [ 采样频率 * 时间段(s) * 采样位数/8(Byte) ] 个存储单元 
        //每次取出 采样位数/8(Byte) 个存储单元进行衰减

        // i 的值为每次衰减处理时的 字节读取位置 (索引)
        for (int i = (int)startIndex; i < endIndex; i += byteNumPerSample)
        {
            //衰减率的方向与淡入淡出有关
            //衰减率的大小与当前读取处理字节的索引位置有关
            if (fadeMode == FadeMode.FadeOut)
                rate = (double)(endIndex - i) / (endIndex - startIndex);
            else
                rate = (double)(i - startIndex) / (endIndex - startIndex);

            //单位时间内的字节数为1 即采用8bit采样位数  每次只需要对一个字节进行衰减 
            if (byteNumPerSample == 1)
            {
                //获取需要衰减的字节值
                byte audioData = audioDatas[i];

                //进行衰减线性处理
                byte changedAudioData = (byte)((double)audioData * rate);
                //byte changedAudioData = Convert.ToByte((double)audioData * rate);

                //对原来的值进行更改
                audioDatas[i] = changedAudioData;
            }
            else if (byteNumPerSample == 2)
            {
                //16bit 量化 每单位时间产生2Byte数据 因此取 2Byte 的数据进行衰减处理
                byte[] audioData = new byte[2];
                Array.Copy(audioDatas, i, audioData, 0, 2);

                //单个样点幅值
                double sample = 0;

                //转换为整数进行线性处理
                sample = (double)BitConverter.ToInt16(audioData, 0) * rate;

                //转换为字节存储
                byte[] buffer = BitConverter.GetBytes(Convert.ToInt16(sample));
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);

                Array.Copy(buffer, 0, audioDatas, i, 2);
            }
        }
        _wavFile.WriteWavFile(audioDatas);

        return true;
    }
}

/// <summary>
/// 淡入淡出模式
/// </summary>
public enum FadeMode
{
    FadeIn,
    FadeOut
}

/// <summary>
/// WAV 文件
/// </summary>
public class WavFile
{
    private WavFormat _wavFormat;   //文件格式
    private long _fileLength;       //文件长度
    private string _filePath;       //文件路径
    private byte[] _audioData;      //语音数据

    private WavFile() { }
    public WavFile(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// 文件格式
    /// </summary>
    public WavFormat WavFormat
    {
        get
        {
            if (_wavFormat == null)
                _wavFormat = GetWavFormat();

            return _wavFormat;
        }
        set { _wavFormat = value; }
    }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileLength
    {
        get
        {
            if (_wavFormat == null)
                _wavFormat = GetWavFormat();

            return _fileLength;
        }
        set { _fileLength = value; }
    }

    /// <summary>
    /// 播放时长
    /// </summary>
    public long PlayTime
    {
        get
        {
            if (_wavFormat == null)
                _wavFormat = GetWavFormat();

            return _audioData.Length / _wavFormat.ByteRate;
        }
    }

    /// <summary>
    /// 语音数据
    /// </summary>
    public byte[] AudioDataBytes
    {
        get
        {
            if (_wavFormat == null)
                _wavFormat = GetWavFormat();

            return _audioData;
        }
        set { _audioData = value; }
    }

    /// <summary>
    /// 设置Wav文件格式
    /// </summary>
    /// <param name="bitsPerSample">采样位数</param>
    /// <param name="channels">声道数</param>
    /// <param name="sampleRate">采样率</param>
    public void SetWavFormat(int bitsPerSample, int channels, int sampleRate)
    {
        _wavFormat = new WavFormat();
        _wavFormat.BitsPerSample = bitsPerSample;
        _wavFormat.Channels = channels;
        _wavFormat.SampleRate = sampleRate;
    }

    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="audioData">音频数据</param>
    public void WriteWavFile(byte[] audioData)
    {
        WriteWavFile(_wavFormat, audioData, 0, audioData.Length);
    }

    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="wavFormat">文件格式</param>
    /// <param name="audioData">音频数据</param>
    /// <param name="startIndex">audioData数组开始索引位置</param>
    /// <param name="length">写入audioData数组长度</param>
    public void WriteWavFile(WavFormat wavFormat, byte[] audioData, int startIndex, int length)
    {
        FileStream fs = null;
        BinaryWriter bw = null;
        try
        {
            fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
            bw = new BinaryWriter(fs);
            fs.Position = 0;
            bw.Write(new char[4] { 'R', 'I', 'F', 'F' });
            //ChunkFileSize
            bw.Write((int)(length + 44 - 8));
            bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
            bw.Write((int)16);
            bw.Write((short)1);
            bw.Write((short)wavFormat.Channels);
            bw.Write(wavFormat.SampleRate);
            bw.Write((int)(wavFormat.SampleRate * ((wavFormat.BitsPerSample * wavFormat.Channels) / 8)));
            bw.Write((short)((wavFormat.BitsPerSample * wavFormat.Channels) / 8));
            bw.Write((short)wavFormat.BitsPerSample);
            bw.Write(new char[4] { 'd', 'a', 't', 'a' });
            bw.Write(length);
            bw.Write(audioData, startIndex, length);
        }
        finally
        {
            if (bw != null)
                bw.Close();
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
        }
    }

    /// <summary>
    /// 获取Wav文件格式
    /// </summary>
    /// <returns></returns>
    private WavFormat GetWavFormat()
    {
        FileStream fs = null;
        BinaryReader br = null;
        WavFormat wavFormat = new WavFormat();

        try
        {
            fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);

            _fileLength = fs.Length;

            fs.Position = 22;
            wavFormat.Channels = br.ReadInt16();
            fs.Position = 24;
            wavFormat.SampleRate = br.ReadInt32();
            fs.Position = 28;
            wavFormat.ByteRate = br.ReadInt32();
            fs.Position = 34;
            wavFormat.BitsPerSample = br.ReadInt16();

            //The audio data
            fs.Position = 44;
            int dataByteSize = (int)(fs.Length - 44);
            if (_audioData == null)
                _audioData = new byte[dataByteSize];
            fs.Read(_audioData, 0, dataByteSize);

        }
        finally
        {
            if (br != null)
                br.Close();
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
        }
        return wavFormat;
    }
}

/// <summary>
/// WAV 文件格式
/// </summary>
public class WavFormat
{
    /// <summary>
    /// 采样位数
    /// </summary>
    public int BitsPerSample;
    /// <summary>
    /// 声道数
    /// </summary>
    public int Channels;
    /// <summary>
    /// 采样率
    /// </summary>
    public int SampleRate;
    /// <summary>
    /// 传输速率(播放速率)
    /// </summary>
    public long ByteRate;
}