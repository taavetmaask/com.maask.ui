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
        private static readonly int TINT = Shader.PropertyToID("_Tint");
        private static readonly int COLOR = Shader.PropertyToID("_Color");
        private static readonly int STROKE = Shader.PropertyToID("_Stroke");
        private static readonly int OUTLINE = Shader.PropertyToID("_Outline");
        private static readonly int OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");
        private static readonly int OUTLINE_TEXTURE = Shader.PropertyToID("_OutlineTex");
        
        [SerializeField] private float _tlRadius;
        [SerializeField] private float _trRadius;
        [SerializeField] private float _blRadius;
        [SerializeField] private float _brRadius;

        [SerializeField] private Color _tint = Color.white;
        [SerializeField] private bool _unified = true;
        [SerializeField] private float _softness = 0.5f;
        [SerializeField] private float _stroke;
        
        [SerializeField] private bool _outline;
        [SerializeField] private float _outlineSize = 4.0f;
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField] private Sprite _outlineSprite;
        
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

        // ReSharper disable once InconsistentNaming
        public bool unified
        {
            get => _unified;
            set
            {
                _unified = value;
                UpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public float topLeftRadius
        {
            get => _tlRadius;
            set
            {
                _tlRadius = value;
                ClampAndUpdateMaterial();
            }
        }
        
        // ReSharper disable once InconsistentNaming
        public float topRightRadius
        {
            get => _trRadius;
            set
            {
                _trRadius = value;
                ClampAndUpdateMaterial();
            }
        }
        
        // ReSharper disable once InconsistentNaming
        public float bottomLeftRadius
        {
            get => _blRadius;
            set
            {
                _blRadius = value;
                ClampAndUpdateMaterial();
            }
        }
        
        // ReSharper disable once InconsistentNaming
        public float bottomRightRadius
        {
            get => _brRadius;
            set
            {
                _brRadius = value;
                ClampAndUpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public float stroke
        {
            get => _stroke;
            set
            {
                _stroke = value;
                ClampAndUpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public float softness
        {
            get => _softness;
            set
            {
                _softness = value;
                ClampAndUpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public bool outlineEnabled
        {
            get => _outline;
            set
            {
                _outline = value;
                ClampAndUpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public float outline
        {
            get => _outlineSize;
            set
            {
                _outlineSize = value;
                ClampAndUpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public Color outlineColor
        {
            get => _outlineColor;
            set
            {
                _outlineColor = value;
                ClampAndUpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public Sprite outlineSprite
        {
            get => _outlineSprite;
            set
            {
                _outlineSprite = value;
                ClampAndUpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public Color tint
        {
            get => _tint;
            set
            {
                _tint = value;
                ClampAndUpdateMaterial();
            }
        }
        
        public void SetAllRadii(float radius)
        {
            _tlRadius = radius;
            _trRadius = radius;
            _blRadius = radius;
            _brRadius = radius;

            ClampAndUpdateMaterial();
        }

        private void ClampAndUpdateMaterial()
        {
            ClampValues();
            UpdateMaterial();
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
                : new Vector4(_blRadius, _tlRadius, _brRadius, _trRadius);
            
            material.SetVector(CORNERS, corners);
            material.SetFloat(SOFTNESS, _softness);
            material.SetColor(TINT, _tint);
            material.SetFloat(STROKE, _outline ? _outlineSize : _stroke);
            material.SetColor(COLOR, color);
            material.SetVector(SIZE, size);
            material.SetFloat(OUTLINE, _outline ? 1.0f : 0.0f);
            material.SetColor(OUTLINE_COLOR, _outlineColor);
            material.SetTexture(OUTLINE_TEXTURE, _outlineSprite != null ? _outlineSprite.texture : null);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ClampValues();
        }

        private void ClampValues()
        {
            var rect = rectTransform.rect;
            var max = Mathf.Min(rect.width, rect.height);
            var halfMax = max / 2.0f;

            _softness = Mathf.Clamp(_softness, 0.0f, max);
            _stroke = Mathf.Clamp(_stroke, 0.0f, halfMax);
            _outlineSize = Mathf.Clamp(_outlineSize, 0.0f, halfMax);
            _tlRadius = Mathf.Clamp(_tlRadius, 0.0f, halfMax);
            _trRadius = Mathf.Clamp(_trRadius, 0.0f, halfMax);
            _blRadius = Mathf.Clamp(_blRadius, 0.0f, halfMax);
            _brRadius = Mathf.Clamp(_brRadius, 0.0f, halfMax);
        }
    }
}