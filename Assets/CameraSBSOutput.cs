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
                // Default color is 0.5, 0.5, 0.5, 0.5
                Color defaultColor = Color.white;
                Color color1 = debugTint ? Color.red : defaultColor;
                Color color2 = debugTint ? Color.blue : defaultColor;
                // Create a temporary render texture of size Screen.width x Screen.height

                GL.PushMatrix();
                GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
                // Graphics.CopyTexture into the top and bottom halves of the render texture
                Graphics.CopyTexture(outputRenderTexture1, 0, 0, 0, 0, Screen.width, Screen.height / 2, rt, 0, 0, 0, 0);
                Graphics.CopyTexture(outputRenderTexture2, 0, 0, 0, 0, Screen.width, Screen.height / 2, rt, 0, 0, 0, Screen.height / 2);
                Graphics.CopyTexture(rt, testingPreviewRT);
                Debug.Log("Screen width/height" + Screen.width + " : " + Screen.height);
                Debug.Log("Current screen size: " + Screen.currentResolution.width + " : " + Screen.currentResolution.height);

                blitMaterial.SetColor("_BaseColor", color1);
                blitMaterial.SetColor("_SecondaryColor", color2);
                // Set the RenderTexture as global target (that means GL too)
                //RenderTexture.active = null;
                Graphics.DrawTexture(sourceRect, rt, blitMaterial);
                RenderTexture.ReleaseTemporary(rt);

                GL.PopMatrix();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D)){
                debugTint = !debugTint;
            }
        }
        // private void OnGUI() {
        //     // Tint red and blue if debugTint is true
        //     GL.sRGBWrite = false;
        //     if (debugTint) {
        //         GUI.color = Color.red;
        //         GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height/2), outputRenderTexture1, ScaleMode.ScaleToFit, false, 0);
        //         GUI.color = Color.blue;
        //         GUI.DrawTexture(new Rect(0, Screen.height/2, Screen.width, Screen.height/2), outputRenderTexture2, ScaleMode.ScaleToFit, false, 0); 
        //     } else {
        //         GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height/2), outputRenderTexture1, ScaleMode.ScaleToFit, false, 0);
        //         GUI.DrawTexture(new Rect(0, Screen.height/2, Screen.width, Screen.height/2), outputRenderTexture2, ScaleMode.ScaleToFit, false, 0); 
        //     }
        //     GL.sRGBWrite = true;
        // }
    }
}