using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Skinner
{
    /// Template mesh asset used in SkinnerGlitch.
    public class SkinnerGlitchTemplate : ScriptableObject
    {
        // Basically this is only a static mesh object that's filled with null
        // triangles. The vertex positions will be dynamically modified in the
        // SkinnerGlitch vertex shader.

        #region Public properties

        public Mesh mesh { get { return _mesh; } }

        [SerializeField] Mesh _mesh;

        #endregion

        #region Public methods

        #if UNITY_EDITOR

        public void RebuildMesh()
        {
            _mesh.Clear();

            // Fill the vertex array with zero.
            const int vcount = (65536 / 3) * 3;
            _mesh.vertices = new Vector3[vcount];

            // Hashed texcoord array
            // .x = hash of the current vertex
            // .y = hash of the left-hand neighbor vertex
            // .z = hash of the right-hand neighbor vertex
            // .w = common hash of the triangle
            var uvs = new List<Vector4>();
            for (var i = 0; i < vcount; i += 3)
            {
                float u0 = Random.value; // vertex #0 hash
                float u1 = Random.value; // vertex #1 hash
                float u2 = Random.value; // vertex #2 hash
                float u3 = Random.value; // common hash
                uvs.Add(new Vector4(u0, u1, u2, u3));
                uvs.Add(new Vector4(u1, u2, u0, u3));
                uvs.Add(new Vector4(u2, u0, u1, u3));
            }
            _mesh.SetUVs(0, uvs);

            // Just enumerate all the vertices into the index array.
            _mesh.SetIndices(
                Enumerable.Range(0, vcount).ToArray(),
                MeshTopology.Triangles, 0
            );

            // Finishing
            _mesh.name = "Glitch Template";
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.UploadMeshData(true);
        }

        #endif

        #endregion

        #region ScriptableObject functions

        void OnEnable()
        {
            if (_mesh == null) {
                _mesh = new Mesh();
                _mesh.name = "Skinner Glitch Template";
            }
        }

        #endregion
    }
}
