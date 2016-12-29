using UnityEngine;
using UnityEditor;

namespace Skinner
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkinnerGlitch))]
    public class SkinnerGlitchEditor : Editor
    {
        SerializedProperty _source;
        SerializedProperty _historyLength;
        SerializedProperty _velocityScale;
        SerializedProperty _edgeThreshold;
        SerializedProperty _areaThreshold;
        SerializedProperty _randomSeed;

        void OnEnable()
        {
            _source = serializedObject.FindProperty("_source");
            _historyLength = serializedObject.FindProperty("_historyLength");
            _velocityScale = serializedObject.FindProperty("_velocityScale");
            _edgeThreshold = serializedObject.FindProperty("_edgeThreshold");
            _areaThreshold = serializedObject.FindProperty("_areaThreshold");
            _randomSeed = serializedObject.FindProperty("_randomSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool reconfigured = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_source);
            EditorGUILayout.PropertyField(_historyLength);
            reconfigured |= EditorGUI.EndChangeCheck();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_velocityScale);
            EditorGUILayout.PropertyField(_edgeThreshold);
            EditorGUILayout.PropertyField(_areaThreshold);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_randomSeed);
            reconfigured |= EditorGUI.EndChangeCheck();

            if (reconfigured)
                foreach (SkinnerGlitch sg in targets) sg.UpdateConfiguration();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
