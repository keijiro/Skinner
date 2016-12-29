using UnityEngine;

namespace Skinner
{
    /// Emits particles from a given Skinner source.
    [AddComponentMenu("Skinner/Skinner Particle")]
    [RequireComponent(typeof(MeshRenderer))]
    public class SkinnerParticle : MonoBehaviour
    {
        #region External object/asset references

        /// Reference to an effect source.
        public SkinnerSource source {
            get { return _source; }
            set { _source = value; _reconfigured = true; }
        }

        [SerializeField]
        [Tooltip("Reference to an effect source.")]
        SkinnerSource _source;

        /// Reference to a template object used for rendering particles.
        public SkinnerParticleTemplate template {
            get { return _template; }
            set { _template = value; _reconfigured = true; }
        }

        [SerializeField]
        [Tooltip("Reference to a template object used for rendering particles.")]
        SkinnerParticleTemplate _template;

        #endregion

        #region Basic dynamics settings

        /// Limits speed of particles. This only affects changes in particle
        /// positions (doesn't modify velocity vectors).
        public float speedLimit {
            get { return _speedLimit; }
            set { _speedLimit = value; }
        }

        [SerializeField]
        [Tooltip("Limits speed of particles. This only affects changes in particle " +
                 "positions (doesn't modify velocity vectors).")]
        float _speedLimit = 1.0f;

        /// The drag (damping) coefficient.
        public float drag {
            get { return _drag; }
            set { _drag = value; }
        }

        [SerializeField, Range(0, 15)]
        [Tooltip("The drag (damping) coefficient.")]
        float _drag = 0.1f;

        /// The constant acceleration.
        public Vector3 gravity {
            get { return _gravity; }
            set { _gravity = value; }
        }

        [SerializeField]
        [Tooltip("The constant acceleration.")]
        Vector3 _gravity = Vector3.zero;

        #endregion

        #region Particle life (duration) settings

        /// Changes the duration of a particle based on its initial speed.
        public float speedToLife {
            get { return _speedToLife; }
            set { _speedToLife = value; }
        }

        [SerializeField]
        [Tooltip("Changes the duration of a particle based on its initial speed.")]
        float _speedToLife = 4.0f;

        /// The maximum duration of particles.
        public float maxLife {
            get { return _maxLife; }
            set { _maxLife = value; }
        }

        [SerializeField]
        [Tooltip("The maximum duration of particles.")]
        float _maxLife = 4.0f;

        #endregion

        #region Spin (rotational movement) settings

        /// Changes the angular velocity of a particle based on its speed.
        public float speedToSpin {
            get { return _speedToSpin; }
            set { _speedToSpin = value; }
        }

        [SerializeField]
        [Tooltip("Changes the angular velocity of a particle based on its speed.")]
        float _speedToSpin = 60.0f;

        /// The maximum angular velocity of particles.
        public float maxSpin {
            get { return _maxSpin; }
            set { _maxSpin = value; }
        }

        [SerializeField]
        [Tooltip("The maximum angular velocity of particles.")]
        float _maxSpin = 20.0f;

        #endregion

        #region Particle scale settings

        /// Changes the scale of a particle based on its initial speed.
        public float speedToScale {
            get { return _speedToScale; }
            set { _speedToScale = value; }
        }

        [SerializeField]
        [Tooltip("Changes the scale of a particle based on its initial speed.")]
        float _speedToScale = 0.5f;

        /// The maximum scale of particles.
        public float maxScale {
            get { return _maxScale; }
            set { _maxScale = value; }
        }

        [SerializeField]
        [Tooltip("The maximum scale of particles.")]
        float _maxScale = 1.0f;

        #endregion

        #region Turbulent noise settings

        /// The amplitude of acceleration from the turbulent noise.
        public float noiseAmplitude {
            get { return _noiseAmplitude; }
            set { _noiseAmplitude = value; }
        }

        [SerializeField]
        [Tooltip("The amplitude of acceleration from the turbulent noise field.")]
        float _noiseAmplitude = 1.0f;

        /// The spatial frequency of the turbulent noise field.
        public float noiseFrequency {
            get { return _noiseFrequency; }
            set { _noiseFrequency = value; }
        }

        [SerializeField]
        [Tooltip("The spatial frequency of the turbulent noise field.")]
        float _noiseFrequency = 0.2f;

        /// Determines how fast the turbulent noise field changes.
        public float noiseMotion {
            get { return _noiseMotion; }
            set { _noiseMotion = value; }
        }

        [SerializeField]
        [Tooltip("Determines how fast the turbulent noise field changes.")]
        float _noiseMotion = 1.0f;

