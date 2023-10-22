using UnityEngine;
using UnityEngine.Rendering;

namespace ETC.CaveCavern
{
    public class CameraSBSOutput : MonoBehaviour
    {
        [SerializeField] private CaveDisplayAsset caveDisplayAsset;
        [SerializeField] private Material blitMaterial;
        public bool debugTint = false;
        [SerializeField] private RenderTexture outputRenderTexture1;
        [SerializeField] private RenderTexture outputRenderTexture2;
        [SerializeField] private RenderTexture testingPreviewRT;

        public bool hasUpdatedRenderTextures = false;

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
                int currentDisplay = caveDisplayAsset.GetDisplayIndexFromCaveDisplay(CaveCameraType.Center);
                Debug.Log("rendering to display " + currentDisplay + " of display length " + Display.displays.Length);
                int cScreenWidth = Display.displays[currentDisplay].systemWidth;
                int cScreenHeight = Display.displays[currentDisplay].systemHeight;
                Debug.Log("rendering to display " + currentDisplay + " " + cScreenWidth + "x" + cScreenHeight);
                Rect screenOutRect = new Rect(0, 0, cScreenWidth, cScreenHeight);
                // Todo: Hotkey to flip eye
                Color defaultColor = Color.white;
                Color color1 = debugTint ? Color.red : defaultColor;
                Color color2 = debugTint ? Color.blue : defaultColor;
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, cScreenWidth, cScreenHeight, 0);
                RenderTexture rt = RenderTexture.GetTemporary(cScreenWidth, cScreenHeight, 0, RenderTextureFormat.ARGB32);

                if (hasUpdatedRenderTextures) {
                    RenderTexture output1 = RenderTexture.GetTemporary(cScreenWidth, cScreenHeight / 2, 0, RenderTextureFormat.ARGB32);
                    RenderTexture output2 = RenderTexture.GetTemporary(cScreenWidth, cScreenHeight / 2, 0, RenderTextureFormat.ARGB32);
                    outputRenderTexture1 = output1;
                    outputRenderTexture2 = output2;
                }

                CaveManager.instance.UpdateRenderTexture(outputRenderTexture1, outputRenderTexture2);
                // Todo: Listener for camera to apply dynamically sized render textures


                Graphics.CopyTexture(outputRenderTexture1, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, 0);
                Graphics.CopyTexture(outputRenderTexture2, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, cScreenHeight / 2);
               // Graphics.CopyTexture(rt, testingPreviewRT);
                blitMaterial.SetColor("_BaseColor", color1);
                blitMaterial.SetColor("_SecondaryColor", color2);

                Graphics.DrawTexture(screenOutRect, rt, blitMaterial);
                RenderTexture.ReleaseTemporary(rt);
                GL.PopMatrix();
            }
        }

        // Todo: Editor utilities component?
        private void Update()
        {
            // Todo: New input system editor keys?
            if (Input.GetKeyDown(KeyCode.D)){
                ToggleDebugTint();
            }
        }

        public void ToggleDebugTint() {
            debugTint = !debugTint;
        }
    }
}