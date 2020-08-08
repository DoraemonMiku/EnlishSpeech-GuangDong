using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioLab : MonoBehaviour
{
    public float pitchSpeed = 1f;
    public int samples = 44100;
    public int frency =44000;
    public AudioSource audioSource;

  
    // Start is called before the first frame update
    void Start()
    {
        // audioSource.clip = CeateAudioClip();
        //audioSource.Play();
        StartCoroutine(HZPlayer());
       
    }
    private void Update()
    {
        if (audioSource.isPlaying) UpdataCurve(audioSource);
    }
    /*private void Update()
    {
        //audioSource.pitch = 3f*Mathf.Cos(2f*pitchSpeed*Time.time * Mathf.PI);
    }*/
    public Text text;
    IEnumerator HZPlayer()
    {
        AudioClip[] acs = new AudioClip[frency];
        //float[] times = new float[frency];
        int allLenth = 0;
        for(int i = 1; i <= frency; i++)
        {
            acs[i - 1] = CeateAudioClip(i);
            allLenth += acs[i - 1].samples;
        }

        float[] lastData = new float[allLenth];
        
        int now = 0;
       
        for (int i = 0;i<acs.Length ; i++)
        {
            float[] data = new float[acs[i].samples];
            acs[i].GetData(data, 0);
            data.CopyTo(lastData, now);
            now += acs[i].samples-1 ;
        }
        AudioClip last=AudioClip.Create("Last",allLenth*10,1,samples,false);
        
        
        
       // last.SetData(lastData, allLenth-1);

        for(int i = 0; i < 10; i++)
        {
            last.SetData(lastData, i*allLenth);
            Array.Reverse(lastData);
        }

        Debug.Log(last.length);
        yield return new WaitForSeconds(3f);
        audioSource.clip = last;
        audioSource.Play();

        text.text = "Audio Clip Have Created!";
        for (int i = 0; i < acs.Length; i++)
        {

            //  text.text ="当前:"+ (i + 1).ToString() + "Hz";


            yield return null;
           // yield return new WaitForSecondsRealtime(acs[i].length);
        }
    }
    public LineRenderer lr;
   public void UpdataCurve(AudioSource ac)
    {
        float[] samplePoint =new float[256];
        int start=ac.timeSamples - 128;
        //int end = ac.timeSamples + 128;
        start = Mathf.Clamp(start, 0, ac.clip.samples - 1);
        //end = Mathf.Clamp(end, 0, ac.clip.samples - 1);
        //float[] acS = new float[end - start];

        bool ok=ac.clip.GetData(samplePoint,start);
        Vector3[] v3s = new Vector3[samplePoint.Length];
        for(int i = 0; i < samplePoint.Length; i++)
        {
            float x = ((float)i / 12.8f) -10f;
            float y = 0f;
            y = (samplePoint[i] * 4f) +1f;
            v3s[i]=new Vector3(x,y,0);
           
        }
        
        lr.positionCount = samplePoint.Length;
        lr.SetPositions(v3s);
       
    }

    public AudioClip CeateAudioClip(int hz)
    {
       /* float maxLenth = 0 ;
        for(int i = 1; i <= frency; i++)
        {
            maxLenth+=1/i;
        }*/
        AudioClip clip = AudioClip.Create("Audio:"+hz.ToString()+"Hz", samples/hz , 1,samples, false);
        float[] clips = new float[clip.samples];
        
     for(int i = 0; i < clip.samples; i++)
        {
            clips[i] = Mathf.Sin(Mathf.PI * 2f * i/ (clip.samples-1));
        }
        clip.SetData(clips,0);
        return clip;

    }
}
