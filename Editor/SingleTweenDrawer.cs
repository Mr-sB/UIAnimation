using UnityEditor;
using UnityEngine;

namespace GameUtil.Editor
{
    [CustomPropertyDrawer(typeof(SingleTween))]
    public class SingleTweenDrawer : PropertyDrawerBase
    {
        private GUIContent mGUIContent;
        
        public override void OnGUI(Rect position,  SerializedProperty property, GUIContent label)
        {
            Init(position);
            if (mGUIContent == null)
                mGUIContent = new GUIContent();
            var name = property.FindPropertyRelative("Name");
            var isDelay = property.FindPropertyRelative(nameof(SingleTween.IsDelay));
            var delay = property.FindPropertyRelative(nameof(SingleTween.Delay));
            var duration = property.FindPropertyRelative(nameof(SingleTween.Duration));
            var tweenerLinkType = property.FindPropertyRelative(nameof(SingleTween.TweenerLinkType));
            var atPosition = property.FindPropertyRelative(nameof(SingleTween.AtPosition));
            var linkType = (SingleTween.LinkType) tweenerLinkType.intValue;
            mGUIContent.text = name.stringValue + (isDelay.boolValue
                    ? $" Delay duration:{delay.floatValue}"
                    : $" {linkType}{(linkType == SingleTween.LinkType.Insert ? " at:" + atPosition.floatValue : "")} duration:{duration.floatValue}"
                );
            if (!PropertyField(property, mGUIContent, false)) return;

            EditorGUI.indentLevel++;
            PropertyField(name);
            PropertyField(isDelay);
            if (isDelay.boolValue)
            {
                PropertyField(delay);
            }
            else
            {
                var useCurve = property.FindPropertyRelative(nameof(SingleTween.UseCurve));
                var curve = property.FindPropertyRelative(nameof(SingleTween.Curve));
                var easeType = property.FindPropertyRelative(nameof(SingleTween.EaseType));
                var mode = property.FindPropertyRelative(nameof(SingleTween.Mode));
                var transform = property.FindPropertyRelative(nameof(SingleTween.Transform));
                var image = property.FindPropertyRelative(nameof(SingleTween.Image));
                var text = property.FindPropertyRelative(nameof(SingleTween.Text));
                var textMeshProUGUI = property.FindPropertyRelative(nameof(SingleTween.TextMeshProUGUI));
                var canvasGroup = property.FindPropertyRelative(nameof(SingleTween.CanvasGroup));
                var rectTransform = property.FindPropertyRelative(nameof(SingleTween.RectTransform));
                var overrideStartStatus = property.FindPropertyRelative(nameof(SingleTween.OverrideStartStatus));
                var startPos = property.FindPropertyRelative(nameof(SingleTween.StartPos));
                var endPos = property.FindPropertyRelative(nameof(SingleTween.EndPos));
                var startRotation = property.FindPropertyRelative(nameof(SingleTween.StartRotation));
                var endRotation = property.FindPropertyRelative(nameof(SingleTween.EndRotation));
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
                
                var oldTweenType = (SingleTween.TweenType) mode.intValue;
                PropertyField(mode);
                var curTweenType = (SingleTween.TweenType) mode.intValue;
                //Change TweenType, maybe need clear or parse component.
                if (curTweenType != oldTweenType)
                {
                    switch (oldTweenType)
                    {
                        case SingleTween.TweenType.Move:
                        case SingleTween.TweenType.Rotate:
                        case SingleTween.TweenType.Scale:
                            //Do not clear
                            if (curTweenType == SingleTween.TweenType.Move || curTweenType == SingleTween.TweenType.Rotate ||
                                curTweenType == SingleTween.TweenType.Scale)
                                break;
                            //Try parse
                            else if (curTweenType == SingleTween.TweenType.AnchorPos3D)
                            {
                                if (transform.objectReferenceValue is RectTransform rect)
                                    rectTransform.objectReferenceValue = rect;
                            }
                            //Clear
                            transform.objectReferenceValue = null;
                            break;
                        case SingleTween.TweenType.Image:
                            image.objectReferenceValue = null;
                            break;
                        case SingleTween.TweenType.Text:
                            text.objectReferenceValue = null;
                            break;
                        case SingleTween.TweenType.TextMeshProUGUI:
                            textMeshProUGUI.objectReferenceValue = null;
                            break;
                        case SingleTween.TweenType.Canvas:
                            canvasGroup.objectReferenceValue = null;
                            break;
                        case SingleTween.TweenType.AnchorPos3D:
                            //Parse
                            if (curTweenType == SingleTween.TweenType.Move || curTweenType == SingleTween.TweenType.Rotate ||
                                curTweenType == SingleTween.TweenType.Scale)
                                transform.objectReferenceValue = rectTransform.objectReferenceValue;
                            rectTransform.objectReferenceValue = null;
                            break;
                    }
                }
                
                switch ((SingleTween.TweenType) mode.intValue)
                {
                    case SingleTween.TweenType.Move:
                        DrawTweenType(transform, overrideStartStatus, startPos, endPos);
                        break;
                    case SingleTween.TweenType.Rotate:
                        DrawTweenType(transform, overrideStartStatus, startRotation, endRotation);
                        break;
                    case SingleTween.TweenType.Scale:
                        DrawTweenType(transform, overrideStartStatus, startScale, endScale);
                        break;
                    case SingleTween.TweenType.Image:
                        DrawTweenType(image, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Text:
                        DrawTweenType(text, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.TextMeshProUGUI:
                        DrawTweenType(textMeshProUGUI, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Canvas:
                        DrawTweenType(canvasGroup, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.AnchorPos3D:
                        DrawTweenType(rectTransform, overrideStartStatus, startPos, endPos);
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
            
            var name = property.FindPropertyRelative("mName");
            var isDelay = property.FindPropertyRelative(nameof(SingleTween.IsDelay));
            var delay = property.FindPropertyRelative(nameof(SingleTween.Delay));

            AddPropertyHeight(name);
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
                var rectTransform = property.FindPropertyRelative(nameof(SingleTween.RectTransform));
                var overrideStartStatus = property.FindPropertyRelative(nameof(SingleTween.OverrideStartStatus));
                var startPos = property.FindPropertyRelative(nameof(SingleTween.StartPos));
                var endPos = property.FindPropertyRelative(nameof(SingleTween.EndPos));
                var startRotation = property.FindPropertyRelative(nameof(SingleTween.StartRotation));
                var endRotation = property.FindPropertyRelative(nameof(SingleTween.EndRotation));
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
                        AddTweenType(transform, overrideStartStatus, startPos, endPos);
                        break;
                    case SingleTween.TweenType.Rotate:
                        AddTweenType(transform, overrideStartStatus, startRotation, endRotation);
                        break;
                    case SingleTween.TweenType.Scale:
                        AddTweenType(transform, overrideStartStatus, startScale, endScale);
                        break;
                    case SingleTween.TweenType.Image:
                        AddTweenType(image, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Text:
                        AddTweenType(text, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.TextMeshProUGUI:
                        AddTweenType(textMeshProUGUI, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.Canvas:
                        AddTweenType(canvasGroup, overrideStartStatus, startAlpha, endAlpha);
                        break;
                    case SingleTween.TweenType.AnchorPos3D:
                        AddTweenType(rectTransform, overrideStartStatus, startPos, endPos);
                        break;
                }
            }
            return mAllHeight - 2;
        }

        private void DrawTweenType(SerializedProperty component, SerializedProperty overrideStartStatus, SerializedProperty start, SerializedProperty end)
        {
            PropertyField(component);
            PropertyField(overrideStartStatus);
            if (overrideStartStatus.boolValue)
                PropertyField(start);
            PropertyField(end);
        }
        
        private void AddTweenType(SerializedProperty component, SerializedProperty overrideStartStatus, SerializedProperty start, SerializedProperty end)
        {
            AddPropertyHeight(component);
            AddPropertyHeight(overrideStartStatus);
            if (overrideStartStatus.boolValue)
                AddPropertyHeight(start);
            AddPropertyHeight(end);
        }
    }
}
