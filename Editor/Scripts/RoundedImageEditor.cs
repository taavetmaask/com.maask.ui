using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;

namespace Maask.UI.Editor
{
    [CustomEditor(typeof(RoundedImage))]
    [CanEditMultipleObjects]
    public class RoundedImageEditor : GraphicEditor
    {
        private SerializedProperty _unified;
        private SerializedProperty _tl;
        private SerializedProperty _tr;
        private SerializedProperty _bl;
        private SerializedProperty _br;

        private AnimBool _fillGroup;
        private SerializedProperty _fillEnabled;
        private SerializedProperty _fillColor;
        private SerializedProperty _fillSprite;
        
        private AnimBool _outlineGroup;
        private SerializedProperty _outlineEnabled;
        private SerializedProperty _outline;
        private SerializedProperty _outlineColor;
        private SerializedProperty _outlineSprite;
        
        private SerializedProperty _softness;
        private SerializedProperty _color;
        private SerializedProperty _material;
        
        private Texture2D _radiusTlIcon;
        private Texture2D _radiusTrIcon;
        private Texture2D _radiusBlIcon;
        private Texture2D _radiusBrIcon;

        [MenuItem("GameObject/UI/Rounded Image")]
        private static void AddRoundedImage(MenuCommand menuCommand)
        {
            var go = new GameObject("Rounded Image");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            go.AddComponent<RoundedImage>();
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
            Selection.activeObject = go;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            _unified = serializedObject.FindProperty("_unified");
            _tl = serializedObject.FindProperty("_tlRadius");
            _tr = serializedObject.FindProperty("_trRadius");
            _bl = serializedObject.FindProperty("_blRadius");
            _br = serializedObject.FindProperty("_brRadius");
            
            _fillEnabled = serializedObject.FindProperty("_fillEnabled");
            _fillColor = serializedObject.FindProperty("_fillColor");
            _fillSprite = serializedObject.FindProperty("m_Sprite");
            
            _fillGroup = new AnimBool(true) { target = _fillEnabled.boolValue, value = _fillEnabled.boolValue };
            _fillGroup.valueChanged.AddListener(Repaint);
            
            _outlineEnabled = serializedObject.FindProperty("_outlineEnabled");
            _outline = serializedObject.FindProperty("_outline");
            _outlineColor = serializedObject.FindProperty("_outlineColor");
            _outlineSprite = serializedObject.FindProperty("_outlineSprite");
            
            _outlineGroup = new AnimBool(true) { target = _outlineEnabled.boolValue, value = _outlineEnabled.boolValue };
            _outlineGroup.valueChanged.AddListener(Repaint);
            
            _softness = serializedObject.FindProperty("_softness");
            _color = serializedObject.FindProperty("m_Color");
            _material = serializedObject.FindProperty("m_Material");
            
            _radiusTlIcon = EditorGUIUtility.Load("Packages/com.maask.UI/Editor/Icons/radius-tl.png") as Texture2D;
            _radiusTrIcon = EditorGUIUtility.Load("Packages/com.maask.UI/Editor/Icons/radius-tr.png") as Texture2D;
            _radiusBlIcon = EditorGUIUtility.Load("Packages/com.maask.UI/Editor/Icons/radius-bl.png") as Texture2D;
            _radiusBrIcon = EditorGUIUtility.Load("Packages/com.maask.UI/Editor/Icons/radius-br.png") as Texture2D;
        }

        protected override void OnDisable()
        {
            _fillGroup.valueChanged.RemoveAllListeners();
            _outlineGroup.valueChanged.RemoveAllListeners();
            
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            RadiusGUI();
            EditorGUILayout.Space(6.0f);
            FillGUI();
            EditorGUILayout.Space(6.0f);
            OutlineGUI();
            EditorGUILayout.Space(6.0f);
            SettingsGUI();

            serializedObject.ApplyModifiedProperties();
        }
        
        #region Radii GUI
        private void RadiusGUI()
        {            
            UnifiedHeaderGUI();

            var unified = _unified.boolValue || _unified.hasMultipleDifferentValues;
            
            EditorGUILayout.BeginHorizontal();
            TopLeftRadiusGUI();
            TopRightRadiusGUI(!unified);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            BottomLeftRadiusGUI(!unified);
            BottomRightRadiusGUI(!unified);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);
        }

        private void UnifiedHeaderGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _unified.hasMultipleDifferentValues;
            
            var newUnified = EditorGUILayout.ToggleLeft("Unified Corner Radius", _unified.boolValue);
            
