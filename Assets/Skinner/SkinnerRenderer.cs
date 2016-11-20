using UnityEngine;

namespace Skinner
{
    public class SkinnerRenderer : MonoBehaviour
    {
        [SerializeField] SkinnerTemplate _template;

        public SkinnerTemplate template {
            get { return _template; }
        }

        [SerializeField, HideInInspector] Shader _shader;

        RenderTexture _positionBuffer;

        public RenderTexture positionBuffer {
            get { return _positionBuffer; }
        }

        Camera _camera;

        void Start()
        {
            _positionBuffer = new RenderTexture(
                _template.vertexCount, 1, 0, RenderTextureFormat.ARGBFloat
            );

            _camera = gameObject.AddComponent<Camera>();
            _camera.renderingPath= RenderingPath.Forward;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.targetTexture = _positionBuffer;
            _camera.farClipPlane = 2e4f;
            _camera.orthographic = true;
            _camera.hideFlags = HideFlags.HideInInspector;
            _camera.orthographicSize = 1e4f;
            _camera.SetReplacementShader(_shader, "Skinner");

            transform.position = new Vector3(0, 0, -1e4f);
        }
    }
}
