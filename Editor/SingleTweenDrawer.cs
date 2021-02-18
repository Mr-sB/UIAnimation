using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameUtil.Editor
{
    [CustomPropertyDrawer(typeof(SingleTween))]
    public class SingleTweenDrawer : PropertyDrawerBase
    {
        private const int SWITCH_BUTTON_WIDTH = 75;
        private GUIContent mGUIContent;
        
        public override void OnGUI(Rect position,  SerializedProperty property, GUIContent label)
        {
            Init(position);
            if (mGUIContent == null)
                mGUIContent = new GUIContent();
            var name = property.FindPropertyRelative("Name");
            var hideButton = property.FindPropertyRelative("HideButton");
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
            //Draw element
            var elementPosition = GetPropertyRect(property);
            elementPosition.width -= SWITCH_BUTTON_WIDTH;
            bool expand = EditorGUI.PropertyField(elementPosition, property, mGUIContent, false);
            //Draw hide/show button
            if (!isDelay.boolValue)
            {
                hideButton.boolValue = GUI.Toggle(
                    new Rect(position.xMax - SWITCH_BUTTON_WIDTH, position.y, SWITCH_BUTTON_WIDTH, EditorGUIUtility.singleLineHeight),
                    hideButton.boolValue, "HideButton", GUI.skin.button);
            }

            if (!expand) return;

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
                
                //Draw Buttons
                if (!hideButton.boolValue)
                {
                    GameObject gameObject = (property.serializedObject.targetObject as Component)?.gameObject;
                    var startStatusRect = EditorGUI.IndentedRect(GetRect(EditorGUIUtility.singleLineHeight));
                    var endStatusRect = EditorGUI.IndentedRect(GetRect(EditorGUIUtility.singleLineHeight));

                    var buttonPosition = startStatusRect;
                    buttonPosition.width = (buttonPosition.width - 2) / 2;
                    if (GUI.Button(buttonPosition, nameof(SingleTween.CopyStartStatus)))
                        (GetObject(property) as SingleTween)?.CopyStartStatus(gameObject);
                    buttonPosition.x = buttonPosition.xMax + 2;
                    if (GUI.Button(buttonPosition, nameof(SingleTween.PasteStartStatus)))
                        (GetObject(property) as SingleTween)?.PasteStartStatus(gameObject);

                    buttonPosition = endStatusRect;
                    buttonPosition.width = (buttonPosition.width - 2) / 2;
                    if (GUI.Button(buttonPosition, nameof(SingleTween.CopyEndStatus)))
                        (GetObject(property) as SingleTween)?.CopyEndStatus(gameObject);
                    buttonPosition.x = buttonPosition.xMax + 2;
                    if (GUI.Button(buttonPosition, nameof(SingleTween.PasteEndStatus)))
                        (GetObject(property) as SingleTween)?.PasteEndStatus(gameObject);
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
                var hideButton = property.FindPropertyRelative("HideButton");
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
                
                //Buttons Height
                if (!hideButton.boolValue)
                    mAllHeight += (EditorGUIUtility.singleLineHeight + 2) * 2;
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
        
        
        private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        //根据SerializedProperty查找到其对应的对象引用
        public static object GetObject(SerializedProperty property)
        {
            //从最外层的property.serializedObject.targetObject(继承自UnityEngine.Object)的对象一层一层的找到目前需要绘制的对象
            string[] array = property.propertyPath.Replace("Array.data", "*Array").Split('.');
            object obj = property.serializedObject.targetObject;
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].StartsWith("*Array"))
                    obj = GetFieldInfoIncludeBase(obj.GetType(), array[i]).GetValue(obj);
                else
                {
                    int index = int.Parse(array[i].Substring(7, array[i].Length - 8));
                    obj = (obj as IList)[index];
                }
            }
            return obj;
        }
        
        //由于Type.GetField似乎不会查找到父类的私有字段，所以循环查找一下
        public static FieldInfo GetFieldInfoIncludeBase(Type type, string fieldName, BindingFlags bindingFlags = bindingFlags)
        {
            FieldInfo fieldInfo = null;
            while (fieldInfo == null && type != null)
            {
                fieldInfo = type.GetField(fieldName, bindingFlags);
                type = type.BaseType;
            }
            return fieldInfo;
        }
    }
}
