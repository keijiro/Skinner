using UnityEngine;
using UnityEditor;

namespace Skinner
{
    [CustomEditor(typeof(SkinnerGlitch))]
    public class SkinnerGlitchEditor : Editor
    {
        SerializedProperty _source;
        SerializedProperty _velocityScale;
        SerializedProperty _historyLength;
        SerializedProperty _randomSeed;

        void OnEnable()
        {
            _source = serializedObject.FindProperty("_source");
            _velocityScale = serializedObject.FindProperty("_velocityScale");
            _historyLength = serializedObject.FindProperty("_historyLength");
            _randomSeed = serializedObject.FindProperty("_randomSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool reconfigured = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_source);
            reconfigured |= EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(_velocityScale);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_historyLength);
            EditorGUILayout.PropertyField(_randomSeed);
            reconfigured |= EditorGUI.EndChangeCheck();

            if (reconfigured)
                foreach (SkinnerGlitch sg in targets) sg.UpdateConfiguration();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
