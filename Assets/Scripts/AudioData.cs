using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData : MonoBehaviour
{
    public float DbValue;
    public float rms;
    public float pitchValue;

    private const int SampleRate = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    private Transform[] visualList;
    private float[] visualScale;
    private int amnVisual = 2;

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
        AnalyseSound();
        if(DbValue < -32.0f || DbValue > -25.0f)
        {
           BuildLine();
        }    
	}

    private void BuildLine()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for (int i = 0; i < amnVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i;
        }
    }

    private void Visualiser()
    {

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
}
