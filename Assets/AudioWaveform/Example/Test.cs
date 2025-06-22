using NaughtyAttributes;
using UI.Effects;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private AudioVisualizer _visualizer;
    
    [SerializeField] private AudioClip _audioClip;

    [SerializeField, Range(0f, 1f), OnValueChanged(nameof(UpdateFilling))]
    private float _fillTest = 0f;

    private void UpdateFilling() => _visualizer.SetProgress(_fillTest);

    [Button]
    private void Generate() => _visualizer.Setup(_audioClip);
}