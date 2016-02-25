using UnityEngine;
using System.Collections;

namespace Spektr
{
    [ExecuteInEditMode]
    public class VoronoiRenderer : MonoBehaviour
    {
        #region Private resources

        [SerializeField]
        VoronoiMesh _mesh;

        [SerializeField, HideInInspector]
        Shader _shader;

        Material material {
            get {
                if (_material == null) {
                    var shader = Shader.Find("Hidden/Spektr/Voronoi");
                    _material = new Material(shader);
                    _material.hideFlags = HideFlags.DontSave;
                }
                return _material;
            }
        }

        Material _material;

        #endregion

        #region MonoBehaviour Functions

        void Update()
        {
            Graphics.DrawMesh(
                _mesh.sharedMesh,
                transform.localToWorldMatrix,
                material, 0, null, 0);
        }

        #endregion
    }
}
