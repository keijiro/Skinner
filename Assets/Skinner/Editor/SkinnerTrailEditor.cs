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

        SerializedProperty _cutoffSpeed;
        SerializedProperty _speedToWidth;
        SerializedProperty _maxWidth;

        SerializedProperty _randomSeed;

        static GUIContent _labelSensitivity = new GUIContent("Sensitivity");

        void OnEnable()
        {
            _source = serializedObject.FindProperty("_source");
            _template = serializedObject.FindProperty("_template");

            _speedLimit = serializedObject.FindProperty("_speedLimit");
            _drag = serializedObject.FindProperty("_drag");

            _cutoffSpeed = serializedObject.FindProperty("_cutoffSpeed");
            _speedToWidth = serializedObject.FindProperty("_speedToWidth");
            _maxWidth = serializedObject.FindProperty("_maxWidth");

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

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_speedLimit);
            EditorGUILayout.PropertyField(_drag);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Line Width Modifier (By Speed)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_cutoffSpeed);
            EditorGUILayout.PropertyField(_speedToWidth, _labelSensitivity);
            EditorGUILayout.PropertyField(_maxWidth);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_randomSeed);
            reconfigured |= EditorGUI.EndChangeCheck();

            if (reconfigured)
                foreach (SkinnerTrail st in targets) st.UpdateConfiguration();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
