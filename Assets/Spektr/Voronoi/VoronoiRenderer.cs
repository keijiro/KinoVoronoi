using UnityEngine;
using System.Collections;

namespace Spektr
{
    [ExecuteInEditMode]
    public class VoronoiRenderer : MonoBehaviour
    {
        #region Private resources

        [SerializeField]
        float _lowThreshold;

        [SerializeField]
        float _highThreshold;

        [SerializeField]
        VoronoiMesh _mesh;

        [SerializeField] Shader _coneShader;
        [SerializeField] Shader _contourShader;

        Material coneMaterial {
            get {
                if (_coneMaterial == null) {
                    var shader = Shader.Find("Hidden/Spektr/Voronoi/Cone");
                    _coneMaterial = new Material(shader);
                    _coneMaterial.hideFlags = HideFlags.DontSave;
                }
                return _coneMaterial;
            }
        }

        Material _coneMaterial;

        Material contourMaterial {
            get {
                if (_contourMaterial == null) {
                    var shader = Shader.Find("Hidden/Spektr/Voronoi/Contour");
                    _contourMaterial = new Material(shader);
                    _contourMaterial.hideFlags = HideFlags.DontSave;
                }
                return _contourMaterial;
            }
        }

        Material _contourMaterial;

        #endregion

        #region MonoBehaviour Functions

        void Update()
        {
            Graphics.DrawMesh(
                _mesh.sharedMesh,
                transform.localToWorldMatrix,
                coneMaterial, 0, null, 0);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            contourMaterial.SetFloat("_LowThreshold", _lowThreshold);
            contourMaterial.SetFloat("_HighThreshold", _highThreshold);
            Graphics.Blit(source, destination, contourMaterial, 0);
        }

        #endregion
    }
}
