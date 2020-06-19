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
            mShowSecondUpSetting.value = serializedObject.FindProperty(nameof(ButtonAnim.NeedSecondUpSetting)).boolValue;
            mShowSecondUpSetting.valueChanged.AddListener(Repaint);
        }

        private void OnDisable()
        {
            mShowSecondUpSetting.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    if (iterator.propertyPath == nameof(ButtonAnim.NeedSecondUpSetting))
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                        mShowSecondUpSetting.target = iterator.boolValue;
                    }
                    else if (iterator.propertyPath == nameof(ButtonAnim.SecondUpSetting))
                    {
                        if (EditorGUILayout.BeginFadeGroup(mShowSecondUpSetting.faded))
                            EditorGUILayout.PropertyField(iterator, true);
                        EditorGUILayout.EndFadeGroup();
                    }
                    else
                        EditorGUILayout.PropertyField(iterator, true);
                }
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}