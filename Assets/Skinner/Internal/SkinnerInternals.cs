using UnityEngine;
using System;

namespace Skinner
{
    /// Small utilities only for internal use.
    internal static class SkinnerInternals
    {
        /// Returns the available format for animation kernel buffers.
        public static RenderTextureFormat supportedBufferFormat {
            get {
                #if UNITY_IOS || UNITY_TVOS || UNITY_ANDROID
                return RenderTextureFormat.ARGBHalf;
                #else
                return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat) ?
                    RenderTextureFormat.ARGBFloat : RenderTextureFormat.ARGBHalf;
                #endif
            }
        }
    }

    /// A generic class for managing a set of animation kernels and buffers.
    internal class AnimationKernelSet<KernelEnum, BufferEnum>
        where KernelEnum : struct
        where BufferEnum : struct
    {
        #region Enum to int converter delegates

        public delegate int KernelEnumToInt(KernelEnum e);
        public delegate int BufferEnumToInt(BufferEnum e);

        #endregion

        #region Private variables

        KernelEnumToInt _getKernelIndex;
        BufferEnumToInt _getBufferIndex;

        Shader _shader;
        Material _material;

        RenderTexture[] _buffers;
        bool _swapFlag;

        bool _ready;

        #endregion

        #region Accessor properties and functions

        /// A material that wraps up the animation kernels.
        public Material material {
            get { return _material; }
        }

        /// Returns if the kernels and buffers have been already set up.
        public bool ready {
            get { return _ready; }
        }

        /// Returns the buffer that was updated in the last frame.
        public RenderTexture GetLastBuffer(BufferEnum buffer)
        {
            var index = _getBufferIndex(buffer);
            return _buffers[_swapFlag ? index + _buffers.Length / 2 : index];
        }

        /// Return the buffer that is going to be updated in the current (or next) frame.
        public RenderTexture GetWorkingBuffer(BufferEnum buffer)
        {
            var index = _getBufferIndex(buffer);
            return _buffers[_swapFlag ? index : index + _buffers.Length / 2];
        }

        #endregion

        #region Public methods

        /// Construct with a given shader.
        /// Just initializes internal variables; does nothing serious.
        public AnimationKernelSet(Shader shader, KernelEnumToInt k2i, BufferEnumToInt b2i)
        {
            _shader = shader;
            _getKernelIndex = k2i;
            _getBufferIndex = b2i;

            // Just allocate an array for buffers.
            var enumCount = Enum.GetValues(typeof(BufferEnum)).Length;
            _buffers = new RenderTexture[enumCount * 2];
        }

        /// Initialize the kernels and the buffers when they haven't been initialized yet.
        public void Setup(int width, int height)
        {
            if (_ready) return;

            _material = new Material(_shader);

            var format = SkinnerInternals.supportedBufferFormat;

            for (var i = 0; i < _buffers.Length; i++)
            {
                var rt = new RenderTexture(width, height, 0, format);
                rt.filterMode = FilterMode.Point;
                rt.wrapMode = TextureWrapMode.Clamp;
                _buffers[i] = rt;
            }

            _swapFlag = false;
            _ready = true;
        }

        /// Destroy the kernels and the buffers when they're still alive.
        public void Release()
        {
            if (!_ready) return;

            UnityEngine.Object.Destroy(_material);
            _material = null;

            for (var i = 0; i < _buffers.Length; i++)
            {
                UnityEngine.Object.Destroy(_buffers[i]);
                _buffers[i] = null;
            }

            _ready = false;
        }

        /// Invoke a kernel and output to a given buffer.
        public void Invoke(KernelEnum kernel, BufferEnum buffer)
        {
            Graphics.Blit(null, GetWorkingBuffer(buffer), _material, _getKernelIndex(kernel));
        }

        /// Swap the double buffers.
        public void SwapBuffers()
        {
            _swapFlag = !_swapFlag;
        }

        #endregion
    }

    /// An adapter class that controls an external pair of MeshFilter and MeshRenderer.
    internal class RendererAdapter
    {
        #region Private variables

        GameObject _gameObject;
        Material _defaultMaterial;
        MaterialPropertyBlock _propertyBlock;

        #endregion

        #region Accessor properties

        /// Access to the custom material property block.
        public MaterialPropertyBlock propertyBlock {
            get { return _propertyBlock; }
        }

        #endregion

        #region Public methods

        /// Construct with given references.
        /// Just initializes internal variables; does nothing serious.
        public RendererAdapter(GameObject gameObject, Material defaultMaterial)
        {
            _gameObject = gameObject;
            _defaultMaterial = defaultMaterial;
            _propertyBlock = new MaterialPropertyBlock();
        }

        /// Update MeshFilter and MeshRenderer.
        public void Update(Mesh templateMesh)
        {
            var meshFilter = _gameObject.GetComponent<MeshFilter>();

            // Add a new mesh filter if missing.
            if (meshFilter == null)
            {
                meshFilter = _gameObject.AddComponent<MeshFilter>();
                meshFilter.hideFlags = HideFlags.NotEditable;
            }

            // Set the template mesh if not set yet.
            if (meshFilter.sharedMesh != templateMesh)
                meshFilter.sharedMesh = templateMesh;

            var meshRenderer = _gameObject.GetComponent<MeshRenderer>();

            // Set the material if no material is set.
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = _defaultMaterial;

            meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        #endregion
    }
}
