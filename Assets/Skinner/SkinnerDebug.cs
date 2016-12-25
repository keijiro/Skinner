using UnityEngine;

namespace Skinner
{
    /// Visualizes vertex attributes of a Skinner source.
    [AddComponentMenu("Skinner/Skinner Debug")]
    public class SkinnerDebug : MonoBehaviour
    {
        #region Editable properties

        /// Reference to a Skinner source that will be visualized.
        public SkinnerSource source {
            get { return _source; }
            set { _source = value; }
        }

        [SerializeField]
        [Tooltip("Reference to a Skinner source that will be visualized.")]
        SkinnerSource _source;

        #endregion

        #region Internal resources

        // Debug shader and temporary objects.
        [SerializeField] Shader _shader;
        Material _material;
        MaterialPropertyBlock _props;

        // Temporary mesh object that's filled with lines.
        Mesh _mesh;

        // The count of line per temporary mesh.
        const int linesPerMesh = 0x10000 / 6; // 10,922

        #endregion

        #region MonoBehaviour functions

        void Start()
        {
            // Create a material just for wrapping the shader.
            _material = new Material(_shader);
            _props = new MaterialPropertyBlock();

            // Build the temporary mesh.
            var vertices = new Vector3 [linesPerMesh * 6];
            var indices = new int [linesPerMesh * 6];

            for (var i = 0; i < linesPerMesh; i++)
            {
                var vi = i * 6;
                var u = (0.5f + i) / linesPerMesh;
                vertices[vi + 0] = new Vector3(u, 0, 0); // velocity line
                vertices[vi + 1] = new Vector3(u, 0, 1);
                vertices[vi + 2] = new Vector3(u, 1, 0); // normal line
                vertices[vi + 3] = new Vector3(u, 1, 1);
                vertices[vi + 4] = new Vector3(u, 2, 0); // tangent line
                vertices[vi + 5] = new Vector3(u, 2, 1);
            }

            for (var i = 0; i < indices.Length; i++) indices[i] = i;

            // Create a mesh.
            _mesh = new Mesh();
            _mesh.vertices = vertices;
            _mesh.SetIndices(indices, MeshTopology.Lines, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.UploadMeshData(true);
        }

        void OnDestroy()
        {
            if (_material != null) Destroy(_material);
            if (_mesh != null) Destroy(_mesh);
        }

        void LateUpdate()
        {
            // Do nothing if the source is not ready.
            if (_source == null || !_source.isReady) return;

            // Update the source buffers.
            _props.SetTexture("_PreviousPositionBuffer", _source.previousPositionBuffer);
            _props.SetTexture("_PositionBuffer", _source.positionBuffer);
            _props.SetTexture("_NormalBuffer", _source.normalBuffer);
            _props.SetTexture("_TangentBuffer", _source.tangentBuffer);

            // Draw the mesh repeatedly to cover all the vertices in the source.
            var drawCount = (_source.vertexCount + linesPerMesh - 1) / linesPerMesh;
            for (var i = 0; i < drawCount; i++)
            {
                _props.SetFloat("_BufferOffset", (float)i / (linesPerMesh * drawCount));
                Graphics.DrawMesh(
                    _mesh, Vector3.zero, Quaternion.identity, _material,
                    gameObject.layer, null, 0, _props
                );
            }
        }

        #endregion
    }
}
