using UnityEngine;
using System.Collections.Generic;

namespace Skinner
{
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("Skinner/Glitch")]
    public class SkinnerGlitch : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] SkinnerSource _source;

        [SerializeField, Range(0, 1)] float _throttle = 1.0f;

        public float throttle {
            get { return _throttle; }
            set { _throttle = value; }
        }

        [SerializeField] int _randomSeed = 0;

        public int randomSeed {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }

        #endregion

        #region Private members

        // References to the built-in assets
        [SerializeField] Shader _kernelsShader;
        [SerializeField] Shader _surfaceShader;

        // Temporary objects
        Mesh _mesh;
        Material _kernelsMaterial;
        Material _surfaceMaterial;
        RenderTexture _positionBuffer1;
        RenderTexture _positionBuffer2;
        RenderTexture _velocityBuffer1;
        RenderTexture _velocityBuffer2;

        // Custom properties applied to the mesh renderer.
        MaterialPropertyBlock _propertyBlock;

        // Create a bulk mesh.
        Mesh CreateBulkMesh()
        {
            var vcount = 21845 * 3; // 66535
            var vertices = new Vector3[vcount];
            var uvs = new List<Vector4>(vcount);
            var indices = new int[vcount];

            for (var i = 0; i < vcount; i += 3)
            {
                float u0 = Random.value;
                float u1 = Random.value;
                float u2 = Random.value;
                float u3 = Random.value;

                vertices[i + 0] = new Vector3(u0, u1, u2);
                vertices[i + 1] = new Vector3(u1, u2, u0);
                vertices[i + 2] = new Vector3(u2, u0, u1);

                uvs.Add(new Vector4(u0, u1, u2, u3));
                uvs.Add(new Vector4(u1, u2, u0, u3));
                uvs.Add(new Vector4(u2, u0, u1, u3));

                indices[i + 0] = i + 0;
                indices[i + 1] = i + 1;
                indices[i + 2] = i + 2;
            }

            var mesh = new Mesh();
            mesh.name = "Glitch";
            mesh.vertices = vertices;
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            mesh.Optimize();
            mesh.UploadMeshData(true);

            return mesh;
        }

        // Create a buffer for simulation.
        RenderTexture CreateSimulationBuffer()
        {
            var format = RenderTextureFormat.ARGBFloat;
            var buffer = new RenderTexture(_source.model.vertexCount, 256, 0, format);
            buffer.hideFlags = HideFlags.HideAndDontSave;
            buffer.filterMode = FilterMode.Point;
            buffer.wrapMode = TextureWrapMode.Repeat;
            return buffer;
        }

        // Try to release a temporary object.
        void ReleaseObject(Object o)
        {
            if (o != null)
            {
                if (Application.isPlaying)
                    Destroy(o);
                else
                    DestroyImmediate(o);
            }
        }

        // Create and initialize internal temporary objects.
        void SetUpTemporaryObjects()
        {
            if (_kernelsMaterial == null)
            {
                _kernelsMaterial = new Material(Shader.Find("Hidden/Skinner/Glitch/Kernels"));
                _kernelsMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            if (_surfaceMaterial == null)
            {
                _surfaceMaterial = new Material(Shader.Find("Hidden/Skinner/Glitch/Surface"));
                _surfaceMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            if (_mesh == null)
            {
                _mesh = CreateBulkMesh();
                _mesh.hideFlags = HideFlags.HideAndDontSave;
            }

            if (_positionBuffer1 == null)
                _positionBuffer1 = CreateSimulationBuffer();

            if (_positionBuffer2 == null)
                _positionBuffer2 = CreateSimulationBuffer();

            if (_velocityBuffer1 == null)
                _velocityBuffer1 = CreateSimulationBuffer();

            if (_velocityBuffer2 == null)
                _velocityBuffer2 = CreateSimulationBuffer();
        }

        // Release internal temporary objects.
        void ReleaseTemporaryObjects()
        {
            ReleaseObject(_kernelsMaterial);
            _kernelsMaterial = null;

            ReleaseObject(_surfaceMaterial);
            _surfaceMaterial = null;

            ReleaseObject(_mesh);
            _mesh = null;

            ReleaseObject(_positionBuffer1);
            _positionBuffer1 = null;

            ReleaseObject(_positionBuffer2);
            _positionBuffer2 = null;

            ReleaseObject(_velocityBuffer1);
            _velocityBuffer1 = null;

            ReleaseObject(_velocityBuffer2);
            _velocityBuffer2 = null;
        }

        // Reset the simulation state.
        void ResetSimulationState()
        {
            Graphics.Blit(null, _positionBuffer2, _kernelsMaterial, 0);
            Graphics.Blit(null, _velocityBuffer2, _kernelsMaterial, 0);
        }

        // Update the parameters in the simulation kernels.
        void UpdateSimulationParameters(float dt)
        {
            _kernelsMaterial.SetFloat("_RandomSeed", _randomSeed);
        }

        // Invoke the simulation kernels.
        void InvokeSimulationKernels(float dt)
        {
            // Swap the buffers.
            var tempPosition = _positionBuffer1;
            var tempVelocity = _velocityBuffer1;

            _positionBuffer1 = _positionBuffer2;
            _velocityBuffer1 = _velocityBuffer2;

            _positionBuffer2 = tempPosition;
            _velocityBuffer2 = tempVelocity;

            // Source position information
            _kernelsMaterial.SetTexture("_SourcePositionBuffer0", _source.previousPositionBuffer);
            _kernelsMaterial.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);

            // Invoke the position update kernel.
            UpdateSimulationParameters(dt);
            _kernelsMaterial.SetTexture("_PositionBuffer", _positionBuffer1);
            _kernelsMaterial.SetTexture("_VelocityBuffer", _velocityBuffer1);
            Graphics.Blit(null, _positionBuffer2, _kernelsMaterial, 2);

            // Invoke the velocity update kernel.
            _kernelsMaterial.SetTexture("_PositionBuffer", _positionBuffer2);
            Graphics.Blit(null, _velocityBuffer2, _kernelsMaterial, 3);
        }

        // Update external component: mesh filter
        void UpdateMeshFilter()
        {
            var meshFilter = GetComponent<MeshFilter>();

            // Add a new mesh filter if missing.
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.hideFlags = HideFlags.NotEditable;
            }

            if (meshFilter.sharedMesh != _mesh)
                meshFilter.sharedMesh = _mesh;
        }

        // Update external component: mesh renderer
        void UpdateMeshRenderer()
        {
            var meshRenderer = GetComponent<MeshRenderer>();

            if (_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();

            _propertyBlock.SetTexture("_PositionBuffer", _positionBuffer2);
            _propertyBlock.SetTexture("_VelocityBuffer", _velocityBuffer2);
            _propertyBlock.SetFloat("_RandomSeed", _randomSeed);
            _propertyBlock.SetFloat("_Offset", Time.frameCount);

            meshRenderer.SetPropertyBlock(_propertyBlock);

            // Set the material if no material is set.
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = _surfaceMaterial;
        }

        #endregion

        #region MonoBehaviour functions

        void OnDestroy()
        {
            ReleaseTemporaryObjects();
        }

        void LateUpdate()
        {
            // Do nothing if no source is given.
            if (_source == null) return;

            // Initialize the temporary objects if not yet.
            if (_mesh == null)
            {
                SetUpTemporaryObjects();
                ResetSimulationState();
            }

            // Advance simulation time.
            InvokeSimulationKernels(Time.deltaTime);

            // Update external components (mesh filter and renderer).
            UpdateMeshFilter();
            UpdateMeshRenderer();
        }

        #endregion
    }
}
