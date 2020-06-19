using System;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace GameUtil.Editor
{
    [CustomEditor(typeof(ButtonAnim))]
    public class ButtonAnimEditor : UnityEditor.Editor
    {
        private AnimBool mShowSecondUpSetting = new AnimBool();

        private void OnEnable()
        {
            mShowSecondUpSetting.value = serializedObject.FindProperty("NeedSecondUpSetting").boolValue;
            mShowSecondUpSetting.valueChanged.AddListener(Repaint);
        }

        private void OnDisable()
        {
            mShowSecondUpSetting.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ScaleTransform"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Button"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RelativeScale"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DownSetting"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UpSetting"), true);
            var needSecondUpSettingProperty = serializedObject.FindProperty("NeedSecondUpSetting");
            EditorGUILayout.PropertyField(needSecondUpSettingProperty, true);
            mShowSecondUpSetting.target = needSecondUpSettingProperty.boolValue;
            if (EditorGUILayout.BeginFadeGroup(mShowSecondUpSetting.faded))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SecondUpSetting"), true);
            EditorGUILayout.EndFadeGroup();
            
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}