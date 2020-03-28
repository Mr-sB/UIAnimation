using UnityEditor;
using UnityEngine;

namespace GameUtil.Editor
{
    [CustomPropertyDrawer(typeof(SingleTween))]
    public class SingleTweenDrawer : PropertyDrawerBase
    {
        public override void OnGUI(Rect position,  SerializedProperty property, GUIContent label)
        {
            Init(position);
            
            if (!PropertyField(property, label, false)) return;
            
            var isDelay = property.FindPropertyRelative(nameof(SingleTween.IsDelay));
            var delay = property.FindPropertyRelative(nameof(SingleTween.Delay));

            EditorGUI.indentLevel++;
            PropertyField(isDelay);
            if (isDelay.boolValue)
            {
                PropertyField(delay);
            }
            else
            {
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
                
                PropertyField(duration);
                PropertyField(useCurve);
                PropertyField(useCurve.boolValue ? curve : easeType);
                PropertyField(tweenerLinkType);
                if ((SingleTween.LinkType) tweenerLinkType.intValue == SingleTween.LinkType.Insert)
                {
                    PropertyField(atPosition);
                }

                PropertyField(mode);
                
                switch ((SingleTween.TweenType) mode.intValue)
                {
                    case SingleTween.TweenType.Move:
                        PropertyField(transform);
                        PropertyField(overrideStartStatus);
                        if (overrideStartStatus.boolValue)
                        {
                            PropertyField(startPos);
                            EditorGUI.GetPropertyHeight(startPos, true);
                        }
                        PropertyField(endPos);
                        break;
                    case SingleTween.TweenType.Scale:
                        PropertyField(transform);
                        PropertyField(overrideStartStatus);
                        if (overrideStartStatus.boolValue)
                        {
                            PropertyField(startScale);
                        }
                        PropertyField(endScale);
                        break;
                    case SingleTween.TweenType.Image:
                        PropertyField(image);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Text:
                        PropertyField(text);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.TextMeshProp:
                        PropertyField(textMeshProUGUI);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Canvas:
                        PropertyField(canvasGroup);
                        DrawAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                }
            }
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            mAllHeight = 0;
            AddPropertyHeight(property);
            if(!property.isExpanded) return mAllHeight - 2;
            
            var isDelay = property.FindPropertyRelative(nameof(SingleTween.IsDelay));
            var delay = property.FindPropertyRelative(nameof(SingleTween.Delay));

            AddPropertyHeight(isDelay);
            if (isDelay.boolValue)
            {
                AddPropertyHeight(delay);
            }
            else
            {
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
                
                AddPropertyHeight(duration);
                AddPropertyHeight(useCurve);
                AddPropertyHeight(useCurve.boolValue ? curve : easeType);
                AddPropertyHeight(tweenerLinkType);
                if ((SingleTween.LinkType) tweenerLinkType.intValue == SingleTween.LinkType.Insert)
                {
                    AddPropertyHeight(atPosition);
                }

                AddPropertyHeight(mode);
                
                switch ((SingleTween.TweenType) mode.intValue)
                {
                    case SingleTween.TweenType.Move:
                        AddPropertyHeight(transform);
                        AddPropertyHeight(overrideStartStatus);
                        if (overrideStartStatus.boolValue)
                        {
                            AddPropertyHeight(startPos);
                            EditorGUI.GetPropertyHeight(startPos, true);
                        }
                        AddPropertyHeight(endPos);
                        break;
                    case SingleTween.TweenType.Scale:
                        AddPropertyHeight(transform);
                        AddPropertyHeight(overrideStartStatus);
                        if (overrideStartStatus.boolValue)
                        {
                            AddPropertyHeight(startScale);
                        }
                        AddPropertyHeight(endScale);
                        break;
                    case SingleTween.TweenType.Image:
                        AddPropertyHeight(image);
                        AddAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Text:
                        AddPropertyHeight(text);
                        AddAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.TextMeshProp:
                        AddPropertyHeight(textMeshProUGUI);
                        AddAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Canvas:
                        AddPropertyHeight(canvasGroup);
                        AddAlpha(overrideStartStatus, startAlpha, endAlpha);
                        break;
                }
            }
            return mAllHeight - 2;
        }

        private void DrawAlpha(SerializedProperty overrideStartStatus, SerializedProperty startAlpha, SerializedProperty endAlpha)
        {
            PropertyField(overrideStartStatus);
            if (overrideStartStatus.boolValue)
                PropertyField(startAlpha);
            PropertyField(endAlpha);
        }
        
        private void AddAlpha(SerializedProperty overrideStartStatus, SerializedProperty startAlpha, SerializedProperty endAlpha)
        {
            AddPropertyHeight(overrideStartStatus);
            if (overrideStartStatus.boolValue)
                AddPropertyHeight(startAlpha);
            AddPropertyHeight(endAlpha);
        }
    }
}
