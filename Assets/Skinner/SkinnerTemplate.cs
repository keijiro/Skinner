using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Skinner
{
    public class SkinnerTemplate : ScriptableObject
    {
        #region Public properties

        /// Number of vertices (read only)
        public int vertexCount {
            get { return _vertexCount; }
        }

        [SerializeField] int _vertexCount;

        /// Tmplate mesh (read only)
        public Mesh mesh {
            get { return _mesh; }
        }

        [SerializeField] Mesh _mesh;

        #endregion

        #region Public methods

        #if UNITY_EDITOR

        /// Asset initialization
        public void Initialize(Mesh source)
        {
            // Input vertices
            var inVertices = source.vertices;
            var inBoneWeights = source.boneWeights;

            // Output vertices
            var outVertices = new List<Vector3>();
            var outBoneWeights = new List<BoneWeight>();
            //var outUVs = new List<Vector2>();

            // Enumerate unique vertices.
            for (var i = 0; i < inVertices.Length; i++)
            {
                if (!outVertices.Any(_ => _ == inVertices[i]))
                {
                    outVertices.Add(inVertices[i]);
                    outBoneWeights.Add(inBoneWeights[i]);
                }
            }

            // Assign unique UVs to the vertices.
            var outUVs = Enumerable.Range(0, outVertices.Count).
                Select(i => Vector2.right * i / outVertices.Count).ToList();

            // Output index array
            var indices = Enumerable.Range(0, outVertices.Count).ToArray();

            // Make a clone of the source mesh to avoid
            // the binding cache problem - https://goo.gl/mORHCR
            _mesh = Instantiate<Mesh>(source);
            _mesh.name = _mesh.name.Substring(0, _mesh.name.Length - 7);

            // Clear the unused attributes.
            _mesh.colors = null;
            _mesh.normals = null;
            _mesh.tangents = null;
            _mesh.uv2 = null;
            _mesh.uv3 = null;
            _mesh.uv4 = null;

            // Overwrite the vertices.
            _mesh.subMeshCount = 0;
            _mesh.SetVertices(outVertices);
            _mesh.SetUVs(0, outUVs);
            _mesh.bindposes = source.bindposes;
            _mesh.boneWeights = outBoneWeights.ToArray();

            // Add point primitives.
            _mesh.subMeshCount = 1;
            _mesh.SetIndices(indices, MeshTopology.Points, 0);

            // Finishing up.
            _mesh.Optimize();
            _mesh.UploadMeshData(true);

            // Store the vertex count.
            _vertexCount = outVertices.Count;
        }

        #endif

        #endregion

        #region ScriptableObject functions

        void OnEnable()
        {
        }

        #endregion
    }
}
