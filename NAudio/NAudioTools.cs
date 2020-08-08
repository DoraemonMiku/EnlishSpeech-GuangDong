using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NAudio.Wave;
using NAudio;
using System;

public class NAudioTools
{
    public class LAudioConver
    {
        private string pathTT="";
        public LAudioConver(string path)
        {
            AudioFileReader fileReader = new AudioFileReader(path);
             this.pathTT = path + ".temp.wav";
            WaveFileWriter.CreateWaveFile16(pathTT, fileReader);
            fileReader.Flush();

        }
        public void DestoryThis()
        {
           if(File.Exists(pathTT)) File.Delete(pathTT);
        }
        public string GetWavAudioPath
        {
            get { return pathTT; }
        }
      
    }

}
