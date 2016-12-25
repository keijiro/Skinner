using UnityEngine;
using System.Collections.Generic;

namespace Skinner
{
    /// Template mesh asset used in SkinnerTrail.
    public class SkinnerTrailTemplate : ScriptableObject
    {
        #region Public properties

        /// Determines how long trails can remain (specified in frames).
        public int historyLength { get { return _historyLength; } }

        [SerializeField]
        [Tooltip("Determines how long trails can remain (specified in frames).")]
        int _historyLength = 32;

        /// How many trail lines in the effect.
        public int lineCount { get { return 0xffff / (2 * _historyLength); } }

        /// Template mesh object.
        public Mesh mesh { get { return _mesh; } }

        [SerializeField] Mesh _mesh;

        #endregion

        #region Public methods

        #if UNITY_EDITOR

        public void RebuildMesh()
        {
            _mesh.Clear();

            var lcount = lineCount; // Just for avoiding recalculation.

            // Vertex list
            var vertices = new List<Vector3>();

            for (var line = 0; line < lcount; line++)
            {
                var u = (line + 0.5f) / lcount;
                for (var seg = 0; seg < _historyLength; seg++)
                {
                    var v = (seg + 0.5f) / _historyLength;
                    vertices.Add(new Vector3(u, v, -0.5f));
                    vertices.Add(new Vector3(u, v, +0.5f));
                }
            }

            _mesh.vertices = vertices.ToArray();

            // Index array
            var indices = new List<int>();
            var vi = 0;

            for (var line = 0; line < lcount; line++)
            {
                for (var seg = 0; seg < _historyLength - 1; seg++)
                {
                    indices.Add(vi + 0);
                    indices.Add(vi + 2);
                    indices.Add(vi + 1);

                    indices.Add(vi + 1);
                    indices.Add(vi + 2);
                    indices.Add(vi + 3);

                    vi += 2;
                }
                vi += 2;
            }

            _mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

            // Finishing
            _mesh.name = "Trail Template";
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.UploadMeshData(true);
        }

        #endif

        #endregion

        #region ScriptableObject functions

        void OnValidate()
        {
            _historyLength = Mathf.Clamp(_historyLength, 4, 512);
        }

        void OnEnable()
        {
            if (_mesh == null) {
                _mesh = new Mesh();
                _mesh.name = "Skinner Trail Template";
            }
        }

        #endregion
    }
}
