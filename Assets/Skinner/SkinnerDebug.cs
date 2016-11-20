using UnityEngine;

namespace Skinner
{
    public class SkinnerDebug : MonoBehaviour
    {
        [SerializeField] SkinnerRenderer _master;

        [SerializeField] Shader _shader;

        Mesh _mesh;
        Material _material;

        void Start()
        {
            var vertexCount = _master.template.vertexCount;

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

            _mesh = new Mesh();
            _mesh.vertices = vertices;
            _mesh.SetIndices(indices, MeshTopology.Lines, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.Optimize();
            _mesh.UploadMeshData(true);

            _material = new Material(_shader);
        }

        void Update()
        {
            _material.SetTexture("_PositionBuffer", _master.positionBuffer);
            Graphics.DrawMesh(
                _mesh, Vector3.zero, Quaternion.identity, _material, gameObject.layer, null, 0
            );
        }
    }
}
