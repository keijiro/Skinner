using UnityEngine;

namespace Skinner
{
    /// Emits glitchy triangles from the given Skinner source.
    [AddComponentMenu("Skinner/Skinner Glitch")]
    [RequireComponent(typeof(MeshRenderer))]
    public class SkinnerGlitch : MonoBehaviour
    {
        #region Public properties

        /// Reference to an effect source.
        public SkinnerSource source {
            get { return _source; }
            set { _source = value; _reconfigured = true; }
        }

        [SerializeField]
        [Tooltip("Reference to an effect source.")]
        SkinnerSource _source;

        /// Length of the frame history.
        public int historyLength {
            get { return _historyLength; }
            set { _historyLength = value; _reconfigured = true; }
        }

        [SerializeField]
        [Tooltip("Length of the frame history.")]
        int _historyLength = 256;

        /// Determines the random number sequence used for the effect.
        public int randomSeed {
            get { return _randomSeed; }
            set { _randomSeed = value; _reconfigured = true; }
        }

        [SerializeField]
        [Tooltip("Determines the random number sequence used for the effect.")]
        int _randomSeed = 0;

        /// Determines how the effect elements inherit the source velocity.
        public float velocityScale {
            get { return _velocityScale; }
            set { _velocityScale = value; }
        }

        [SerializeField, Range(0, 1)]
        [Tooltip("Determines how the effect elements inherit the source velocity.")]
        float _velocityScale = 0.2f;

        #endregion

        #region Public methods

        #if UNITY_EDITOR

        /// Notify changes on the configuration.
        /// This method is only available from Editor.
        public void UpdateConfiguration()
        {
            _reconfigured = true;
        }

        #endif

        #endregion

        #region Built-in assets

        [SerializeField] SkinnerGlitchTemplate _template;
        [SerializeField] Shader _kernelShader;
        [SerializeField] Material _defaultMaterial;

        #endregion

        #region Animation kernels

        // Temporary objects used in the animation kernels.
        Material _kernelMaterial;
        RenderTexture _positionBuffer1;
        RenderTexture _positionBuffer2;
        RenderTexture _velocityBuffer1;
        RenderTexture _velocityBuffer2;

        // Indicates changes on the configuration.
        // (temporary objects have to be reset)
        bool _reconfigured = true;

        // Create a buffer for animation kernels.
        RenderTexture CreateBuffer()
        {
            var format = RenderTextureFormat.ARGBFloat;
            var buffer = new RenderTexture(_source.vertexCount, _historyLength, 0, format);
            buffer.hideFlags = HideFlags.HideAndDontSave;
            buffer.filterMode = FilterMode.Point;
            buffer.wrapMode = TextureWrapMode.Clamp;
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

        // Create and initialize temporary objects used in the animation kernels.
        void InitializeAnimationKernels()
        {
            if (_kernelMaterial == null)
            {
                _kernelMaterial = new Material(Shader.Find("Hidden/Skinner/Glitch/Kernels"));
                _kernelMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            _kernelMaterial.SetFloat("_RandomSeed", _randomSeed);

            if (_positionBuffer1 == null) _positionBuffer1 = CreateBuffer();
            if (_positionBuffer2 == null) _positionBuffer2 = CreateBuffer();
            if (_velocityBuffer1 == null) _velocityBuffer1 = CreateBuffer();
            if (_velocityBuffer2 == null) _velocityBuffer2 = CreateBuffer();

            // Clear the first buffers.
            Graphics.Blit(null, _positionBuffer2, _kernelMaterial, 0);
            Graphics.Blit(null, _velocityBuffer2, _kernelMaterial, 0);
        }

        // Release the temporary objects used in the animation kernels.
        void ReleaseAnimationKernels()
        {
            ReleaseObject(_kernelMaterial); _kernelMaterial = null;
            ReleaseObject(_positionBuffer1); _positionBuffer1 = null;
            ReleaseObject(_positionBuffer2); _positionBuffer2 = null;
            ReleaseObject(_velocityBuffer1); _velocityBuffer1 = null;
            ReleaseObject(_velocityBuffer2); _velocityBuffer2 = null;
        }

        // Invoke the animation kernels.
        void InvokeAnimationKernels()
        {
            // Swap the buffers.
            var tempPosition = _positionBuffer1;
            var tempVelocity = _velocityBuffer1;

            _positionBuffer1 = _positionBuffer2;
            _velocityBuffer1 = _velocityBuffer2;

            _positionBuffer2 = tempPosition;
            _velocityBuffer2 = tempVelocity;

            // Source position attributes.
            _kernelMaterial.SetTexture("_SourcePositionBuffer0", _source.previousPositionBuffer);
            _kernelMaterial.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);

            // Invoke the position update kernel.
            _kernelMaterial.SetTexture("_PositionBuffer", _positionBuffer1);
            _kernelMaterial.SetTexture("_VelocityBuffer", _velocityBuffer1);
            _kernelMaterial.SetFloat("_VelocityScale", _velocityScale);
            Graphics.Blit(null, _positionBuffer2, _kernelMaterial, 2);

            // Invoke the velocity update kernel.
            _kernelMaterial.SetTexture("_PositionBuffer", _positionBuffer2);
            Graphics.Blit(null, _velocityBuffer2, _kernelMaterial, 3);
        }

        #endregion

        #region External component control

        // Custom properties applied to the mesh renderer.
        MaterialPropertyBlock _overrideProps;

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

            // Set the template mesh if not set yet.
            if (meshFilter.sharedMesh != _template.mesh)
                meshFilter.sharedMesh = _template.mesh;
        }

        // Update external component: mesh renderer
        void UpdateMeshRenderer()
        {
            var meshRenderer = GetComponent<MeshRenderer>();

            // Set the material if no material is set.
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = _defaultMaterial;

            // Override the material properties.
            if (_overrideProps == null)
                _overrideProps = new MaterialPropertyBlock();

            _overrideProps.SetTexture("_PositionBuffer", _positionBuffer2);
            _overrideProps.SetFloat("_RandomSeed", _randomSeed);
            _overrideProps.SetFloat("_BufferOffset", Time.frameCount);

            meshRenderer.SetPropertyBlock(_overrideProps);
        }

        #endregion

        #region MonoBehaviour functions

        void Reset()
        {
            _reconfigured = true;
        }

        void OnValidate()
        {
            _historyLength = Mathf.Clamp(_historyLength, 1, 1024);
        }

        void OnDestroy()
        {
            ReleaseAnimationKernels();
        }

        void LateUpdate()
        {
            // Do nothing if the source is not ready.
            if (_source == null || !_source.isReady) return;

            // Reset the animation kernels on reconfiguration.
            // Also it's called in the first frame.
            if (_reconfigured)
            {
                ReleaseAnimationKernels();
                InitializeAnimationKernels();
                _reconfigured = false;
            }

            // Invoke animation and update external components.
            InvokeAnimationKernels();
            UpdateMeshFilter();
            UpdateMeshRenderer();
        }

        #endregion
    }
}
