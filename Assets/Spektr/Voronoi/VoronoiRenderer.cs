using UnityEngine;
using System.Collections;

namespace Spektr
{
    [ExecuteInEditMode]
    public class VoronoiRenderer : MonoBehaviour
    {
        #region Private resources

        [SerializeField, ColorUsage(false)]
        Color _lineColor = Color.white;

        [SerializeField, ColorUsage(false)]
        Color _cellColor = Color.white;

        [SerializeField, ColorUsage(false)]
        Color _bgColor = Color.black;

        [SerializeField, Range(0, 1)]
        float _lowThreshold = 0;

        [SerializeField, Range(0, 1)]
        float _highThreshold = 1;

        [SerializeField, Range(1, 10)]
        float _cellExponent = 1;

        [SerializeField, Range(0, 1)]
        float _cellHighlight = 0.8f;

        [SerializeField]
        int _iteration = 4;

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
            // temporary color buffer
            var rtColor = RenderTexture.GetTemporary(
                source.width, source.height,
                24, RenderTextureFormat.Default
            );

            // temporary normal buffer
            var rtNormal = RenderTexture.GetTemporary(
                source.width, source.height,
                0, RenderTextureFormat.DefaultHDR
            );

            // bind them as a multi-render target
            var mrt = new RenderBuffer[2] {
                rtColor.colorBuffer, rtNormal.colorBuffer
            };
            Graphics.SetRenderTarget(mrt, rtColor.depthBuffer);

            // clear
            GL.Clear(true, true, Color.black);

            // set up the cone shader
            coneMaterial.SetTexture("_Source", source);
            var aspect = (float)source.width / source.height;
            coneMaterial.SetFloat("_Aspect", aspect);
            coneMaterial.SetFloat("_LowThreshold", _lowThreshold);
            coneMaterial.SetFloat("_HighThreshold", _highThreshold);

            // draw cones repeatedly
            for (var i = 0; i < _iteration; i++)
            {
                coneMaterial.SetPass(0);
                coneMaterial.SetFloat("_RandomSeed", i * 5);
                Graphics.DrawMeshNow(_mesh.sharedMesh, Matrix4x4.identity);
            }

            // set up the contour shader
            contourMaterial.SetTexture("_ColorTexture", rtColor);
            contourMaterial.SetTexture("_NormalTexture", rtNormal);
            contourMaterial.SetColor("_LineColor", _lineColor.gamma);
            contourMaterial.SetColor("_CellColor", _cellColor.gamma);
            contourMaterial.SetColor("_BgColor", _bgColor.gamma);
            contourMaterial.SetFloat("_CellExponent", _cellExponent);
            contourMaterial.SetFloat("_CellThreshold", _cellHighlight);

            // contour filter
            Graphics.Blit(null, destination, contourMaterial, 0);

            // dispose temporary buffers
            RenderTexture.ReleaseTemporary(rtColor);
            RenderTexture.ReleaseTemporary(rtNormal);
        }

        #endregion
    }
}
