using UnityEngine;
using UnityEditor;
using System.IO;

namespace Skinner
{
    [CustomEditor(typeof(SkinnerTrailTemplate))]
    public class SkinnerTrailTemplateEditor : Editor
    {
        #region Editor functions

        SerializedProperty _historyLength;

        const string _helpText = 
            "The Skinner Trail renderer tries to draw trail lines as many " +
            "as possible in a single draw call, and thus the number of " +
            "lines is automatically determined from the history length.";

        void OnEnable()
        {
            _historyLength = serializedObject.FindProperty("_historyLength");
        }

        public override void OnInspectorGUI()
        {
            var template = (SkinnerTrailTemplate)target;

            // Editable properties
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_historyLength);
            var rebuild = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            // Readonly members
            EditorGUILayout.LabelField("Line Count", template.lineCount.ToString());
            EditorGUILayout.HelpBox(_helpText, MessageType.None);

            // Rebuild the template mesh when the properties are changed.
            if (rebuild) template.RebuildMesh();
        }

        #endregion

        #region Create menu item functions

        [MenuItem("Assets/Create/Skinner/Trail Template")]
        public static void CreateTemplateAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(path), "");
            var assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Skinner Trail Template.asset");

            // Create a template asset.
            var asset = ScriptableObject.CreateInstance<SkinnerTrailTemplate>();
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