            if (EditorGUI.EndChangeCheck())
            {
                _unified.boolValue = newUnified;
            }
            
            EditorGUI.showMixedValue = false;
            EditorGUILayout.Space(2.0f);
        }

        private void TopLeftRadiusGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _tl.hasMultipleDifferentValues;
            
            var value = RadiusFieldLeftGUI(_tl.floatValue, _tl.floatValue, _radiusTlIcon, true);
            
            EditorGUI.showMixedValue = false;

            if (!EditorGUI.EndChangeCheck()) return;

            _tl.floatValue = value;
        }

        private void TopRightRadiusGUI(bool enabled)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _tr.hasMultipleDifferentValues;
            
            var value = RadiusFieldRightGUI(_tr.floatValue, _tl.floatValue, _radiusTrIcon, enabled);
            
            EditorGUI.showMixedValue = false;

            if (!EditorGUI.EndChangeCheck()) return;

            _tr.floatValue = value;
        }
        
        private void BottomLeftRadiusGUI(bool enabled)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _bl.hasMultipleDifferentValues;
            
            var value = RadiusFieldLeftGUI(_bl.floatValue, _tl.floatValue, _radiusBlIcon, enabled);
            
            EditorGUI.showMixedValue = false;

            if (!EditorGUI.EndChangeCheck()) return;

            _bl.floatValue = value;
        }
        
        private void BottomRightRadiusGUI(bool enabled)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _br.hasMultipleDifferentValues;

            var value = RadiusFieldRightGUI(_br.floatValue, _tl.floatValue, _radiusBrIcon, enabled);
            
            EditorGUI.showMixedValue = false;

            if (!EditorGUI.EndChangeCheck()) return;

            _br.floatValue = value;
        }
        
        private static float RadiusFieldLeftGUI(float value, float altValue, Texture icon, bool enabled)
        {
            GUI.enabled = enabled;
            var area = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            var drag = new Rect(area.x + area.width, area.y, 18, area.height);
            value = DraggableFloatFieldGUI(value, altValue, area, drag, enabled);
            GUILayout.Box(icon, EditorStyles.label, GUILayout.Width(18), GUILayout.Height(18));
            GUI.enabled = true;
            return value;
        }

        private static float RadiusFieldRightGUI(float value, float altValue, Texture icon, bool enabled)
        {
            GUI.enabled = enabled;
            GUILayout.Box(icon, EditorStyles.label, GUILayout.Width(18), GUILayout.Height(18));
            var br = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            var brDrag = new Rect(br.x - 18, br.y, 18, br.height);
            value = DraggableFloatFieldGUI(value, altValue, br, brDrag, enabled);
            GUI.enabled = true;
            return value;
        }

        private static float DraggableFloatFieldGUI(float value, float altValue, Rect area, Rect dragZone, bool enabled)
        {
            if (enabled)
            {
                return EditorGUIHelpers.DraggableFloatField(area, value, dragZone);
            }
            
            EditorGUIHelpers.DraggableFloatField(area, altValue, dragZone);
            return value;
        }
        #endregion
        
        private void FillGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _fillEnabled.hasMultipleDifferentValues;
            
            _fillGroup.target = EditorGUILayout.ToggleLeft("Enable Fill", _fillGroup.target) || _fillEnabled.hasMultipleDifferentValues;
            
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                _fillEnabled.boolValue = _fillGroup.target;
            }
            
            if (EditorGUILayout.BeginFadeGroup(_fillGroup.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_fillSprite, label: new GUIContent("Fill Sprite"));
                EditorGUILayout.PropertyField(_fillColor);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndFadeGroup();
        }
        
        private void OutlineGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _outlineEnabled.hasMultipleDifferentValues;
            
            _outlineGroup.target = EditorGUILayout.ToggleLeft("Enable Outline", _outlineGroup.target) || _outlineEnabled.hasMultipleDifferentValues;
            
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                _outlineEnabled.boolValue = _outlineGroup.target;
            }

            if (EditorGUILayout.BeginFadeGroup(_outlineGroup.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_outline);
                EditorGUILayout.PropertyField(_outlineSprite);
                EditorGUILayout.PropertyField(_outlineColor);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndFadeGroup();
        }

        private void SettingsGUI()
        {
            EditorGUILayout.PropertyField(_softness);
            EditorGUILayout.PropertyField(_color);
            EditorGUILayout.PropertyField(_material);
        }
    }
}