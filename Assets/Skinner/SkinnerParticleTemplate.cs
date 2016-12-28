using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Skinner
{
    /// Template mesh asset used in SkinnerParticle
    public class SkinnerParticleTemplate : ScriptableObject
    {
        #region Public properties

        /// List of meshes of particle shapes.
        public Mesh[] shapes { get { return _shapes; } }

        [Tooltip("List of meshes of particle shapes.")]
        [SerializeField] Mesh[] _shapes = new Mesh[1];

        /// Maximum number of particle instances.
        public int maxInstanceCount { get { return _maxInstanceCount; } }

        [Tooltip("Maximum number of particle instances.")]
        [SerializeField] int _maxInstanceCount = 8192;

        /// Actual number of particle instances.
        /// This value may be less than maxInstanceCount.
        public int instanceCount { get { return _instanceCount; } }

        [SerializeField] int _instanceCount;

        /// Tmplate mesh object.
        public Mesh mesh { get { return _mesh; } }

        [SerializeField] Mesh _mesh;

        #endregion

        #region Private members

        [SerializeField] Mesh _defaultShape;

        Mesh GetShape(int index)
        {
            if (_shapes == null || _shapes.Length == 0) return _defaultShape;
            var mesh = _shapes[index % _shapes.Length];
            return mesh == null ? _defaultShape : mesh;
        }

        #endregion

        #region Public methods

        #if UNITY_EDITOR

        public void RebuildMesh()
        {
            var vtx_out = new List<Vector3>();
            var nrm_out = new List<Vector3>();
            var tan_out = new List<Vector4>();
            var uv0_out = new List<Vector2>();
            var uv1_out = new List<Vector2>();
            var idx_out = new List<int>();

            var vertexCount = 0;
            _instanceCount = 0;

            // Push the source shapes one by one into the temporary array.
            while (_instanceCount < maxInstanceCount)
            {
                // Get the N-th Source mesh.
                var mesh = GetShape(_instanceCount);
                var vtx_in = mesh.vertices;

                // Keep the vertex count under 64k.
                if (vertexCount + vtx_in.Length > 65535) break;

                // Copy the vertices.
                vtx_out.AddRange(vtx_in);
                nrm_out.AddRange(mesh.normals);
                tan_out.AddRange(mesh.tangents);
                uv0_out.AddRange(mesh.uv);

                // Set UV1 temporarily.
                var uv1 = new Vector2(_instanceCount + 0.5f, 0);
                uv1_out.AddRange(Enumerable.Repeat(uv1, vtx_in.Length));

                // Copy the indices.
                idx_out.AddRange(mesh.triangles.Select(i => i + vertexCount));

                // Increment the vertex/instance count.
                vertexCount += vtx_in.Length;
                _instanceCount++;
            }

            // Rescale the UV1.
            uv1_out = uv1_out.Select(x => x / instanceCount).ToList();

            // Rebuild the mesh asset.
            _mesh.Clear();
            _mesh.SetVertices(vtx_out);
            _mesh.SetNormals(nrm_out);
            _mesh.SetUVs(0, uv0_out);
            _mesh.SetUVs(1, uv1_out);
            _mesh.SetIndices(idx_out.ToArray(), MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.UploadMeshData(true);
        }

        #endif

        #endregion

        #region ScriptableObject functions

        void OnValidate()
        {
            _maxInstanceCount = Mathf.Clamp(_maxInstanceCount, 4, 8192);
        }

        void OnEnable()
        {
            if (_mesh == null) {
                _mesh = new Mesh();
                _mesh.name = "Skinner Particle Template";
            }
        }

        #endregion
    }
}
