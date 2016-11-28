using UnityEngine;

namespace Skinner
{
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("Skinner/Glitch")]
    public class SkinnerGlitch : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] SkinnerSource _source;

        [SerializeField, Range(0, 1)] float _throttle = 1.0f;

        public float throttle {
            get { return _throttle; }
            set { _throttle = value; }
        }

        [SerializeField] int _randomSeed = 0;

        public int randomSeed {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }

        #endregion

        #region Private members

        // References to the built-in assets
        [SerializeField] Shader _shader;

        // Temporary objects
        Mesh _mesh;
        Material _material;

        // Custom properties applied to the mesh renderer.
        MaterialPropertyBlock _propertyBlock;

        // Create a bulk mesh.
        Mesh CreateBulkMesh()
        {
            var vcount = 21845 * 3; // 66535
            var vertices = new Vector3[vcount];
            var indices = new int[vcount];

            for (var i = 0; i < vcount; i += 3)
            {
                float u0 = (i + 0.5f) / vcount;
                float u1 = (i + 1.5f) / vcount;
                float u2 = (i + 2.5f) / vcount;

                vertices[i + 0] = new Vector3(u0, u1, u2);
                vertices[i + 1] = new Vector3(u1, u2, u0);
                vertices[i + 2] = new Vector3(u2, u0, u1);

                indices[i + 0] = i + 0;
                indices[i + 1] = i + 1;
                indices[i + 2] = i + 2;
            }

            var mesh = new Mesh();
            mesh.name = "Glitch";
            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            mesh.Optimize();
            mesh.UploadMeshData(true);

            return mesh;
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
                _material = new Material(Shader.Find("Hidden/Skinner/Glitch"));
                _material.hideFlags = HideFlags.HideAndDontSave;
            }

            if (_mesh == null)
            {
                _mesh = CreateBulkMesh();
                _mesh.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        // Release internal temporary objects.
        void ReleaseTemporaryObjects()
        {
            ReleaseObject(_material);
            _material = null;

            ReleaseObject(_mesh);
            _mesh = null;
        }

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

            if (meshFilter.sharedMesh != _mesh)
                meshFilter.sharedMesh = _mesh;
        }

        // Update external component: mesh renderer
        void UpdateMeshRenderer()
        {
            var meshRenderer = GetComponent<MeshRenderer>();

            if (_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();

            _propertyBlock.SetTexture("_PositionBuffer", _source.positionBuffer);
            _propertyBlock.SetFloat("_RandomSeed", _randomSeed);

            meshRenderer.SetPropertyBlock(_propertyBlock);

            // Set the material if no material is set.
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = _material;
        }

        #endregion

        #region MonoBehaviour functions

        void OnDestroy()
        {
            ReleaseTemporaryObjects();
        }

        void LateUpdate()
        {
            // Do nothing if no source is given.
            if (_source == null) return;

            // Initialize the temporary objects if not yet.
            SetUpTemporaryObjects();

            // Update external components (mesh filter and renderer).
            UpdateMeshFilter();
            UpdateMeshRenderer();
        }

        #endregion
    }
}
