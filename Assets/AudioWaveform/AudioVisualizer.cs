using UnityEngine;
using UnityEngine.UI;

namespace UI.Effects
{
    public class AudioVisualizer : MonoBehaviour
    {
        private static Texture2D _defaultWaveForm;

        private static readonly int WaveformTex = Shader.PropertyToID("_WaveformTex");
        private static readonly int WaveformNum = Shader.PropertyToID("_WaveformNum");
        private static readonly int Progress = Shader.PropertyToID("_Progress");
        private static readonly int FilledColor = Shader.PropertyToID("_FilledColor");
        private static readonly int UnfilledColor = Shader.PropertyToID("_UnfilledColor");
        [field: SerializeField] private RawImage WaveFormsImage { get; set; }
        [field: SerializeField] private Material SharedWaveFormsMaterial { get; set; }

        [field: SerializeField, Range(0.25f, 10f)] private float BarScaleFactor { get; set; } = 1f;

        [Tooltip("Used in formula: 2^(BarCount)")]
        [field: SerializeField, Range(4, 7)] private int BarCountPower { get; set; } = 6;

        private int WaveFormResolution => (int)Mathf.Pow(2, BarCountPower);
        private Material _material;
        private Texture2D _currentTexture;

        public void Setup(AudioClip clip)
        {
            if (!_defaultWaveForm)
            {
                _defaultWaveForm = AudioUtils.Generate1DGrayscaleTexture(new float[WaveFormResolution]);
            }

            if (_material == null)
            {
                _material = Instantiate(SharedWaveFormsMaterial);
                WaveFormsImage.material = _material;
            }

            if (!clip)
            {
                SetupDefault();
            }
            else
            {
                Setup(clip.GetWaveform(WaveFormResolution, BarScaleFactor));
            }
        }

        private void Setup(float[] waveFormData)
        {
            SetWaveformCount(waveFormData.Length);
            SetWaveformTexture(AudioUtils.Generate1DGrayscaleTexture(waveFormData));
        }

        private void SetupDefault()
        {
            SetWaveformCount(_defaultWaveForm.width);
            SetWaveformTexture(_defaultWaveForm);
        }


        private void SetWaveformTexture(Texture2D grayscaleTexture) =>
            _material.SetTexture(WaveformTex, grayscaleTexture);

        private void SetWaveformCount(int barsCount) => _material.SetInteger(WaveformNum, Mathf.Max(1, barsCount));
        public void SetProgress(float progress01) => _material.SetFloat(Progress, Mathf.Clamp01(progress01));
        public void SetFilledColor(Color color) => _material.SetColor(FilledColor, color);
        public void SetUnfilledColor(Color color) => _material.SetColor(UnfilledColor, color);

        private void OnDestroy()
        {
            Destroy(_currentTexture);
            Destroy(_material);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_defaultWaveForm)
            {
                _defaultWaveForm = AudioUtils.Generate1DGrayscaleTexture(new float[WaveFormResolution]);
            }

            if (_material == null)
            {
                _material = Instantiate(SharedWaveFormsMaterial);
                WaveFormsImage.material = _material;
            }
        }
#endif
    }
}