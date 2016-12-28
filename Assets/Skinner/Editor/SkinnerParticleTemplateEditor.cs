using UnityEngine;
using UnityEditor;
using System.IO;

namespace Skinner
{
    [CustomEditor(typeof(SkinnerParticleTemplate))]
    public class SkinnerParticleTemplateEditor : Editor
    {
        #region Editor functions

        SerializedProperty _shapes;
        SerializedProperty _maxInstanceCount;

        const string _helpText = 
            "The Skinner Particle renderer draws all particles in a single " +
            "draw call, and thus the actual number of particle instances is " +
            "limited by the number of vertices in the particle shapes; it " +
            "may be less than the Max Instance Count.";

        void OnEnable()
        {
            _shapes = serializedObject.FindProperty("_shapes");
            _maxInstanceCount = serializedObject.FindProperty("_maxInstanceCount");
        }

        public override void OnInspectorGUI()
        {
            var template = (SkinnerParticleTemplate)target;

            // Editable properties
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_shapes, true);
            EditorGUILayout.PropertyField(_maxInstanceCount);
            var rebuild = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            // Readonly members
            EditorGUILayout.LabelField("Instance Count", template.instanceCount.ToString());
            EditorGUILayout.HelpBox(_helpText, MessageType.None);

            // Rebuild button
            rebuild |= GUILayout.Button("Rebuild");

            // Rebuild the template mesh when the properties are changed.
            if (rebuild) template.RebuildMesh();
        }

        #endregion

        #region Create menu item functions

        [MenuItem("Assets/Create/Skinner/Particle Template")]
        public static void CreateTemplateAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(path), "");
            var assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Skinner Particle Template.asset");

            // Create a template asset.
            var asset = ScriptableObject.CreateInstance<SkinnerParticleTemplate>();
            AssetDatabase.CreateAsset(asset, assetPathName);
            AssetDatabase.AddObjectToAsset(asset.mesh, asset);

            // Build an initial mesh for the asset.
            asset.RebuildMesh();

            // Save the generated mesh asset.
            AssetDatabase.SaveAssets();

            // Tweak the selection.
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        #endregion
    }
}
