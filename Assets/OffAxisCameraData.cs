using Assets.CameraOffAxisProjection.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETC.CaveCavern {
    [RequireComponent(typeof(CameraOffAxisProjection))]
    public class OffAxisCameraData : MonoBehaviour {
        public float cameraIPD = 0;
        public bool isRightCamera = false;

        [SerializeField] private CaveCameraType cameraType;
        [SerializeField] private RenderTexture outputRenderTexture1;
        [SerializeField] private RenderTexture outputRenderTexture2;
        [SerializeField, HideInInspector] private CameraOffAxisProjection cameraOffAxis;

        [SerializeField] private PointOfViewTransform povTransform;
        private Vector3 targetPosition;

        /// <summary>
        /// POV transform rotation ratio from 0 to 1, with 1 being fully offset, and 0 being no offset
        /// </summary>
        /// <param name="povTransformRot">Value from 0 to 180 for current POV transform</param>
        /// <returns></returns>
        public float GetRotationRatio(float povTransformRot) {
            float rotOffset = cameraType switch {
                CaveCameraType.Center => 0.00001f,
                CaveCameraType.Left => 90,
                CaveCameraType.Right => 90,
                _ => 0
            };
            float ratio = 1 - Mathf.Clamp(Mathf.Abs(povTransformRot - rotOffset)/ 90,0,1);
            return ratio;
        }
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
                RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            }
        }
        private void OnDisable() {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;    
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
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
            }
        }
        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera) {
            if (camera == this.cameraOffAxis.Camera) {
            }
        }

        public Camera GetPOVCamera() {
            return cameraOffAxis.Camera;
        }
        public void UpdatePOVPosition(float tranRotY) {
            targetPosition = povTransform.transform.position;
            float targetPosOffset = isRightCamera ? cameraIPD/2 : -cameraIPD/2;
            bool isLeftOrRight = cameraType == CaveCameraType.Left || cameraType == CaveCameraType.Right;

            targetPosOffset *= GetRotationRatio(tranRotY);
            if (isLeftOrRight)
                targetPosition.z += targetPosOffset;
            else targetPosition.x += targetPosOffset;
            // Rotate the X and Z relative to this object's Y rotation
            cameraOffAxis.PointOfView = targetPosition;
        }
        public void UpdateRenderTextures(RenderTexture newRT1, RenderTexture newRT2) {

            outputRenderTexture1 = newRT1;
            outputRenderTexture2 = newRT2;
        }
        public void OnDrawGizmos(){
            if (!Application.isPlaying) return;
            const float gizmoSize = 0.15f;
            Vector3 gizmoPos = povTransform.transform.position;

            Gizmos.color = isRightCamera ? Color.red : Color.blue;
            // Rotate by 90 degrees if left or right
            bool isLeftOrRight = cameraType == CaveCameraType.Left || cameraType == CaveCameraType.Right;
            if(isLeftOrRight){
                gizmoPos.z = targetPosition.z;
                Gizmos.matrix = cameraOffAxis.transform.worldToLocalMatrix;
            } else{
                gizmoPos.x = targetPosition.x;
            }
            Gizmos.DrawSphere(gizmoPos, gizmoSize);
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