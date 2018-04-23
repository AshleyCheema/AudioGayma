using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData : MonoBehaviour
{
    public float DbValue;
    public float rms;
    public float pitchValue;
    public float frequency;
    public int samplerate = 11024;

    public GameObject go;

    private const int SampleRate = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    private Transform[] visualList;
    private float[] visualScale;
    private int amnVisual = 4;

    // Use this for initialization
    void Start ()
    {
		_samples = new float[SampleRate];
		_spectrum = new float[SampleRate];
        _fSample = AudioSettings.outputSampleRate;

    }
	
	// Update is called once per frame
	void Update ()
    {
        frequency = GetFundamentalFrequency();
        AnalyseSound();

        //go.GetComponent<Transform>().Translate(0, Time.time * 5, 0);

        //if(frequency < )




    }

    private void BuildLine()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for (int i = 0; i < amnVisual; i++)
        {
            //go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.tag = "Note";
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i;
            
        }
    }

    void AnalyseSound()
    {
        GetComponent<AudioSource>().GetOutputData(_samples, 0);
        int i;
        float sum = 0;
        for(i = 0; i < SampleRate; i++)
        {
            sum += _samples[i] * _samples[i];
        }
        rms = Mathf.Sqrt(sum / SampleRate);
        DbValue = 20 * Mathf.Log10(rms / RefValue);

        if (DbValue < -160) DbValue = -160;

        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        int maxN = 0;

        for(i = 0; i < SampleRate; i++)
        {
            if(!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                
                continue;

                maxV = _spectrum[i];
                maxN = i; 
        }

        float freqN = maxN;

        if (maxN > 0 && maxN < SampleRate - 1)
        {
            float dL = _spectrum[maxN - 1] / _spectrum[maxN];
            float dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }

        pitchValue = freqN * (_fSample / 2) / SampleRate;
    }

    float GetFundamentalFrequency()
    {
        float fundamentalFrequency = 0.0f;
        float[] data = new float[8192];
        GetComponent<AudioSource>().GetSpectrumData(data, 0, FFTWindow.BlackmanHarris);
        float s = 0.0f;
        int i = 0;
        for (int j = 1; j < 8192; j++)
        {
            if (s < data[j])
            {
                s = data[j];
                i = j;
            }
        }
        fundamentalFrequency = i * samplerate / 8192;
        return fundamentalFrequency;
    }

}
