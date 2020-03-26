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
            
            var isDelay = property.FindPropertyRelative(nameof(SingleTween.IsDelay));
            var delay = property.FindPropertyRelative(nameof(SingleTween.Delay));
            var duration = property.FindPropertyRelative(nameof(SingleTween.Duration));
            var useCurve = property.FindPropertyRelative(nameof(SingleTween.UseCurve));
            var curve = property.FindPropertyRelative(nameof(SingleTween.Curve));
            var easeType = property.FindPropertyRelative(nameof(SingleTween.EaseType));
            var tweenerLinkType = property.FindPropertyRelative(nameof(SingleTween.TweenerLinkType));
            var atPosition = property.FindPropertyRelative(nameof(SingleTween.AtPosition));
            var mode = property.FindPropertyRelative(nameof(SingleTween.Mode));
            var transform = property.FindPropertyRelative(nameof(SingleTween.Transform));
            var image = property.FindPropertyRelative(nameof(SingleTween.Image));
            var text = property.FindPropertyRelative(nameof(SingleTween.Text));
            var textMeshProUGUI = property.FindPropertyRelative(nameof(SingleTween.TextMeshProUGUI));
            var canvasGroup = property.FindPropertyRelative(nameof(SingleTween.CanvasGroup));
            var overrideStartStatus = property.FindPropertyRelative(nameof(SingleTween.OverrideStartStatus));
            var startPos = property.FindPropertyRelative(nameof(SingleTween.StartPos));
            var endPos = property.FindPropertyRelative(nameof(SingleTween.EndPos));
            var startScale = property.FindPropertyRelative(nameof(SingleTween.StartScale));
            var endScale = property.FindPropertyRelative(nameof(SingleTween.EndScale));
            var startAlpha = property.FindPropertyRelative(nameof(SingleTween.StartAlpha));
            var endAlpha = property.FindPropertyRelative(nameof(SingleTween.EndAlpha));

            EditorGUI.indentLevel++;
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, isDelay);
            if (isDelay.boolValue)
            {
                position = GetSingleLine(position);
                EditorGUI.PropertyField(position, delay);
            }
            else
            {
                position = GetSingleLine(position);
                EditorGUI.PropertyField(position, duration);
                position = GetSingleLine(position);
                EditorGUI.PropertyField(position, useCurve);
                position = GetSingleLine(position);
                EditorGUI.PropertyField(position, useCurve.boolValue ? curve : easeType);
                position = GetSingleLine(position);
                EditorGUI.PropertyField(position, tweenerLinkType);
                if ((SingleTween.LinkType) tweenerLinkType.intValue == SingleTween.LinkType.Insert)
                {
                    position = GetSingleLine(position);
                    EditorGUI.PropertyField(position, atPosition);
                }

                position = GetSingleLine(position);
                EditorGUI.PropertyField(position, mode);
                
                switch ((SingleTween.TweenType) mode.intValue)
                {
                    case SingleTween.TweenType.MoveTween:
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, transform);
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, overrideStartStatus);
                        if (overrideStartStatus.boolValue)
                        {
                            position = GetSingleLine(position);
                            EditorGUI.PropertyField(position, startPos);
                        }
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, endPos);
                        break;
                    case SingleTween.TweenType.ScaleTween:
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, transform);
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, overrideStartStatus);
                        if (overrideStartStatus.boolValue)
                        {
                            position = GetSingleLine(position);
                            EditorGUI.PropertyField(position, startScale);
                        }
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, endScale);
                        break;
                    case SingleTween.TweenType.ImageTween:
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, image);
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, overrideStartStatus);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha, position);
                        break;
                    case SingleTween.TweenType.TextTween:
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, text);
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, overrideStartStatus);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha, position);
                        break;
                    case SingleTween.TweenType.TextMeshPropTween:
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, textMeshProUGUI);
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, overrideStartStatus);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha, position);
                        break;
                    case SingleTween.TweenType.CanvasTween:
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, canvasGroup);
                        position = GetSingleLine(position);
                        EditorGUI.PropertyField(position, overrideStartStatus);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha, position);
                        break;
                }
            }
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            if (property.FindPropertyRelative(nameof(SingleTween.IsDelay)).boolValue) return 3 * (EditorGUIUtility.singleLineHeight + 2) - 2;
            int line = 10;
            if ((SingleTween.LinkType) property.FindPropertyRelative(nameof(SingleTween.TweenerLinkType)).intValue ==
                SingleTween.LinkType.Insert)
                line++;
            if (property.FindPropertyRelative(nameof(SingleTween.OverrideStartStatus)).boolValue)
                line++;
            return line * (EditorGUIUtility.singleLineHeight + 2) - 2;
        }

        private static void DrawAlpha(SerializedProperty overrideStartStatus, SerializedProperty startAlpha, SerializedProperty endAlpha, Rect position)
        {
            if (overrideStartStatus.boolValue)
            {
                position = GetSingleLine(position);
                EditorGUI.PropertyField(position, startAlpha);
            }
            position = GetSingleLine(position);
            EditorGUI.PropertyField(position, endAlpha);
        }
    }
}
