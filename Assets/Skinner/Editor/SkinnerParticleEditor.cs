using UnityEngine;
using UnityEditor;

namespace Skinner
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkinnerParticle))]
    public class SkinnerParticleEditor : Editor
    {
        SerializedProperty _source;
        SerializedProperty _template;

        SerializedProperty _speedLimit;
        SerializedProperty _drag;
        SerializedProperty _gravity;

        SerializedProperty _speedToLife;
        SerializedProperty _maxLife;

        SerializedProperty _speedToSpin;
        SerializedProperty _maxSpin;

        SerializedProperty _speedToScale;
        SerializedProperty _maxScale;

        SerializedProperty _noiseAmplitude;
        SerializedProperty _noiseFrequency;
        SerializedProperty _noiseMotion;

        SerializedProperty _randomSeed;

        static GUIContent _labelSpeedToLife = new GUIContent("Life by Speed");
        static GUIContent _labelSpeedToSpin = new GUIContent("Spin by Speed");
        static GUIContent _labelSpeedToScale = new GUIContent("Scale by Speed");

        void OnEnable()
        {
            _source = serializedObject.FindProperty("_source");
            _template = serializedObject.FindProperty("_template");

            _speedLimit = serializedObject.FindProperty("_speedLimit");
            _drag = serializedObject.FindProperty("_drag");
            _gravity = serializedObject.FindProperty("_gravity");

            _speedToLife = serializedObject.FindProperty("_speedToLife");
            _maxLife = serializedObject.FindProperty("_maxLife");

            _speedToSpin = serializedObject.FindProperty("_speedToSpin");
            _maxSpin = serializedObject.FindProperty("_maxSpin");

            _speedToScale = serializedObject.FindProperty("_speedToScale");
            _maxScale = serializedObject.FindProperty("_maxScale");

            _noiseAmplitude = serializedObject.FindProperty("_noiseAmplitude");
            _noiseFrequency = serializedObject.FindProperty("_noiseFrequency");
            _noiseMotion = serializedObject.FindProperty("_noiseMotion");

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
            EditorGUILayout.PropertyField(_gravity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_speedToLife, _labelSpeedToLife);
            EditorGUILayout.PropertyField(_maxLife);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_speedToSpin, _labelSpeedToSpin);
            EditorGUILayout.PropertyField(_maxSpin);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_speedToScale, _labelSpeedToScale);
            EditorGUILayout.PropertyField(_maxScale);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_noiseAmplitude);
            EditorGUILayout.PropertyField(_noiseFrequency);
            EditorGUILayout.PropertyField(_noiseMotion);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_randomSeed);
            reconfigured |= EditorGUI.EndChangeCheck();

            if (reconfigured)
                foreach (SkinnerParticle sp in targets) sp.UpdateConfiguration();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
