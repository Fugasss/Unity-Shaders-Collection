using System;
using UnityEngine;

public static class AudioExtensions
{
    /// <summary>
    /// Generates waveform (samples) data
    /// </summary>
    public static float[] GetWaveform(this AudioClip clip, int sampleCount, float modifier = 1f)
    {
        if (clip == null || sampleCount <= 0)
        {
            Debug.LogError("Invalid AudioClip or sampleCount");
            return Array.Empty<float>();
        }

        float[] allSamples = new float[clip.samples * clip.channels];
        clip.GetData(allSamples, 0);

        float[] waveform = new float[sampleCount];
        int stepSize = allSamples.Length / sampleCount;

        for (int i = 0; i < sampleCount; i++)
        {
            float sum = 0f;
            for (int j = 0; j < stepSize; j++)
            {
                int index = i * stepSize + j;
                sum += Mathf.Abs(allSamples[index]);
            }

            waveform[i] = (sum / stepSize) * modifier; // average amplitude for that segment
        }

        return waveform;
    }
    
}