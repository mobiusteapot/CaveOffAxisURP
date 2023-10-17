using UnityEngine;
using UnityEngine.Rendering;

namespace ETC.CaveCavern
{
    public class CameraSBSOutput : MonoBehaviour
    {
        [SerializeField] private Rect sourceRect;
        [SerializeField] private Material blitMaterial;
        public bool debugTint = false;
        [SerializeField] private RenderTexture outputRenderTexture1;
        [SerializeField] private RenderTexture outputRenderTexture2;
        [SerializeField] private RenderTexture testingPreviewRT;

        private void OnEnable()
        {
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
            if (outputRenderTexture1 == null || outputRenderTexture2 == null)
            {
                Debug.LogError("OffAxisCameraData: RenderTextures are null. Disable and re-enable after assigning them.");
            }
            else
            {
                RenderPipelineManager.endCameraRendering += OnBeginCameraRendering;
            }
        }
        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnBeginCameraRendering;
        }
        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            Debug.Log("attempting render");
            if (camera == this.GetComponent<Camera>())
            {
                // Todo: Hotkey to flip eye
                Color defaultColor = Color.white;
                Color color1 = debugTint ? Color.red : defaultColor;
                Color color2 = debugTint ? Color.blue : defaultColor;
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
                Graphics.CopyTexture(outputRenderTexture1, 0, 0, 0, 0, Screen.width, Screen.height / 2, rt, 0, 0, 0, 0);
                Graphics.CopyTexture(outputRenderTexture2, 0, 0, 0, 0, Screen.width, Screen.height / 2, rt, 0, 0, 0, Screen.height / 2);
                Graphics.CopyTexture(rt, testingPreviewRT);
                blitMaterial.SetColor("_BaseColor", color1);
                blitMaterial.SetColor("_SecondaryColor", color2);

                Graphics.DrawTexture(sourceRect, rt, blitMaterial);
                RenderTexture.ReleaseTemporary(rt);
                GL.PopMatrix();
            }
        }

        private void Update()
        {
            // Todo: New input system editor keys?
            if (Input.GetKeyDown(KeyCode.D)){
                debugTint = !debugTint;
            }
        }
    }
}