using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Maask.UI
{
    [AddComponentMenu("UI/Rounded Image", 12)]
    public class RoundedImage : Image
    {
        private static readonly int RADII = Shader.PropertyToID("_Radii");
        private static readonly int SIZE = Shader.PropertyToID("_Size");
        private static readonly int SOFTNESS = Shader.PropertyToID("_Softness");
        private static readonly int COLOR = Shader.PropertyToID("_Color");

        private static readonly int FILL_COLOR = Shader.PropertyToID("_FillColor");
        private static readonly int FILL_TEXTURE = Shader.PropertyToID("_MainTex");
        
        private static readonly int OUTLINE = Shader.PropertyToID("_Outline");
        private static readonly int OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");
        private static readonly int OUTLINE_TEXTURE = Shader.PropertyToID("_OutlineTexture");
        
        [SerializeField] private bool _unified = true;
        [SerializeField] private float _tlRadius;
        [SerializeField] private float _trRadius;
        [SerializeField] private float _blRadius;
        [SerializeField] private float _brRadius;

        [SerializeField] private bool _fillEnabled = true;
        [FormerlySerializedAs("_tint")] 
        [SerializeField] private Color _fillColor = Color.white;
        
        [FormerlySerializedAs("_outline")] 
        [SerializeField] private bool _outlineEnabled;
        [FormerlySerializedAs("_outlineSize")]
        [SerializeField] private float _outline = 4.0f;
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField] private Sprite _outlineSprite;
        
        [SerializeField] private float _softness = 0.5f;
        [SerializeField] private Material _defaultMaterial;
        
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

        #region Radii Accessors
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
        public float radius
        {
            get => _tlRadius;
            set
            {
                _tlRadius = value;
                _trRadius = value;
                _blRadius = value;
                _brRadius = value;
                ClampAndUpdateMaterial();
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
        #endregion

        #region Fill Accessors
        // ReSharper disable once InconsistentNaming
        public bool fillEnabled
        {
            get => _fillEnabled;
            set
            {
                _fillEnabled = value;
                UpdateMaterial();
            }
        }
        
        // ReSharper disable once InconsistentNaming
        public Color fillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                UpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public Sprite fillSprite
        {
            get => sprite;
            set
            {
                sprite = value;
                UpdateMaterial();
            }
        }
        #endregion
        
        #region Outline Accessors
        // ReSharper disable once InconsistentNaming
        public bool outlineEnabled
        {
            get => _outlineEnabled;
            set
            {
                _outlineEnabled = value;
                UpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public float outline
        {
            get => _outline;
            set
            {
                _outline = value;
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
                UpdateMaterial();
            }
        }

        // ReSharper disable once InconsistentNaming
        public Sprite outlineSprite
        {
            get => _outlineSprite;
            set
            {
                _outlineSprite = value;
                UpdateMaterial();
            }
        }
        #endregion
        
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
        
        private void ClampAndUpdateMaterial()
        {
            ValidateValues();
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
         
            materialForRendering.SetVector(RADII, corners);
            
            materialForRendering.SetColor(FILL_COLOR, _fillEnabled ? _fillColor : Color.clear);
            materialForRendering.SetTexture(FILL_TEXTURE, sprite != null ? sprite.texture : null);
            
            materialForRendering.SetFloat(OUTLINE, _outlineEnabled ? _outline : 0.0f);
            materialForRendering.SetColor(OUTLINE_COLOR, _outlineColor);
            materialForRendering.SetTexture(OUTLINE_TEXTURE, _outlineSprite != null ? _outlineSprite.texture : null);
            
            materialForRendering.SetVector(SIZE, size);
            materialForRendering.SetFloat(SOFTNESS, _softness);
            materialForRendering.SetColor(COLOR, color);
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateValues();
        }
        #endif
        
        private void ValidateValues()
        {
            var rect = rectTransform.rect;
            
            if (rect.width < 0.001f || rect.height < 0.001f) return;
            
            var max = Mathf.Min(rect.width, rect.height);
            var halfMax = max / 2.0f;

            _softness = Mathf.Clamp(_softness, 0.0f, max);
            _outline = Mathf.Clamp(_outline, 0.0f, halfMax);
            _tlRadius = Mathf.Clamp(_tlRadius, 0.0f, halfMax);
            _trRadius = Mathf.Clamp(_trRadius, 0.0f, halfMax);
            _blRadius = Mathf.Clamp(_blRadius, 0.0f, halfMax);
            _brRadius = Mathf.Clamp(_brRadius, 0.0f, halfMax);
        }
    }
}