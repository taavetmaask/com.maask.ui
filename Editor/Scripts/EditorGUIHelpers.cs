using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Maask.UI
{
    public static class EditorGUIHelpers
    {
        private static readonly Type EDITOR_GUI_TYPE = typeof(EditorGUI);

        private static readonly Type RECYCLED_TEXT_EDITOR_TYPE =
            Assembly.GetAssembly(EDITOR_GUI_TYPE).GetType("UnityEditor.EditorGUI+RecycledTextEditor");

        private static readonly Type[] ARGUMENT_TYPES =
        {
            RECYCLED_TEXT_EDITOR_TYPE, typeof(Rect), typeof(Rect), typeof(int), typeof(float), typeof(string),
            typeof(GUIStyle), typeof(bool)
        };

        private static readonly MethodInfo DO_FLOAT_FIELD_METHOD = EDITOR_GUI_TYPE.GetMethod("DoFloatField",
            BindingFlags.NonPublic | BindingFlags.Static, null, ARGUMENT_TYPES, null);

        private static readonly FieldInfo FIELD_INFO =
            EDITOR_GUI_TYPE.GetField("s_RecycledEditorInternal", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly object RECYCLED_EDITOR = FIELD_INFO.GetValue(null);

        public static float DraggableFloatField(Rect position, float value, Rect dragHotZone)
        {
            return (float)DO_FLOAT_FIELD_METHOD.Invoke(null, new [] {
                RECYCLED_EDITOR, 
                position, 
                dragHotZone, 
                GUIUtility.GetControlID("EditorTextField".GetHashCode(), FocusType.Keyboard, position), 
                value, 
                "g7",
                EditorStyles.numberField, 
                true
            });
        }
    }
}