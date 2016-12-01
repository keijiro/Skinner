using UnityEngine;
using UnityEngine.Rendering;

namespace Skinner
{
    public class SkinnerTrail : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] SkinnerSource _source;

        #endregion

        #region Internal resources

        [SerializeField] Shader _kernelsShader;
        [SerializeField] Shader _trailShader;

        Material _kernelsMaterial;
        Material _trailMaterial;

        RenderTexture _positionBuffer0;
        RenderTexture _positionBuffer1;

        RenderTexture _velocityBuffer0;
        RenderTexture _velocityBuffer1;

        RenderTexture _basisBuffer0;
        RenderTexture _basisBuffer1;

        Mesh _mesh;

        int _frameCount;

        #endregion

        #region Internal resources

        void Start()
        {
            // Materials for wrapping the shaders
            _kernelsMaterial = new Material(_kernelsShader);
            _trailMaterial = new Material(_trailShader);

            // Double position/velocity buffer
            var tw = _source.model.vertexCount / 1;
            var th = 65536 / 2 / tw;
            var format = RenderTextureFormat.ARGBFloat;

            _positionBuffer0 = new RenderTexture(tw, th, 0, format);
            _positionBuffer1 = new RenderTexture(tw, th, 0, format);

            _velocityBuffer0 = new RenderTexture(tw, th, 0, format);
            _velocityBuffer1 = new RenderTexture(tw, th, 0, format);

            _basisBuffer0 = new RenderTexture(tw, th, 0, format);
            _basisBuffer1 = new RenderTexture(tw, th, 0, format);

            _positionBuffer0.filterMode = FilterMode.Point;
            _positionBuffer1.filterMode = FilterMode.Point;

            _velocityBuffer0.filterMode = FilterMode.Point;
            _velocityBuffer1.filterMode = FilterMode.Point;

            _basisBuffer0.filterMode = FilterMode.Point;
            _basisBuffer1.filterMode = FilterMode.Point;

            // Vertex array
            var vertices = new Vector3 [tw * th * 2];
            var offs = 0;
            for (var ix = 0; ix < tw; ix++)
            {
                var u = (0.5f + ix) / tw;
                for (var iy = 0; iy < th; iy++)
                {
                    var v = (0.5f + iy) / th;
                    vertices[offs++] = new Vector3(u, v, -0.5f);
                    vertices[offs++] = new Vector3(u, v, +0.5f);
                }
            }

            // Index array
            var indices = new int [tw * 6 * (th - 1)];
            offs = 0;
            for (var ix = 0; ix < tw; ix++)
            {
                var vi = th * ix * 2;
                for (var iy = 0; iy < th - 1; iy++)
                {
                    indices[offs++] = vi;
                    indices[offs++] = vi + 2;
                    indices[offs++] = vi + 1;

                    indices[offs++] = vi + 1;
                    indices[offs++] = vi + 2;
                    indices[offs++] = vi + 3;

                    vi += 2;
                }
            }

            // Create a mesh.
            _mesh = new Mesh();
            _mesh.vertices = vertices;
            _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.Optimize();
            _mesh.UploadMeshData(true);
        }

        void OnDestroy()
        {
            if (_kernelsMaterial != null) Destroy(_kernelsMaterial);
            if (_trailMaterial != null) Destroy(_trailMaterial);

            if (_positionBuffer0 != null) Destroy(_positionBuffer0);
            if (_positionBuffer1 != null) Destroy(_positionBuffer1);

            if (_velocityBuffer0 != null) Destroy(_velocityBuffer0);
            if (_velocityBuffer1 != null) Destroy(_velocityBuffer1);

            if (_basisBuffer0 != null) Destroy(_basisBuffer0);
            if (_basisBuffer1 != null) Destroy(_basisBuffer1);

            if (_mesh != null) Destroy(_mesh);
        }

        void LateUpdate()
        {
            var swap = (_frameCount & 1) != 0;
            var pb0 = swap ? _positionBuffer1 : _positionBuffer0;
            var pb1 = swap ? _positionBuffer0 : _positionBuffer1;
            var vb0 = swap ? _velocityBuffer1 : _velocityBuffer0;
            var vb1 = swap ? _velocityBuffer0 : _velocityBuffer1;
            var bb0 = swap ? _basisBuffer1 : _basisBuffer0;
            var bb1 = swap ? _basisBuffer0 : _basisBuffer1;

            // New positions
            _kernelsMaterial.SetTexture("_NewPositionBuffer", _source.positionBuffer);

            if (_frameCount < 3)
            {
                // The first frame: initialize the buffers.
                Graphics.Blit(null, vb1, _kernelsMaterial, 0);
                Graphics.Blit(null, pb1, _kernelsMaterial, 1);
                Graphics.Blit(null, bb1, _kernelsMaterial, 2);
            }
            else
            {
                // Update velocities.
                _kernelsMaterial.SetTexture("_PositionBuffer", pb0);
                _kernelsMaterial.SetTexture("_VelocityBuffer", vb0);
                Graphics.Blit(null, vb1, _kernelsMaterial, 3);

                // Update positions.
                _kernelsMaterial.SetTexture("_VelocityBuffer", vb1);
                Graphics.Blit(null, pb1, _kernelsMaterial, 4);

                // Update orthonormal bases.
                _kernelsMaterial.SetTexture("_PositionBuffer", pb1);
                _kernelsMaterial.SetTexture("_BasisBuffer", bb0);
                Graphics.Blit(null, bb1, _kernelsMaterial, 5);
            }

            // Draw the line mesh.
            _trailMaterial.SetTexture("_PositionBuffer", pb1);
            _trailMaterial.SetTexture("_VelocityBuffer", vb1);
            _trailMaterial.SetTexture("_BasisBuffer", bb1);

            Graphics.DrawMesh(
                _mesh, Vector3.zero, Quaternion.identity,
                _trailMaterial, gameObject.layer, null, 0,
                null, ShadowCastingMode.On, true
            );

            _frameCount++;
        }

        #endregion
    }
}
