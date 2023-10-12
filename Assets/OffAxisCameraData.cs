using Assets.CameraOffAxisProjection.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETC.CaveCavern {
    [RequireComponent(typeof(CameraOffAxisProjection))]
    public class OffAxisCameraData : MonoBehaviour {
        public float cameraOffset = 0;
        public bool isRightCamera = false;

        [SerializeField] private CaveCameraType cameraType;
        [SerializeField] private RenderTexture outputRenderTexture1;
        [SerializeField] private RenderTexture outputRenderTexture2;
        [SerializeField, HideInInspector] private CameraOffAxisProjection cameraOffAxis;

        [SerializeField] private PointOfViewTransform povTransform;
        // Todo: Shallow render-only camera?
        private void Awake() {
            if(isRightCamera){
                var ogName = this.gameObject.name;
                this.gameObject.name = ogName + "_R";
                isRightCamera = false;
                var leftCamera = Instantiate(this.gameObject, this.transform);
                leftCamera.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                isRightCamera = true;
                leftCamera.name = ogName + "_L";
                var lc = leftCamera.GetComponent<OffAxisCameraData>();
                lc.povTransform.AddPointOfViewTracker(lc);
            }    
        }
        private void OnEnable(){
            if (outputRenderTexture1 == null || outputRenderTexture2 == null) {
                Debug.LogError("OffAxisCameraData: RenderTextures are null. Disable and re-enable after assigning them.");
            } else {
                RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            }
        }
        private void OnDisable() {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;    
        }
        private void Reset() {
            this.cameraOffAxis = this.GetComponent<CameraOffAxisProjection>();
            this.povTransform = FindObjectOfType<PointOfViewTransform>();
        }

        public void OnValidate() {
            if (povTransform != null) {
                povTransform.AddPointOfViewTracker(this);
            }
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera) {
            if (camera == this.cameraOffAxis.Camera) {
                camera.targetTexture = isRightCamera ? outputRenderTexture1 : outputRenderTexture2;

                UpdatePOVPosition();
                UpdatePOVRotation();
                // Render the camera to the render texture

            }
        }

        public Camera GetPOVCamera() {
            return cameraOffAxis.Camera;
        }
        public void UpdatePOVPosition() {
            Vector3 newPosition = povTransform.transform.position;

            newPosition.x += isRightCamera ? cameraOffset : -cameraOffset;
            // Rotate the X and Z relative to this object's Y rotation
            cameraOffAxis.PointOfView = newPosition;
        }
        public void UpdatePOVRotation() {
            Vector3 newRotation = new Vector3(0, povTransform.transform.rotation.eulerAngles.y, 0);
            float rotationOffset = cameraType switch {
                CaveCameraType.Center => 0,
                CaveCameraType.Left => -90,
                CaveCameraType.Right => 90,
                _ => 0
            };
            newRotation.y += rotationOffset;
            cameraOffAxis.ViewportRotation = newRotation;
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OffAxisCameraData))]
    public class PointOfViewTrackerEditor : Editor {
        SerializedProperty cameraOffAxis;

        private void OnEnable() {
            cameraOffAxis = serializedObject.FindProperty(nameof(cameraOffAxis));
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(cameraOffAxis);

        }
    }
#endif
}