using UnityEngine;
using UnityEngine.Rendering;

namespace ETC.CaveCavern
{
    public class CameraSBSOutput : MonoBehaviour
    {
        [SerializeField] private CaveDisplayAsset caveDisplayAsset;
        // [SerializeField] private Material blitMaterial;
        public bool debugTint = false;
        [SerializeField] private RenderTexture outputRenderTexture1Left;
        [SerializeField] private RenderTexture outputRenderTexture2Left;
        [SerializeField] private RenderTexture outputRenderTexture1Center;
        [SerializeField] private RenderTexture outputRenderTexture2Center;
        [SerializeField] private RenderTexture outputRenderTexture1Right;
        [SerializeField] private RenderTexture outputRenderTexture2Right;
        [SerializeField] private RenderTexture testingPreviewRT;
        [SerializeField] private CaveCameraType cameraType;

        public bool hasUpdatedRenderTextures = false;

        private void OnEnable()
        {
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
            RenderPipelineManager.endCameraRendering += OnBeginCameraRendering;
        }
        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnBeginCameraRendering;
        }
        public void ToggleCurrentRenderTexture() {
            hasUpdatedRenderTextures = !hasUpdatedRenderTextures;
        }
        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            Debug.Log("attempting render");
            if (camera == this.GetComponent<Camera>())
            {
#if UNITY_EDITOR
                int currentDisplay = 0;
#else
                int currentDisplay = caveDisplayAsset.GetDisplayIndexFromCaveDisplay(cameraType);
#endif
                Debug.Log("rendering to display " + currentDisplay + " of display length " + Display.displays.Length);
                int cScreenWidth = Display.displays[currentDisplay].systemWidth;
                int cScreenHeight = Display.displays[currentDisplay].systemHeight;
                Debug.Log("rendering to display " + currentDisplay + " " + cScreenWidth + "x" + cScreenHeight);
                Rect screenOutRect = new Rect(0, 0, cScreenWidth, cScreenHeight);
                // Todo: Hotkey to flip eye
                Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                Color color1 = debugTint ? Color.red : defaultColor;
                Color color2 = debugTint ? Color.blue : defaultColor;
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, cScreenWidth, cScreenHeight, 0);
                RenderTexture rt = RenderTexture.GetTemporary(cScreenWidth, cScreenHeight, 0, RenderTextureFormat.ARGB32);

                /*
                if (hasUpdatedRenderTextures) {
                    RenderTexture output1 = RenderTexture.GetTemporary(cScreenWidth, cScreenHeight / 2, 0, RenderTextureFormat.ARGB32);
                    RenderTexture output2 = RenderTexture.GetTemporary(cScreenWidth, cScreenHeight / 2, 0, RenderTextureFormat.ARGB32);
                    hasUpdatedRenderTextures = false;
                }
                */
                // Todo: Listener for camera to apply dynamically sized render textures

                switch (cameraType) {
                    // Switch on camera type
                    case CaveCameraType.Left:
                        Graphics.CopyTexture(outputRenderTexture1Left, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, 0);
                        Graphics.CopyTexture(outputRenderTexture2Left, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, cScreenHeight / 2);
                        break;
                    case CaveCameraType.Center:
                        Graphics.CopyTexture(outputRenderTexture1Center, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, 0);
                        Graphics.CopyTexture(outputRenderTexture2Center, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, cScreenHeight / 2);
                        break;
                    case CaveCameraType.Right:
                        Graphics.CopyTexture(outputRenderTexture1Right, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, 0);
                        Graphics.CopyTexture(outputRenderTexture2Right, 0, 0, 0, 0, cScreenWidth, cScreenHeight / 2, rt, 0, 0, 0, cScreenHeight / 2);
                        break;
                }
                Graphics.CopyTexture(rt, testingPreviewRT);
               // blitMaterial.SetColor("_BaseColor", color1);
               // blitMaterial.SetColor("_SecondaryColor", color2);

               // Graphics.DrawTexture(screenOutRect, rt);
                Graphics.DrawTexture(screenOutRect, rt, new Rect(0,0,1,1), 0, 0, 0, 0, defaultColor, null);
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