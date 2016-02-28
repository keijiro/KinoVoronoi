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
            var rtColor = RenderTexture.GetTemporary(
                source.width, source.height,
                24, RenderTextureFormat.Default
            );

            var rtNormal = RenderTexture.GetTemporary(
                source.width, source.height,
                0, RenderTextureFormat.DefaultHDR
            );

            var mrt = new RenderBuffer[2] {
                rtColor.colorBuffer, rtNormal.colorBuffer
            };

            Graphics.SetRenderTarget(mrt, rtColor.depthBuffer);
            GL.Clear(true, true, Color.black);

            coneMaterial.SetTexture("_Source", source);

            var aspect = (float)source.width / source.height;
            coneMaterial.SetFloat("_Aspect", aspect);

            for (var i = 0; i < 6; i++)
            {
                coneMaterial.SetPass(0);
                coneMaterial.SetFloat("_RandomSeed", i * 5);
                Graphics.DrawMeshNow(_mesh.sharedMesh, Matrix4x4.identity);
            }

            contourMaterial.SetTexture("_ColorTexture", rtColor);
            contourMaterial.SetTexture("_NormalTexture", rtNormal);
            contourMaterial.SetFloat("_LowThreshold", _lowThreshold);
            contourMaterial.SetFloat("_HighThreshold", _highThreshold);
            Graphics.Blit(null, destination, contourMaterial, 0);

            RenderTexture.ReleaseTemporary(rtColor);
            RenderTexture.ReleaseTemporary(rtNormal);
        }

        #endregion
    }
}
