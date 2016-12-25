using UnityEngine;
using UnityEditor;

namespace Skinner
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkinnerSource))]
    public class SkinnerSourceEditor : Editor
    {
        SerializedProperty _model;

        void OnEnable()
        {
            _model = serializedObject.FindProperty("_model");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_model);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
