using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETC.CaveCavern {
    public class CaveManager : MonoBehaviour {
        // Cave singleton

        public static CaveManager instance;
        [SerializeField] private PointOfViewTransform PointOfViewTransform;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(this);
            }
        }

        public void UpdateRenderTexture(RenderTexture newRT1, RenderTexture newRT2) {
            PointOfViewTransform.UpdateRenderTextureForCameras(newRT1, newRT2);
        }
    }
}
