using UnityEngine;

namespace UI.Effects
{
    [RequireComponent(typeof(RectTransform))]
    public class SizePulseEffectUI : MonoBehaviour
    {
        private static readonly int Pivot = Shader.PropertyToID("_Pivot");
        private static readonly int Speed = Shader.PropertyToID("_Speed");
        private static readonly int Scale = Shader.PropertyToID("_Scale");
        
        [SerializeField] private Material _sharedMaterial;
        [SerializeField] private bool _updatePivotOnEnable;

        private Material _material;
        
        private void OnEnable()
        {
            if (_material == null) 
                _material = new Material(_sharedMaterial);
            
            if(_updatePivotOnEnable)
                UpdateVertexPivot();
        }

        private void OnDestroy()
        {
            if (_material != null) 
                Destroy(_material);
        }

        public void UpdateVertexPivot() => _material.SetVector(Pivot, ((RectTransform)transform).anchoredPosition);
        public void SetAnimationSpeed(float speed) => _material.SetFloat(Speed, speed);
        public void SetAnimationScale(float scale) => _material.SetFloat(Scale, scale);
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            OnEnable();
            UpdateVertexPivot();
        }
#endif
    }
}