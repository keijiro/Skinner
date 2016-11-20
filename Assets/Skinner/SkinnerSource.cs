using UnityEngine;
using UnityEngine.Rendering;

namespace Skinner
{
    public class SkinnerSource : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] SkinnerModel _model;

        public SkinnerModel model {
            get { return _model; }
        }

        #endregion

        #region Public property (runtime only)

        public RenderTexture positionBuffer {
            get { return _swapFlag ? _positionBuffer1 : _positionBuffer0; }
        }

        public RenderTexture previousPositionBuffer {
            get { return _swapFlag ? _positionBuffer0 : _positionBuffer1; }
        }

        public RenderTexture normalBuffer {
            get { return _normalBuffer; }
        }

        public RenderTexture tangentBuffer {
            get { return _tangentBuffer; }
        }

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _replacementShader;
        [SerializeField, HideInInspector] Material _sourceMaterial;

        #endregion

        #region Private members

        Camera _camera;
        RenderTexture _positionBuffer0;
        RenderTexture _positionBuffer1;
        RenderTexture _normalBuffer;
        RenderTexture _tangentBuffer;
        RenderBuffer[] _mrt0;
        RenderBuffer[] _mrt1;
        bool _swapFlag;

        // Create a render texture used as a vector buffer.
        RenderTexture CreateBuffer()
        {
            var rt = new RenderTexture(_model.vertexCount, 1, 0, RenderTextureFormat.ARGBFloat);
            rt.filterMode = FilterMode.Point;
            return rt;
        }

        // Override the settings of the skinned mesh renderer.
        void OverrideRenderer()
        {
            var r = GetComponent<SkinnedMeshRenderer>();
            r.sharedMesh = _model.mesh;
            r.material = _sourceMaterial;
            r.receiveShadows = false;
            r.enabled = false; // Will be controlled by CullingStateController.
        }

        // Create a camera used for rendering the position buffer.
        void BuildCamera()
        {
            // Create a new game object.
            var go = new GameObject("Camera");
            go.hideFlags = HideFlags.HideInHierarchy;

            // Make it child of this node.
            var tr = go.transform;
            tr.parent = transform;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;

            // Add a camera to the game object.
            _camera = go.AddComponent<Camera>();

            _camera.renderingPath= RenderingPath.Forward;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.depth = -10000;

            _camera.nearClipPlane = -100;
            _camera.farClipPlane = 100;
            _camera.orthographic = true;
            _camera.orthographicSize = 100;

            _camera.SetReplacementShader(_replacementShader, "Skinner");

            // Add a culling state controller to hide this object from other cameras.
            var culler = go.AddComponent<CullingStateController>();
            culler.target = GetComponent<SkinnedMeshRenderer>();
        }

        #endregion

        #region MonoBehaviour functions

        void Start()
        {
            _positionBuffer0 = CreateBuffer();
            _positionBuffer1 = CreateBuffer();
            _normalBuffer = CreateBuffer();
            _tangentBuffer = CreateBuffer();

            // MRT set 0
            _mrt0 = new [] {
                _positionBuffer0.colorBuffer,
                _normalBuffer.colorBuffer,
                _tangentBuffer.colorBuffer
            };

            // MRT set 1
            _mrt1 = new [] {
                _positionBuffer1.colorBuffer,
                _normalBuffer.colorBuffer,
                _tangentBuffer.colorBuffer
            };

            OverrideRenderer();
            BuildCamera();

            // Set MRT 0
            _camera.SetTargetBuffers(_mrt0, _positionBuffer0.depthBuffer);
        }

        void OnEnable()
        {
            if (_camera != null) _camera.enabled = true;
        }

        void OnDisable()
        {
            _camera.enabled = false;
        }

        void OnDestroy()
        {
            if (_positionBuffer0 != null) Destroy(_positionBuffer0);
            if (_positionBuffer1 != null) Destroy(_positionBuffer1);
            if (_normalBuffer != null) Destroy(_normalBuffer);
            if (_tangentBuffer != null) Destroy(_tangentBuffer);
        }

        void Update()
        {
            _swapFlag = !_swapFlag;

            if (_swapFlag)
                _camera.SetTargetBuffers(_mrt1, _positionBuffer1.depthBuffer);
            else
                _camera.SetTargetBuffers(_mrt0, _positionBuffer0.depthBuffer);
        }

        #endregion
    }
}
