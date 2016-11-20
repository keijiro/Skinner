using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Skinner
{
    [CustomEditor(typeof(SkinnerTemplate))]
    public class SkinnerTemplateEditor : Editor
    {
        #region Editor functions

        public override void OnInspectorGUI()
        {
        }

        #endregion

        #region Create menu item functions

        static Mesh[] SelectedMeshAssets {
            get {
                var assets = Selection.GetFiltered(typeof(Mesh), SelectionMode.Deep);
                return assets.Select(x => (Mesh)x).ToArray();
            }
        }

        [MenuItem("Assets/Skinner/Convert To Template", true)]
        static bool ValidateAssets()
        {
            return SelectedMeshAssets.Length > 0;
        }

        [MenuItem("Assets/Skinner/Convert To Template")]
        static void ConvertAssets()
        {
            var converted = new List<Object>();

            foreach (var source in SelectedMeshAssets)
            {
                // Destination file path.
                var dirPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(source));
                var assetPath = AssetDatabase.GenerateUniqueAssetPath(dirPath + "/Skinner Template.asset");

                // Create a template asset.
                var asset = ScriptableObject.CreateInstance<SkinnerTemplate>();
                asset.Initialize(source);
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.AddObjectToAsset(asset.mesh, asset);

                converted.Add(asset);
            }

            // Save the generated assets.
            AssetDatabase.SaveAssets();

            // Select the generated assets.
            EditorUtility.FocusProjectWindow();
            Selection.objects = converted.ToArray();
        }

        #endregion
    }
}
