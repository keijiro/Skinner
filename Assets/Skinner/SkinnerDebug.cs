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

        #region MonoBehaviour functions

        void Start()
        {
            // Create a material just for wrapping the shader.
            _material = new Material(_shader);

            // Build the vertex and index array.
            var vertexCount = _source.model.vertexCount;
            var vertices = new Vector3 [vertexCount * 6];
            var indices = new int [vertexCount * 6];

            for (var i = 0; i < vertexCount; i++)
            {
                var vi = i * 6;
                var u = (0.5f + i) / vertexCount;
                vertices[vi + 0] = new Vector3(u, 0, 0);
                vertices[vi + 1] = new Vector3(u, 0, 1);
                vertices[vi + 2] = new Vector3(u, 1, 0);
                vertices[vi + 3] = new Vector3(u, 1, 1);
                vertices[vi + 4] = new Vector3(u, 2, 0);
                vertices[vi + 5] = new Vector3(u, 2, 1);
            }

            for (var i = 0; i < indices.Length; i++)
                indices[i] = i;

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
            _material.SetTexture("_PreviousPositionBuffer", _source.previousPositionBuffer);
            _material.SetTexture("_PositionBuffer", _source.positionBuffer);
            _material.SetTexture("_NormalBuffer", _source.normalBuffer);
            _material.SetTexture("_TangentBuffer", _source.tangentBuffer);
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _material, gameObject.layer, null, 0);
        }

        #endregion
    }
}
