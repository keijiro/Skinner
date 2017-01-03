using UnityEngine;
using UnityEngine.Rendering;

namespace Skinner
{
    /// Bakes vertex attributes of a skinned mesh into textures
    /// and provides them to Skinner renderers.
    [AddComponentMenu("Skinner/Skinner Source")]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class SkinnerSource : MonoBehaviour
    {
        #region Editable properties

        // This is only editable on Editor (not changeable at runtime).

        [SerializeField, Tooltip("Preprocessed model data.")]
        SkinnerModel _model;

        #endregion

        #region Public properties

        /// The count of vertices of the source model.
        public int vertexCount {
            get { return _model != null ? _model.vertexCount : 0; }
        }

        /// Checks if the buffers are ready for use.
        public bool isReady {
            get { return _frameCount > 1; }
        }

        /// Baked texture of skinned vertex positions.
        public RenderTexture positionBuffer {
            get { return _swapFlag ? _positionBuffer1 : _positionBuffer0; }
        }

        /// Baked texture of skinned vertex positions from the previous frame.
        public RenderTexture previousPositionBuffer {
            get { return _swapFlag ? _positionBuffer0 : _positionBuffer1; }
        }

        /// Baked texture of skinned vertex normals.
        public RenderTexture normalBuffer {
            get { return _normalBuffer; }
        }

        /// Baked texture of skinned vertex tangents.
        public RenderTexture tangentBuffer {
            get { return _tangentBuffer; }
        }

        #endregion

        #region Internal resources

        // Replacement shader used for baking vertex attributes.
        [SerializeField] Shader _replacementShader;

        // Placeholder material that draws nothing but only has the replacement tag.
        [SerializeField] Material _placeholderMaterial;

        #endregion

        #region Private members

        // Vertex attribute buffers.
        RenderTexture _positionBuffer0;
        RenderTexture _positionBuffer1;
        RenderTexture _normalBuffer;
        RenderTexture _tangentBuffer;

        // Multiple render target for even/odd frames.
        RenderBuffer[] _mrt0;
        RenderBuffer[] _mrt1;
        bool _swapFlag;

        // Temporary camera used for vertex baking.
        Camera _camera;

        // Used for rejecting the first and second frame.
        int _frameCount;

        // Create a render texture for vertex baking.
        RenderTexture CreateBuffer()
        {
            var format = SkinnerInternals.supportedBufferFormat;
            var rt = new RenderTexture(_model.vertexCount, 1, 0, format);
            rt.filterMode = FilterMode.Point;
            return rt;
        }

        // Override the settings of the skinned mesh renderer.
        void OverrideRenderer()
        {
            var smr = GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = _model.mesh;
            smr.material = _placeholderMaterial;
            smr.receiveShadows = false;

            // This renderer is disabled to hide from other cameras. It will be
            // enable by CullingStateController only while rendered from our
            // vertex baking camera.
            smr.enabled = false;
        }

        // Create a camera for vertex baking.
        void BuildCamera()
        {
            // Create a new game object.
            var go = new GameObject("Camera");
            go.hideFlags = HideFlags.HideInHierarchy;

            // Make it parented by this game object.
            var tr = go.transform;
            tr.parent = transform;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;

            // Add a camera to the game object.
            _camera = go.AddComponent<Camera>();

            _camera.renderingPath= RenderingPath.Forward;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.depth = -10000; // Is this a right way to control the order?

            _camera.nearClipPlane = -100;
            _camera.farClipPlane = 100;
            _camera.orthographic = true;
            _camera.orthographicSize = 100;

            _camera.SetReplacementShader(_replacementShader, "Skinner");
            _camera.enabled = false; // We'll explicitly call Render().

            // Add CullingStateController to hide from other cameras.
            var culler = go.AddComponent<CullingStateController>();
            culler.target = GetComponent<SkinnedMeshRenderer>();
        }

        #endregion

        #region MonoBehaviour functions

        void Start()
        {
            // Create the attribute buffers.
            _positionBuffer0 = CreateBuffer();
            _positionBuffer1 = CreateBuffer();
            _normalBuffer = CreateBuffer();
            _tangentBuffer = CreateBuffer();

            // MRT set 0 (used in even frames)
            _mrt0 = new [] {
                _positionBuffer0.colorBuffer,
                _normalBuffer.colorBuffer,
                _tangentBuffer.colorBuffer
            };

            // MRT set 1 (used in odd frames)
            _mrt1 = new [] {
                _positionBuffer1.colorBuffer,
                _normalBuffer.colorBuffer,
                _tangentBuffer.colorBuffer
            };

            // Set up the baking rig.
            OverrideRenderer();
            BuildCamera();

            _swapFlag = true; // This will become false by the first Update call.
        }

        void OnDestroy()
        {
            if (_positionBuffer0 != null) Destroy(_positionBuffer0);
            if (_positionBuffer1 != null) Destroy(_positionBuffer1);
            if (_normalBuffer != null) Destroy(_normalBuffer);
            if (_tangentBuffer != null) Destroy(_tangentBuffer);
        }

        void LateUpdate()
        {
            // Swap the buffers and invoke vertex baking.
            _swapFlag = !_swapFlag;
            if (_swapFlag)
                _camera.SetTargetBuffers(_mrt1, _positionBuffer1.depthBuffer);
            else
                _camera.SetTargetBuffers(_mrt0, _positionBuffer0.depthBuffer);
            _camera.Render();
            _frameCount++;
        }

        #endregion
    }
}
