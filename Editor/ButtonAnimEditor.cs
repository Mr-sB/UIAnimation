using UnityEditor;
using UnityEditor.AnimatedValues;

namespace GameUtil.Editor
{
    [CustomEditor(typeof(ButtonAnim)), CanEditMultipleObjects]
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
                switch (iterator.propertyPath)
                {
                    case "m_Script":
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.PropertyField(iterator, true);
                        break;
                    case nameof(ButtonAnim.NeedSecondUpSetting):
                        EditorGUILayout.PropertyField(iterator, true);
                        mShowSecondUpSetting.target = iterator.boolValue;
                        break;
                    case nameof(ButtonAnim.SecondUpSetting):
                    {
                        if (EditorGUILayout.BeginFadeGroup(mShowSecondUpSetting.faded))
                            EditorGUILayout.PropertyField(iterator, true);
                        EditorGUILayout.EndFadeGroup();
                        break;
                    }
                    default:
                        EditorGUILayout.PropertyField(iterator, true);
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}