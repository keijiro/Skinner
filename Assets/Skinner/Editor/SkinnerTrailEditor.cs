using UnityEngine;
using UnityEditor;

namespace Skinner
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkinnerTrail))]
    public class SkinnerTrailEditor : Editor
    {
        SerializedProperty _source;
        SerializedProperty _template;
        SerializedProperty _speedLimit;
        SerializedProperty _drag;
        SerializedProperty _randomSeed;

        void OnEnable()
        {
            _source = serializedObject.FindProperty("_source");
            _template = serializedObject.FindProperty("_template");
            _speedLimit = serializedObject.FindProperty("_speedLimit");
            _drag = serializedObject.FindProperty("_drag");
            _randomSeed = serializedObject.FindProperty("_randomSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool reconfigured = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_source);
            EditorGUILayout.PropertyField(_template);
            reconfigured |= EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(_speedLimit);
            EditorGUILayout.PropertyField(_drag);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_randomSeed);
            reconfigured |= EditorGUI.EndChangeCheck();

            if (reconfigured)
                foreach (SkinnerTrail st in targets) st.UpdateConfiguration();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
