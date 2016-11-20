using UnityEngine;

namespace Skinner
{
    public class SkinnerDebug : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] SkinnerSource _source;

        #endregion

        #region Internal resources

        [SerializeField] Shader _shader;

        Material _material;
        Mesh _mesh;

        #endregion

        void Start()
        {
            // Create a material just for wrapping the shader.
            _material = new Material(_shader);

            // Build the vertex and index array.
            var vertexCount = _source.model.vertexCount;
            var vertices = new Vector3 [vertexCount * 4];
            var indices = new int [vertexCount * 8];

            for (var i = 0; i < vertexCount; i++)
            {
                var vi = i * 4;
                var u = (0.5f + i) / vertexCount;
                vertices[vi + 0] = new Vector3(u, -1, -1);
                vertices[vi + 1] = new Vector3(u, +1, -1);
                vertices[vi + 2] = new Vector3(u, -1, +1);
                vertices[vi + 3] = new Vector3(u, +1, +1);

                var ii = i * 8;
                indices[ii + 0] = vi;
                indices[ii + 1] = vi + 1;
                indices[ii + 2] = vi + 1;
                indices[ii + 3] = vi + 3;
                indices[ii + 4] = vi + 3;
                indices[ii + 5] = vi + 2;
                indices[ii + 6] = vi + 2;
                indices[ii + 7] = vi + 0;
            }

            // Create a mesh.
            _mesh = new Mesh();
            _mesh.vertices = vertices;
            _mesh.SetIndices(indices, MeshTopology.Lines, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.Optimize();
            _mesh.UploadMeshData(true);
        }

        void OnDestroy()
        {
            if (_material != null) Destroy(_material);
            if (_mesh != null) Destroy(_mesh);
        }

        void Update()
        {
            _material.SetTexture("_PositionBuffer", _source.positionBuffer);
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _material, gameObject.layer, null, 0);
        }
    }
}
