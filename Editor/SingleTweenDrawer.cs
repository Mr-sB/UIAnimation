using UnityEditor;
using UnityEngine;

namespace GameUtil.Editor
{
    [CustomPropertyDrawer(typeof(SingleTween))]
    public class SingleTweenDrawer : PropertyDrawer
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
            
            var delay = property.FindPropertyRelative(nameof(SingleTween.Delay));
            var duration = property.FindPropertyRelative(nameof(SingleTween.Duration));
            var useCurve = property.FindPropertyRelative(nameof(SingleTween.UseCurve));
            var curve = property.FindPropertyRelative(nameof(SingleTween.Curve));
            var easeType = property.FindPropertyRelative(nameof(SingleTween.EaseType));
            var mode = property.FindPropertyRelative(nameof(SingleTween.Mode));
            var startPos = property.FindPropertyRelative(nameof(SingleTween.StartPos));
            var endPos = property.FindPropertyRelative(nameof(SingleTween.EndPos));
            var startScale = property.FindPropertyRelative(nameof(SingleTween.StartScale));
            var endScale = property.FindPropertyRelative(nameof(SingleTween.EndScale));
            var startAlpha = property.FindPropertyRelative(nameof(SingleTween.StartAlpha));
            var endAlpha = property.FindPropertyRelative(nameof(SingleTween.EndAlpha));

            EditorGUI.indentLevel++;
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, delay);
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, duration);
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, useCurve);
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, useCurve.boolValue ? curve : easeType);
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, mode);
            switch ((SingleTween.TweenType)mode.intValue)
            {
                case SingleTween.TweenType.MoveTween:
                    position = GetSingleLine(position);
                    EditorGUI.PropertyField(position, startPos);
                    position = GetSingleLine(position);
                    EditorGUI.PropertyField(position, endPos);
                    break;
                case SingleTween.TweenType.ScaleTween:
                    position = GetSingleLine(position);
                    EditorGUI.PropertyField(position, startScale);
                    position = GetSingleLine(position);
                    EditorGUI.PropertyField(position, endScale);
                    break;
                case SingleTween.TweenType.ImageTween:
                case SingleTween.TweenType.TextTween:
                case SingleTween.TweenType.TextMeshPropTween:
                case SingleTween.TweenType.CanvasTween:
                    position = GetSingleLine(position);
                    EditorGUI.PropertyField(position, startAlpha);
                    position = GetSingleLine(position);
                    EditorGUI.PropertyField(position, endAlpha);
                    break;
            }
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 8 * (EditorGUIUtility.singleLineHeight + 2) - 2 : EditorGUIUtility.singleLineHeight;
        }
    }
}
