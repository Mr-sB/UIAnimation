using DG.Tweening;
using UnityEditor;
using UnityEditorInternal;

namespace GameUtil.Editor
{
    [CustomEditor(typeof(UIDOTween)), CanEditMultipleObjects]
    public class UIDOTweenEditor : UnityEditor.Editor
    {
        private const float ELEMENT_OFFSET_X = 7;

        private const float ELEMENT_OFFSET_Y =
#if UNITY_2020_1_OR_NEWER
                0
#elif UNITY_2019_4_OR_NEWER
                1
#else
                2
#endif
            ;
        private ReorderableList mStartSingleTweenReorderableList;
        private ReorderableList mCloseSingleTweenReorderableList;

        private void OnEnable()
        {
            //StartSingleTweenReorderableList
            mStartSingleTweenReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(UIDOTween.StartSingleTweens)));
            mStartSingleTweenReorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, mStartSingleTweenReorderableList.serializedProperty.displayName);
            };
            mStartSingleTweenReorderableList.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.x += ELEMENT_OFFSET_X;
                rect.width -= ELEMENT_OFFSET_X;
                rect.y += ELEMENT_OFFSET_Y;
                EditorGUI.PropertyField(rect, mStartSingleTweenReorderableList.serializedProperty.GetArrayElementAtIndex(index));
            };
            mStartSingleTweenReorderableList.elementHeightCallback = index =>
                EditorGUI.GetPropertyHeight(mStartSingleTweenReorderableList.serializedProperty.GetArrayElementAtIndex(index)) + ELEMENT_OFFSET_Y;
            mStartSingleTweenReorderableList.onAddCallback = OnAddCallback;
            
            //CloseSingleTweenReorderableList
            mCloseSingleTweenReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(UIDOTween.CloseSingleTweens)));
            mCloseSingleTweenReorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, mCloseSingleTweenReorderableList.serializedProperty.displayName);
            };
            mCloseSingleTweenReorderableList.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.x += ELEMENT_OFFSET_X;
                rect.width -= ELEMENT_OFFSET_X;
                rect.y += ELEMENT_OFFSET_Y;
                EditorGUI.PropertyField(rect, mCloseSingleTweenReorderableList.serializedProperty.GetArrayElementAtIndex(index));
            };
            mCloseSingleTweenReorderableList.elementHeightCallback = index =>
                EditorGUI.GetPropertyHeight(mCloseSingleTweenReorderableList.serializedProperty.GetArrayElementAtIndex(index)) + ELEMENT_OFFSET_Y;
            mCloseSingleTweenReorderableList.onAddCallback = OnAddCallback;
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
                    case nameof(UIDOTween.StartSingleTweens):
                        mStartSingleTweenReorderableList.DoLayoutList();
                        break;
                    case nameof(UIDOTween.CloseSingleTweens):
                        mCloseSingleTweenReorderableList.DoLayoutList();
                        break;
                    default:
                        EditorGUILayout.PropertyField(iterator, true);
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        private static void OnAddCallback(ReorderableList list)
        {
            //Add at last
            list.serializedProperty.InsertArrayElementAtIndex(list.count);
            //Is first element, change EaseType to Ease.OutQuad(default)
            if (list.count == 1)
                list.serializedProperty.GetArrayElementAtIndex(0).FindPropertyRelative(nameof(SingleTween.EaseType)).intValue = (int) Ease.OutQuad;
        }
    }
}