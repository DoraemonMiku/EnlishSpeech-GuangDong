using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuoHaoLab;
using MathNet.Numerics;

public class Test : MonoBehaviour
{
    public AudioSource audioSource;
    public LineRenderer testLine;

    public AudioClip[] audioClips = new AudioClip[2];

    float[] data;
    // Start is called before the first frame update
    void Start()
    {
        data=new float[audioSource.clip.samples];
        
       audioSource.clip.GetData(data,0);

        AudioClip ac=AudioClip.Create("Test", data.Length, 1, 16000, false);
        ac.SetData(MFCC.PreEmphasis(data), 0);
       
        audioSource.clip = ac;
       // Debug.Log(ModelFileManager.CutAudioToFrames(data).Length);
    }
    public int idx = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Play();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Complex32[] complex = MFCC.FFT(MFCC.Framing(MFCC.PreEmphasis(data))[idx]);
            

            float[] powerImg = MFCC.GetPower(complex);
            testLine.positionCount = powerImg.Length;
            for (int i = 0; i < powerImg.Length; i++)
            {
                testLine.SetPosition(i, new Vector2(i, complex[i].Magnitude));
                Debug.Log(complex[i].MagnitudeSquared);
            }

            
            /*testLine.positionCount = complex.Length;
            for (int i = 0; i < complex.Length; i++)
            {
                testLine.SetPosition(i, new Vector2(complex[i].Real, complex[i].Imaginary));

            }*/

            idx += 1;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            float[] dataA =new float[audioClips[0].samples];
            audioClips[0].GetData(dataA, 0);
            float[] dataB = new float[audioClips[1].samples];
            audioClips[1].GetData(dataB, 0);
            PingCeBeta.GetGrade(dataA,dataB);
        }
    }

}
