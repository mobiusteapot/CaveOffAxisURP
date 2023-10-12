using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace ETC.CaveCavern
{
    public class CameraSBSOutput : MonoBehaviour
    {
        public bool debugTint = false;
        [SerializeField] private RenderTexture outputRenderTexture1;
        [SerializeField] private RenderTexture outputRenderTexture2;
        private void OnEnable()
        {
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
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                Graphics.DrawTexture(new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2), outputRenderTexture1);
                Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height / 2), outputRenderTexture2);

              //  Graphics.DrawTexture(new Rect(-Screen.width/2, -Screen.height / 2, Screen.width, Screen.height/2), outputRenderTexture2, new Rect(0, 0, 1, 1), 0, 0, 0, 0, defaultColor);

               // Graphics.DrawTexture(new Rect(-Screen.width/2, 0, Screen.width, Screen.height/2), outputRenderTexture1, new Rect(0, 0, outputRenderTexture1.width, outputRenderTexture1.height), 0, 0, 0, 0, defaultColor);
                // Tint red and blue if debugTint is true
                if(debugTint){
                    Graphics.DrawTexture(new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2), outputRenderTexture1, new Rect(0, 0, 1, 1), 0, 0, 0, 0, Color.red);
                    Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height / 2), outputRenderTexture2, new Rect(0, 0, 1, 1), 0, 0, 0, 0, Color.blue);
                }
                GL.PopMatrix();
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