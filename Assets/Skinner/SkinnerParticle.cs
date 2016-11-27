using UnityEngine;

namespace Skinner
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("Skinner/Particle")]
    public class SkinnerParticle : MonoBehaviour
    {
        #region Emitter parameters

        [SerializeField] SkinnerSource _source;

        [SerializeField, Range(0, 1)] float _throttle = 1.0f;

        public float throttle {
            get { return _throttle; }
            set { _throttle = value; }
        }

        #endregion

        #region Particle life parameters

        [SerializeField] float _life = 4.0f;

        public float life {
            get { return _life; }
            set { _life = value; }
        }

        [SerializeField, Range(0, 1)] float _lifeRandomness = 0.6f;

        public float lifeRandomness {
            get { return _lifeRandomness; }
            set { _lifeRandomness = value; }
        }

        #endregion

        #region Acceleration parameters

        [SerializeField] Vector3 _acceleration = Vector3.zero;

        public Vector3 acceleration {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        [SerializeField, Range(0, 4)] float _drag = 0.1f;

        public float drag {
            get { return _drag; }
            set { _drag = value; }
        }

        #endregion

        #region Rotation parameters

        [SerializeField] float _spin = 20.0f;

        public float spin {
            get { return _spin; }
            set { _spin = value; }
        }

        [SerializeField] float _speedToSpin = 60.0f;

        public float speedToSpin {
            get { return _speedToSpin; }
            set { _speedToSpin = value; }
        }

        [SerializeField, Range(0, 1)] float _spinRandomness = 0.3f;

        public float spinRandomness {
            get { return _spinRandomness; }
            set { _spinRandomness = value; }
        }

        #endregion

        #region Turbulent noise parameters

        [SerializeField] float _noiseAmplitude = 1.0f;

        public float noiseAmplitude {
            get { return _noiseAmplitude; }
            set { _noiseAmplitude = value; }
        }

        [SerializeField] float _noiseFrequency = 0.2f;

        public float noiseFrequency {
            get { return _noiseFrequency; }
            set { _noiseFrequency = value; }
        }

        [SerializeField] float _noiseMotion = 1.0f;

        public float noiseMotion {
            get { return _noiseMotion; }
            set { _noiseMotion = value; }
        }

        #endregion

        #region Render settings

        [SerializeField] SkinnerParticleTemplate _template;

        [SerializeField] float _scale = 1.0f;

        public float scale {
            get { return _scale; }
            set { _scale = value; }
        }

        [SerializeField, Range(0, 1)] float _scaleRandomness = 0.5f;

        public float scaleRandomness {
            get { return _scaleRandomness; }
            set { _scaleRandomness = value; }
        }

        [SerializeField] int _randomSeed = 0;

        public int randomSeed {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }

        #endregion

        #region Public functions

        #if UNITY_EDITOR

        public void RequestReconfigurationFromEditor()
        {
            _reconfigured = true;
        }

        #endif

        #endregion

        #region Private members

        // References to the built-in assets
        [SerializeField] Shader _kernels;
        [SerializeField] Material _defaultMaterial;

        // Temporary objects for simulation
        Material _material;
        RenderTexture _positionBuffer1;
        RenderTexture _positionBuffer2;
        RenderTexture _velocityBuffer1;
        RenderTexture _velocityBuffer2;
        RenderTexture _rotationBuffer1;
        RenderTexture _rotationBuffer2;

        // Variables for simulation
        float _time;
        Vector3 _noiseOffset;

        // Custom properties applied to the mesh renderer.
        MaterialPropertyBlock _propertyBlock;

        // Reset flag
        bool _reconfigured = true;

        // Check if externally reconfigured.
        bool CheckReconfigured()
        {
            if (_reconfigured) return true;
            // Check if the template matches to the simulation buffer.
            if (_template != null && _positionBuffer2 != null &&
                _template.instanceCount != _positionBuffer2.width) return true;
            return false;
        }

        // Create a buffer for simulation.
        RenderTexture CreateSimulationBuffer()
        {
            var format = RenderTextureFormat.ARGBFloat;
            var width = _template.instanceCount;
            var buffer = new RenderTexture(width, 1, 0, format);
            buffer.hideFlags = HideFlags.HideAndDontSave;
            buffer.filterMode = FilterMode.Point;
            buffer.wrapMode = TextureWrapMode.Clamp;
            return buffer;
        }

        // Try to release a temporary object.
        void ReleaseObject(Object o)
        {
            if (o != null)
                if (Application.isPlaying)
                    Destroy(o);
                else
                    DestroyImmediate(o);
        }

        // Create and initialize internal temporary objects.
        void SetUpTemporaryObjects()
        {
            if (_material == null)
            {
                var shader = Shader.Find("Hidden/Skinner/Particle/Kernels");
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }

            if (_positionBuffer1 == null) _positionBuffer1 = CreateSimulationBuffer();
            if (_positionBuffer2 == null) _positionBuffer2 = CreateSimulationBuffer();
            if (_velocityBuffer1 == null) _velocityBuffer1 = CreateSimulationBuffer();
            if (_velocityBuffer2 == null) _velocityBuffer2 = CreateSimulationBuffer();
            if (_rotationBuffer1 == null) _rotationBuffer1 = CreateSimulationBuffer();
            if (_rotationBuffer2 == null) _rotationBuffer2 = CreateSimulationBuffer();
        }

        // Release internal temporary objects.
        void ReleaseTemporaryObjects()
        {
            ReleaseObject(_material); _material = null;
            ReleaseObject(_positionBuffer1); _positionBuffer1 = null;
            ReleaseObject(_positionBuffer2); _positionBuffer2 = null;
            ReleaseObject(_velocityBuffer1); _velocityBuffer1 = null;
            ReleaseObject(_velocityBuffer2); _velocityBuffer2 = null;
            ReleaseObject(_rotationBuffer1); _rotationBuffer1 = null;
            ReleaseObject(_rotationBuffer2); _rotationBuffer2 = null;
        }

        // Reset the simulation state.
        void ResetSimulationState()
        {
            _time = 0;
            _noiseOffset = Vector3.zero;

            UpdateSimulationParameters(0);

            Graphics.Blit(null, _positionBuffer2, _material, 0);
            Graphics.Blit(null, _velocityBuffer2, _material, 1);
            Graphics.Blit(null, _rotationBuffer2, _material, 2);
        }

        // Update the parameters in the simulation kernels.
        void UpdateSimulationParameters(float dt)
        {
            var m = _material;

            var invLifeMax = 1.0f / Mathf.Max(_life, 0.01f);
            var invLifeMin = invLifeMax / Mathf.Max(1 - _lifeRandomness, 0.01f);
            m.SetVector("_LifeParams", new Vector2(invLifeMin, invLifeMax));

            var drag = Mathf.Exp(-_drag * dt);
            var aparams = new Vector4(_acceleration.x, _acceleration.y, _acceleration.z, drag);
            m.SetVector("_Acceleration", aparams);

            var pi360 = Mathf.PI / 360;
            var sparams = new Vector3(_spin * pi360, _speedToSpin * pi360, _spinRandomness);
            m.SetVector("_SpinParams", sparams);

            m.SetVector("_NoiseParams", new Vector2(_noiseFrequency, _noiseAmplitude));

            // Move the noise field backward in the direction of the
            // acceleration vector, or simply pull up if no acceleration is set.
            var noiseDir = (_acceleration == Vector3.zero) ? Vector3.up : _acceleration.normalized;
            _noiseOffset += noiseDir * _noiseMotion * dt;
            m.SetVector("_NoiseOffset", _noiseOffset);

            _time += dt;
            m.SetVector("_Config", new Vector3(_throttle, dt, _time));

            m.SetFloat("_RandomSeed", _randomSeed);
        }

        // Invoke the simulation kernels.
        void InvokeSimulationKernels(float dt)
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

            // Source position information
            _material.SetTexture("_SourcePositionBuffer0", _source.previousPositionBuffer);
            _material.SetTexture("_SourcePositionBuffer1", _source.positionBuffer);

            // Invoke the position update kernel.
            UpdateSimulationParameters(dt);
            _material.SetTexture("_PositionBuffer", _positionBuffer1);
            _material.SetTexture("_VelocityBuffer", _velocityBuffer1);
            Graphics.Blit(null, _positionBuffer2, _material, 3);

            // Invoke the velocity and rotation update kernel
            // with the updated position.
            _material.SetTexture("_PositionBuffer", _positionBuffer2);
            Graphics.Blit(null, _velocityBuffer2, _material, 4);
            _material.SetTexture("_VelocityBuffer", _velocityBuffer2);
            _material.SetTexture("_RotationBuffer", _rotationBuffer1);
            Graphics.Blit(null, _rotationBuffer2, _material, 5);
        }

        // Update external components: mesh filter.
        void UpdateMeshFilter()
        {
            var meshFilter = GetComponent<MeshFilter>();

            // Add a new mesh filter if missing.
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.hideFlags = HideFlags.NotEditable;
            }

            if (meshFilter.sharedMesh != _template.mesh)
                meshFilter.sharedMesh = _template.mesh;
        }

        // Update external components: mesh renderer.
        void UpdateMeshRenderer()
        {
            var meshRenderer = GetComponent<MeshRenderer>();

            if (_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();

            _propertyBlock.SetTexture("_PositionBuffer", _positionBuffer2);
            _propertyBlock.SetTexture("_VelocityBuffer", _velocityBuffer2);
            _propertyBlock.SetTexture("_RotationBuffer", _rotationBuffer2);
            _propertyBlock.SetTexture("_PreviousPositionBuffer", _positionBuffer1);
            _propertyBlock.SetTexture("_PreviousRotationBuffer", _rotationBuffer1);
            var minScale = _scale * (1 - _scaleRandomness);
            _propertyBlock.SetVector("_Scale", new Vector2(minScale, _scale));
            _propertyBlock.SetFloat("_RandomSeed", _randomSeed);

            meshRenderer.SetPropertyBlock(_propertyBlock);

            // Set the default material if no material is set.
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = _defaultMaterial;
        }

        #endregion

        #region MonoBehaviour functions

        void Reset()
        {
            _reconfigured = true;
        }

        void OnDestroy()
        {
            ReleaseTemporaryObjects();
        }

        void Update()
        {
            // Do nothing if no template is set.
            if (_template == null) return;

            if (CheckReconfigured())
            {
                // Initialize temporary objects at the first frame,
                // and re-initialize them on configuration changes.
                ReleaseTemporaryObjects();
                SetUpTemporaryObjects();
                ResetSimulationState();
                _reconfigured = false;
            }

            if (Application.isPlaying)
            {
                // Advance simulation time.
                InvokeSimulationKernels(Time.deltaTime);
            }
            else
            {
                // Editor: Simulate 1 second from the initial state.
                ResetSimulationState();
                for (var i = 0; i < 24; i++) 
                    InvokeSimulationKernels(1.0f / 24);
            }

            // Update external components (mesh filter and renderer).
            UpdateMeshFilter();
            UpdateMeshRenderer();
        }

        #endregion
    }
}
