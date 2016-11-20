using UnityEngine;

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

        RenderTexture _positionBuffer;

        public RenderTexture positionBuffer {
            get { return _positionBuffer; }
        }

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _replacementShader;
        [SerializeField, HideInInspector] Material _sourceMaterial;

        #endregion

        #region Private members

        Camera _camera;

        // Override the settings of the skinned mesh renderer.
        void OverrideRenderer()
        {
            var r = GetComponent<SkinnedMeshRenderer>();
            r.sharedMesh = _model.mesh;
            r.material = _sourceMaterial;
            //r.enabled = false; // Will be controlled by CullingStateController.
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
            _camera.targetTexture = _positionBuffer;

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
            _positionBuffer = new RenderTexture(
                _model.vertexCount, 1, 0, RenderTextureFormat.ARGBFloat
            );
            _positionBuffer.filterMode = FilterMode.Point;

            OverrideRenderer();
            BuildCamera();
        }

        void OnEnable()
        {
            if (_camera != null)
                _camera.enabled = true;
        }

        void OnDisable()
        {
            _camera.enabled = false;
        }

        void OnDestroy()
        {
            if (_positionBuffer != null)
                Destroy(_positionBuffer);
        }

        #endregion
    }
}
