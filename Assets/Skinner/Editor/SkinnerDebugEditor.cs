using UnityEngine;
using UnityEditor;

namespace Skinner
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkinnerDebug))]
    public class SkinnerDebugEditor : Editor
    {
        SerializedProperty _source;

        void OnEnable()
        {
            _source = serializedObject.FindProperty("_source");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_source);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
