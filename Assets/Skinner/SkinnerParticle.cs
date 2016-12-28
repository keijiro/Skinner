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

        /// Determines the duration of a particle based on its initial speed.
        public float speedToLife {
            get { return _speedToLife; }
            set { _speedToLife = value; }
        }

        [SerializeField]
        [Tooltip("Determines the duration of a particle based on its initial speed.")]
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

        /// Determines the angular velocity of a particle based on its speed.
        public float speedToSpin {
            get { return _speedToSpin; }
            set { _speedToSpin = value; }
        }

        [SerializeField]
        [Tooltip("Determines the angular velocity of a particle based on its speed.")]
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

        /// Determines the scale of a particle based on its initial speed.
        public float speedToScale {
            get { return _speedToScale; }
            set { _speedToScale = value; }
        }

        [SerializeField]
        [Tooltip("Determines the scale of a particle based on its initial speed.")]
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
        RenderTexture _rotationBuffer1;
        RenderTexture _rotationBuffer2;

        // Variables for animation.
        Vector3 _noiseOffset;

        // Indicates changes on the configuration.
        // (temporary objects have to be reset)
        bool _reconfigured = true;

        // Create a buffer for animation kernels.
        RenderTexture CreateBuffer()
        {
            var format = RenderTextureFormat.ARGBFloat;
            var buffer = new RenderTexture(_template.instanceCount, 1, 0, format);
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
                _kernelMaterial = new Material(Shader.Find("Hidden/Skinner/Particle/Kernels"));
                _kernelMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            _kernelMaterial.SetFloat("_RandomSeed", _randomSeed);

            if (_positionBuffer1 == null) _positionBuffer1 = CreateBuffer();
            if (_positionBuffer2 == null) _positionBuffer2 = CreateBuffer();
            if (_velocityBuffer1 == null) _velocityBuffer1 = CreateBuffer();
            if (_velocityBuffer2 == null) _velocityBuffer2 = CreateBuffer();
            if (_rotationBuffer1 == null) _rotationBuffer1 = CreateBuffer();
            if (_rotationBuffer2 == null) _rotationBuffer2 = CreateBuffer();

            // Clear the first buffers.
            _kernelMaterial.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);
            Graphics.Blit(null, _positionBuffer2, _kernelMaterial, 0);
            Graphics.Blit(null, _velocityBuffer2, _kernelMaterial, 1);
            Graphics.Blit(null, _rotationBuffer2, _kernelMaterial, 2);
        }

        // Release the temporary objects used in the animation kernels.
        void ReleaseAnimationKernels()
        {
            ReleaseObject(_kernelMaterial); _kernelMaterial = null;
            ReleaseObject(_positionBuffer1); _positionBuffer1 = null;
            ReleaseObject(_positionBuffer2); _positionBuffer2 = null;
            ReleaseObject(_velocityBuffer1); _velocityBuffer1 = null;
            ReleaseObject(_velocityBuffer2); _velocityBuffer2 = null;
            ReleaseObject(_rotationBuffer1); _rotationBuffer1 = null;
            ReleaseObject(_rotationBuffer2); _rotationBuffer2 = null;
        }

        // Invoke the animation kernels.
        void InvokeAnimationKernels()
        {
            // Swap the buffers.
            var tempPosition = _positionBuffer1;
            var tempVelocity = _velocityBuffer1;
            var tempRotation = _rotationBuffer1;

            _positionBuffer1 = _positionBuffer2;
            _velocityBuffer1 = _velocityBuffer2;
            _rotationBuffer1 = _rotationBuffer2;

            _positionBuffer2 = tempPosition;
            _velocityBuffer2 = tempVelocity;
            _rotationBuffer2 = tempRotation;

            // Update kernel parameters.
            _kernelMaterial.SetVector("_Damper", new Vector2(
                Mathf.Exp(-_drag * Time.deltaTime), _speedLimit
            ));

            _kernelMaterial.SetVector("_Gravity", _gravity * Time.deltaTime);

            _kernelMaterial.SetVector("_Life", new Vector2(
                Time.deltaTime / _maxLife, Time.deltaTime / (_maxLife * _speedToLife)
            ));

            var pi360dt = Mathf.PI * Time.deltaTime / 360;
            _kernelMaterial.SetVector("_Spin", new Vector2(
                _maxSpin * pi360dt, _speedToSpin * pi360dt
            ));

            _kernelMaterial.SetVector("_NoiseParams", new Vector2(
                _noiseFrequency, _noiseAmplitude * Time.deltaTime
            ));

            // Move the noise field backward in the direction of the
            // gravity vector, or simply pull up if no gravity is set.
            var noiseDir = (_gravity == Vector3.zero) ? Vector3.up : _gravity.normalized;
            _noiseOffset += noiseDir * _noiseMotion * Time.deltaTime;
            _kernelMaterial.SetVector("_NoiseOffset", _noiseOffset);

            // Source position attributes.
            _kernelMaterial.SetTexture("_SourcePositionBuffer0", _source.previousPositionBuffer);
            _kernelMaterial.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);

            // Invoke the position update kernel.
            _kernelMaterial.SetTexture("_PositionBuffer", _positionBuffer1);
            _kernelMaterial.SetTexture("_VelocityBuffer", _velocityBuffer1);
            Graphics.Blit(null, _positionBuffer2, _kernelMaterial, 3);

            // Invoke the velocity update kernel.
            _kernelMaterial.SetTexture("_PositionBuffer", _positionBuffer2);
            Graphics.Blit(null, _velocityBuffer2, _kernelMaterial, 4);

            // Invoke the rotation update kernel.
            _kernelMaterial.SetTexture("_RotationBuffer", _rotationBuffer1);
            _kernelMaterial.SetTexture("_VelocityBuffer", _velocityBuffer2);
            Graphics.Blit(null, _rotationBuffer2, _kernelMaterial, 5);
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
            _overrideProps.SetTexture("_VelocityBuffer", _velocityBuffer2);
            _overrideProps.SetTexture("_RotationBuffer", _rotationBuffer2);
            _overrideProps.SetVector("_Scale", new Vector2(_maxScale, _speedToScale));
            _overrideProps.SetFloat("_RandomSeed", _randomSeed);

            meshRenderer.SetPropertyBlock(_overrideProps);
        }

        #endregion

        #region MonoBehaviour functions

        void Reset()
        {
            _reconfigured = true;
        }

        void OnDestroy()
        {
            ReleaseAnimationKernels();
        }

        void OnValidate()
        {
            _speedToLife = Mathf.Max(_speedToLife, 0);
            _maxLife = Mathf.Max(_maxLife, 0.01f);

            _speedToScale = Mathf.Max(_speedToScale, 0);
            _maxScale = Mathf.Max(_maxScale, 0);
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
