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

            EditorGUILayout.PropertyField(_source);
            EditorGUILayout.PropertyField(_velocityScale);
            EditorGUILayout.PropertyField(_historyLength);
            EditorGUILayout.PropertyField(_randomSeed);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
