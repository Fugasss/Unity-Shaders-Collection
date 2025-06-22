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
        
        [SerializeField] private RawImage _waveFormsImage;
        [SerializeField] private Material _sharedWaveFormsMaterial;
        [SerializeField, Range(0.1f, 10f)] private float _barScaleFactor = 1f;

        [Tooltip("Used in formula: 2^(BarCount)")]
        [SerializeField, Range(4, 7)] private int _barCountPower = 6;

        private Material _material;
        private Texture2D _currentSamplesTexture;
        
        private int WaveformResolution => (int)Mathf.Pow(2, _barCountPower);

        public void Setup(AudioClip clip)
        {
            if (!_defaultWaveForm)
            {
                _defaultWaveForm = AudioUtils.Generate1DGrayscaleTexture(new float[WaveformResolution]);
            }

            if (_material == null)
            {
                _material = Instantiate(_sharedWaveFormsMaterial);
                _waveFormsImage.material = _material;
            }

            if (!clip || clip.samples <= 0)
            {
                SetupDefault();
            }
            else
            {
                Setup(clip.GetWaveform(WaveformResolution, _barScaleFactor));
            }
        }

        public void Setup(float[] waveFormData)
        {
            if (waveFormData.Length == 0)
            {
                SetupDefault();
                return;
            }
            
            SetWaveformCount(waveFormData.Length);
            SetWaveformTexture(AudioUtils.Generate1DGrayscaleTexture(waveFormData));
        }

        private void SetupDefault()
        {
            SetWaveformCount(_defaultWaveForm.width);
            SetWaveformTexture(_defaultWaveForm);
        }


        private void SetWaveformTexture(Texture2D grayscaleTexture)
        {
            _currentSamplesTexture = grayscaleTexture;
            _material.SetTexture(WaveformTex, grayscaleTexture);
        }

        private void SetWaveformCount(int barsCount) => _material.SetFloat(WaveformNum, Mathf.Max(1f, barsCount));
        public void SetProgress(float progress01) => _material.SetFloat(Progress, Mathf.Clamp01(progress01));
        public void SetFilledColor(Color color) => _material.SetColor(FilledColor, color);
        public void SetUnfilledColor(Color color) => _material.SetColor(UnfilledColor, color);

        private void OnDestroy()
        {
            if (_currentSamplesTexture && _currentSamplesTexture != _defaultWaveForm)
                Destroy(_currentSamplesTexture);

            if (_material)
                Destroy(_material);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            if (!_defaultWaveForm)
            {
                _defaultWaveForm = AudioUtils.Generate1DGrayscaleTexture(new float[WaveformResolution]);
            }

            if (!_material || _waveFormsImage.material != _material)
            {
                _material = Instantiate(_sharedWaveFormsMaterial);
                _waveFormsImage.material = _material;
            }
        }
#endif
    }
}