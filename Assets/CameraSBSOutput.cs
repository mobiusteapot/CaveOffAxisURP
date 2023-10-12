using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace ETC.CaveCavern
{
    public class CameraSBSOutput : MonoBehaviour
    {
        [SerializeField] private Rect sourceRect;
        public bool debugTint = false;
        [SerializeField] private RenderTexture outputRenderTexture1;
        [SerializeField] private RenderTexture outputRenderTexture2;
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

                // Default color is 0.5, 0.5, 0.5, 0.5
                Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Color color1 = debugTint ? Color.red : defaultColor;
                Color color2 = debugTint ? Color.blue : defaultColor;

                GL.PushMatrix();
                GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);

                Graphics.DrawTexture(new Rect(0, Screen.height / 4, Screen.width, Screen.height / 2), outputRenderTexture1, sourceRect, 0, 0, 0, 0, color1);
                Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height / 2), outputRenderTexture2, sourceRect, 0, 0, 0, 0, color2);

                GL.PopMatrix();
                RenderTexture.active = null;
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