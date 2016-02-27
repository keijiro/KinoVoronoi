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

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var rtTemp = RenderTexture.GetTemporary(
                source.width, source.height,
                24, RenderTextureFormat.DefaultHDR
            );

            RenderTexture.active = rtTemp;
            GL.Clear(true, true, Color.black);

            coneMaterial.SetTexture("_Source", source);

            for (var i = 0; i < 8; i++)
            {
                coneMaterial.SetPass(0);
                coneMaterial.SetFloat("_RandomSeed", i * 5);
                Graphics.DrawMeshNow(_mesh.sharedMesh, Matrix4x4.identity);
            }

            contourMaterial.SetFloat("_LowThreshold", _lowThreshold);
            contourMaterial.SetFloat("_HighThreshold", _highThreshold);
            Graphics.Blit(rtTemp, destination, contourMaterial, 0);

            RenderTexture.ReleaseTemporary(rtTemp);
        }

        #endregion
    }
}
