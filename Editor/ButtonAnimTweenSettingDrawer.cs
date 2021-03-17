using UnityEditor;
using UnityEngine;

namespace GameUtil.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAnim.TweenSetting))]
    public class ButtonAnimTweenSettingDrawer : PropertyDrawerBase
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(position);

            if (!PropertyField(property, label)) return;

            var duration = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Duration));
            var useCurve = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.UseCurve));
            var curve = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Curve));
            var easeType = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.EaseType));
            var scale = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Scale));

            EditorGUI.indentLevel++;
            PropertyField(duration);
            PropertyField(useCurve);
            PropertyField(useCurve.boolValue ? curve : easeType);
            PropertyField(scale);
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            mAllHeight = 0;
            AddPropertyHeight(property);
            if(!property.isExpanded) return mAllHeight - 2;
            
            var duration = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Duration));
            var useCurve = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.UseCurve));
            var curve = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Curve));
            var easeType = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.EaseType));
            var scale = property.FindPropertyRelative(nameof(ButtonAnim.TweenSetting.Scale));

            AddPropertyHeight(duration);
            AddPropertyHeight(useCurve);
            AddPropertyHeight(useCurve.boolValue ? curve : easeType);
            AddPropertyHeight(scale);
            return mAllHeight - 2;
        }
    }
}