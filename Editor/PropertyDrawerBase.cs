using UnityEditor;
using UnityEngine;

namespace GameUtil.Editor
{
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        protected Rect mPosition;
        protected float mLastHeight;
        protected float mAllHeight;

        protected void Init(Rect position)
        {
            mPosition = position;
            mLastHeight = -2;
        }

        protected Rect GetRect(float height)
        {
            mPosition.y += mLastHeight + 2;//add 2 pixel height
            mLastHeight = height;
            mPosition.height = mLastHeight;
            return mPosition;
        }
        
        protected Rect GetPropertyRect(SerializedProperty property, bool includeChildren = false)
        {
            return GetRect(EditorGUI.GetPropertyHeight(property, includeChildren));
        }

        protected bool PropertyField(SerializedProperty property, bool includeChildren = false)
        {
            return EditorGUI.PropertyField(GetPropertyRect(property, includeChildren), property, includeChildren);
        }
        
        protected bool PropertyField(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUI.PropertyField(GetPropertyRect(property, includeChildren), property, label, includeChildren);
        }

        protected void AddPropertyHeight(SerializedProperty property, bool includeChildren = false)
        {
            mAllHeight += EditorGUI.GetPropertyHeight(property, includeChildren) + 2;//add 2 pixel height
        }
    }
}