        #endregion

        #region Other settings

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

        [SerializeField] Shader _kernelShader;
        [SerializeField] Material _defaultMaterial;

        #endregion

        #region Animation kernels management

        enum Kernels {
            InitializePosition, InitializeVelocity, InitializeRotation,
            UpdatePosition, UpdateVelocity, UpdateRotation
        }

        enum Buffers { Position, Velocity, Rotation }

        AnimationKernelSet<Kernels, Buffers> _kernel;

        // Local state variables.
        Vector3 _noiseOffset;

        void InvokeAnimationKernels()
        {
            if (_kernel == null)
                _kernel = new AnimationKernelSet<Kernels, Buffers>(_kernelShader, x => (int)x, x => (int)x);

            if (!_kernel.ready)
            {
                // Initialize the animation kernels and buffers.
                _kernel.Setup(_template.instanceCount, 1);
                _kernel.material.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);
                _kernel.material.SetFloat("_RandomSeed", _randomSeed);
                _kernel.Invoke(Kernels.InitializePosition, Buffers.Position);
                _kernel.Invoke(Kernels.InitializeVelocity, Buffers.Velocity);
                _kernel.Invoke(Kernels.InitializeRotation, Buffers.Rotation);
            }
            else
            {
                // Update kernel parameters.
                _kernel.material.SetVector("_Damper", new Vector2(
                    Mathf.Exp(-_drag * Time.deltaTime), _speedLimit
                ));

                _kernel.material.SetVector("_Gravity", _gravity * Time.deltaTime);

                _kernel.material.SetVector("_Life", new Vector2(
                    Time.deltaTime / _maxLife, Time.deltaTime / (_maxLife * _speedToLife)
                ));

                var pi360dt = Mathf.PI * Time.deltaTime / 360;
                _kernel.material.SetVector("_Spin", new Vector2(
                    _maxSpin * pi360dt, _speedToSpin * pi360dt
                ));

                _kernel.material.SetVector("_NoiseParams", new Vector2(
                    _noiseFrequency, _noiseAmplitude * Time.deltaTime
                ));

                // Move the noise field backward in the direction of the
                // gravity vector, or simply pull up if no gravity is set.
                var noiseDir = (_gravity == Vector3.zero) ? Vector3.up : _gravity.normalized;
                _noiseOffset += noiseDir * _noiseMotion * Time.deltaTime;
                _kernel.material.SetVector("_NoiseOffset", _noiseOffset);

                // Transfer the source position attributes.
                _kernel.material.SetTexture("_SourcePositionBuffer0", _source.previousPositionBuffer);
                _kernel.material.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);

                // Invoke the position update kernel.
                _kernel.material.SetTexture("_PositionBuffer", _kernel.GetLastBuffer(Buffers.Position));
                _kernel.material.SetTexture("_VelocityBuffer", _kernel.GetLastBuffer(Buffers.Velocity));
                _kernel.Invoke(Kernels.UpdatePosition, Buffers.Position);

                // Invoke the velocity update kernel with the updated positions.
                _kernel.material.SetTexture("_PositionBuffer", _kernel.GetWorkingBuffer(Buffers.Position));
                _kernel.Invoke(Kernels.UpdateVelocity, Buffers.Velocity);

                // Invoke the rotation update kernel with the updated velocity.
                _kernel.material.SetTexture("_RotationBuffer", _kernel.GetLastBuffer(Buffers.Rotation));
                _kernel.material.SetTexture("_VelocityBuffer", _kernel.GetWorkingBuffer(Buffers.Velocity));
                _kernel.Invoke(Kernels.UpdateRotation, Buffers.Rotation);
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
            block.SetTexture("_PreviousPositionBuffer", _kernel.GetWorkingBuffer(Buffers.Position));
            block.SetTexture("_PreviousRotationBuffer", _kernel.GetWorkingBuffer(Buffers.Rotation));
            block.SetTexture("_PositionBuffer", _kernel.GetLastBuffer(Buffers.Position));
            block.SetTexture("_VelocityBuffer", _kernel.GetLastBuffer(Buffers.Velocity));
            block.SetTexture("_RotationBuffer", _kernel.GetLastBuffer(Buffers.Rotation));
            block.SetVector("_Scale", new Vector2(_maxScale, _speedToScale));
            block.SetFloat("_RandomSeed", _randomSeed);

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
            _speedToLife = Mathf.Max(_speedToLife, 0);
            _maxLife = Mathf.Max(_maxLife, 0.01f);

            _speedToScale = Mathf.Max(_speedToScale, 0);
            _maxScale = Mathf.Max(_maxScale, 0);
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
