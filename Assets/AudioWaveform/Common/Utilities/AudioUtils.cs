using UnityEngine;

public static class AudioUtils
{
    /// <summary>
    /// Red channel contains sample data
    /// </summary>
    public static Texture2D Generate1DGrayscaleTexture(float[] waveFormData) 
    {
        var waveformTexture = new Texture2D(waveFormData.Length, 1, TextureFormat.RFloat, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Color[] colors = new Color[waveFormData.Length];

        for (int i = 0; i < waveFormData.Length; i++)
        {
            colors[i] = new Color(waveFormData[i], waveFormData[i], waveFormData[i]);
        }

        waveformTexture.SetPixels(colors);
        waveformTexture.Apply();
        return waveformTexture;
    }

}