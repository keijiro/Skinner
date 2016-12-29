using UnityEngine;

namespace Skinner
{
    /// Emits glitchy triangles from a given Skinner source.
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

        /// Length of the frame history buffer.
        public int historyLength {
            get { return _historyLength; }
            set { _historyLength = value; _reconfigured = true; }
        }

        [SerializeField]
        [Tooltip("Length of the frame history buffer.")]
        int _historyLength = 256;

        /// Determines how an effect element inherit a source velocity.
        public float velocityScale {
            get { return _velocityScale; }
            set { _velocityScale = value; }
        }

        [SerializeField, Range(0, 1)]
        [Tooltip("Determines how an effect element inherit a source velocity.")]
        float _velocityScale = 0.2f;

        /// Triangles that have longer edges than this value will be culled.
        public float edgeThreshold {
            get { return _edgeThreshold; }
            set { _edgeThreshold = value; }
        }

        [SerializeField]
        [Tooltip("Triangles that have longer edges than this value will be culled.")]
        float _edgeThreshold = 0.75f;

        /// Triangles that have larger area than this value will be culled.
        public float areaThreshold {
            get { return _areaThreshold; }
            set { _areaThreshold = value; }
        }

        [SerializeField]
        [Tooltip("Triangles that have larger area than this value will be culled.")]
        float _areaThreshold = 0.02f;

        /// Determines the random number sequence used for the effect.
        public int randomSeed {
            get { return _randomSeed; }
            set { _randomSeed = value; _reconfigured = true; }
        }

        [SerializeField]
        [Tooltip("Determines the random number sequence used for the effect.")]
        int _randomSeed = 0;

        #endregion

        #region Reconfiguration detection

        // Indicates changes in the configuration.
        // (temporary objects have to be reset)
        bool _reconfigured;

        #if UNITY_EDITOR

        /// Notify changes in the configuration.
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

        #region Animation kernels management

        enum Kernels {
            InitializePosition, InitializeVelocity,
            UpdatePosition, UpdateVelocity
        }

        enum Buffers { Position, Velocity }

        AnimationKernelSet<Kernels, Buffers> _kernel;

        void InvokeAnimationKernels()
        {
            if (_kernel == null)
                _kernel = new AnimationKernelSet<Kernels, Buffers>(_kernelShader, x => (int)x, x => (int)x);

            if (!_kernel.ready)
            {
                // Initialize the animation kernels and buffers.
                _kernel.Setup(_source.vertexCount, _historyLength);
                _kernel.material.SetFloat("_RandomSeed", _randomSeed);
                _kernel.Invoke(Kernels.InitializePosition, Buffers.Position);
                _kernel.Invoke(Kernels.InitializeVelocity, Buffers.Velocity);
            }
            else
            {
                // Transfer the source position attributes.
                _kernel.material.SetTexture("_SourcePositionBuffer0", _source.previousPositionBuffer);
                _kernel.material.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);

                // Invoke the position update kernel.
                _kernel.material.SetTexture("_PositionBuffer", _kernel.GetLastBuffer(Buffers.Position));
                _kernel.material.SetTexture("_VelocityBuffer", _kernel.GetLastBuffer(Buffers.Velocity));
                _kernel.material.SetFloat("_VelocityScale", _velocityScale);
                _kernel.Invoke(Kernels.UpdatePosition, Buffers.Position);

                // Invoke the velocity update kernel with the updated positions.
                _kernel.material.SetTexture("_PositionBuffer", _kernel.GetWorkingBuffer(Buffers.Position));
                _kernel.Invoke(Kernels.UpdateVelocity, Buffers.Velocity);
            }

            _kernel.SwapBuffers();
        }

        #endregion

        #region External renderer control

        RendererAdapter _renderer;

        void UpdateRenderer()
        {
            if (_renderer == null)
                _renderer = new RendererAdapter(gameObject, _defaultMaterial);

            // Update the custom property block.
            var block = _renderer.propertyBlock;
            block.SetTexture("_PositionBuffer", _kernel.GetLastBuffer(Buffers.Position));
            block.SetFloat("_EdgeThreshold", _edgeThreshold);
            block.SetFloat("_AreaThreshold", _areaThreshold);
            block.SetFloat("_RandomSeed", _randomSeed);
            block.SetFloat("_BufferOffset", Time.frameCount);

            _renderer.Update(_template.mesh);
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
            _edgeThreshold = Mathf.Max(_edgeThreshold, 0);
            _areaThreshold = Mathf.Max(_areaThreshold, 0);
        }

        void OnDestroy()
        {
            _kernel.Release();
        }

        void LateUpdate()
        {
            // Do nothing if the source is not ready.
            if (_source == null || !_source.isReady) return;

            // Reset the animation kernels on reconfiguration.
            if (_reconfigured)
            {
                if (_kernel != null) _kernel.Release();
                _reconfigured = false;
            }

            // Invoke the animation kernels and update the renderer.
            InvokeAnimationKernels();
            UpdateRenderer();
        }

        #endregion
    }
}
