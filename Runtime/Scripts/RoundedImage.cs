using UnityEngine;
using UnityEngine.UI;

namespace Maask.UI
{
    [AddComponentMenu("UI/Rounded Image", 12)]
    public class RoundedImage : Image
    {
        private static readonly int CORNERS = Shader.PropertyToID("_Corners");
        private static readonly int SIZE = Shader.PropertyToID("_Size");
        private static readonly int SOFTNESS = Shader.PropertyToID("_Softness");
        private static readonly int COLOR = Shader.PropertyToID("_Color");
        private static readonly int STROKE = Shader.PropertyToID("_Stroke");
        
        [SerializeField] private float _tlRadius;
        [SerializeField] private float _trRadius;
        [SerializeField] private float _blRadius;
        [SerializeField] private float _brRadius;
        
        [SerializeField] private bool _unified;
        [SerializeField] private float _softness = 0.5f;
        [SerializeField] private float _stroke;

        private Material _defaultMaterial;
        
        public override Material defaultMaterial
        {
            get
            {
                if (_defaultMaterial == null)
                {
                    _defaultMaterial = new Material(Shader.Find("UI/Rounded Image"));
                }

                return _defaultMaterial;
            }
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
            UpdateRoundedMaterial();
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateRoundedMaterial();
        }

        private void UpdateRoundedMaterial()
        {
            var rect = rectTransform.rect;
            var size = new Vector2(rect.width, rect.height);
            var corners = _unified 
                ? Vector4.one * _tlRadius 
                : new Vector4(_blRadius, _tlRadius, _trRadius, _brRadius);
            
            material.SetVector(CORNERS, corners);
            material.SetFloat(SOFTNESS, _softness);
            material.SetFloat(STROKE, _stroke);
            material.SetColor(COLOR, color);
            material.SetVector(SIZE, size);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            var rect = rectTransform.rect;
            var max = Mathf.Min(rect.width, rect.height);
            var halfMax = max / 2.0f;

            _softness = Mathf.Clamp(_softness, 0.0f, max);
            _stroke = Mathf.Clamp(_stroke, 0.0f, halfMax);
            _tlRadius = Mathf.Clamp(_tlRadius, 0.0f, halfMax);
            _trRadius = Mathf.Clamp(_trRadius, 0.0f, halfMax);
            _blRadius = Mathf.Clamp(_blRadius, 0.0f, halfMax);
            _brRadius = Mathf.Clamp(_brRadius, 0.0f, halfMax);
        }
    }
}