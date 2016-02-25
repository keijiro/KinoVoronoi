using UnityEngine;
using System.Collections.Generic;

namespace Spektr
{
    public class VoronoiMesh : ScriptableObject
    {
        #region Public Properties

        [SerializeField]
        int _pointCount = 20;

        public int pointCount {
            get { return _pointCount; }
        }

        [SerializeField]
        int _coneResolution = 32;

        public int coneResolution {
            get { return _coneResolution; }
        }

        [SerializeField, HideInInspector]
        Mesh _mesh;

        public Mesh sharedMesh {
            get { return _mesh; }
        }

        #endregion

        #region Public Methods

        public void RebuildMesh()
        {
            if (_mesh == null)
            {
                Debug.LogError("Mesh asset is missing.");
                return;
            }

            _mesh.Clear();

            var v_per_c = Mathf.Max(_coneResolution, 6); // vertices per cone
            var c_count = Mathf.Max(_pointCount, 1);     // cone count

            var v_array = new List<Vector3>((1 + v_per_c) * c_count);
            var t_array = new List<Vector2>((1 + v_per_c) * c_count);
            var i_array = new List<int>(v_per_c * 3 * c_count);

            // vertex array: first cone
            v_array.Add(-Vector3.forward);
            t_array.Add(Vector2.zero);

            for (var v_i = 0; v_i < v_per_c; v_i++)
            {
                var r = Mathf.PI * 2 / v_per_c * v_i;
                v_array.Add(new Vector3(Mathf.Sin(r), Mathf.Cos(r), 0));
                t_array.Add(new Vector2(v_i + 1, 0));
            }

            // vertex array: populate the cone
            for (var c_i = 1; c_i < c_count; c_i++)
            {
                for (var v_i = 0; v_i < v_per_c + 1; v_i++)
                {
                    v_array.Add(v_array[v_i]);
                    t_array.Add(new Vector2(v_i, c_i));
                }
            }

            // index array
            for (var c_i = 0; c_i < c_count; c_i++)
            {
                var offs = (1 + v_per_c) * c_i;
                for (var v_i = 0; v_i < v_per_c; v_i++)
                {
                    i_array.Add(offs);
                    i_array.Add(offs + 1 + v_i);
                    i_array.Add(offs + 1 + (v_i + 1) % v_per_c);
                }
            }

            _mesh.SetVertices(v_array);
            _mesh.SetUVs(0, t_array);
            _mesh.SetIndices(i_array.ToArray(), MeshTopology.Triangles, 0);

            // very bad way to avoid being culled. don't do at home.
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

            _mesh.Optimize();
            _mesh.RecalculateNormals();
            _mesh.UploadMeshData(true);
        }

        #endregion

        #region ScriptableObject Functions

        void OnEnable()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "Cones";
            }
        }

        #endregion
    }
}
