using UnityEditor;
using UnityEngine;

namespace GameUtil.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAnim.TweenSetting))]
    public class ButtonAnimTweenSettingDrawer : PropertyDrawer
    {
        private static Rect GetSingleLine(Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight + 2;
            position.height = EditorGUIUtility.singleLineHeight;
            return position;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.y -= EditorGUIUtility.singleLineHeight + 2;
            
            position = GetSingleLine(position);
            if (!EditorGUI.PropertyField(position, property, label, false)) return;

            var duration = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Duration));
            var useCurve = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.UseCurve));
            var curve = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Curve));
            var easeType = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.EaseType));
            var scale = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Scale));

            EditorGUI.indentLevel++;
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, duration);
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, useCurve);
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, useCurve.boolValue ? curve : easeType);
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, scale);
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 5 * (EditorGUIUtility.singleLineHeight + 2) - 2 : EditorGUIUtility.singleLineHeight;
        }
    }
}