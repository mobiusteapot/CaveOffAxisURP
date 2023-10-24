using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETC.CaveCavern {
    public class PointOfViewTransform : MonoBehaviour {
        [SerializeField] private TrackedPoseDriver trackedPoseDriver;
        private Quaternion initialRotationOffset;
        bool hasInitialRotationOffset = false;
        public float cameraOffset = 0;
        private float povInitialYRot = 0;

        [SerializeField, HideInInspector] private List<OffAxisCameraData> povTrackers;
        private bool invertCameras = false;
        public float PovYRot {
            get {
                float normalizedRot = transform.rotation.y;
                normalizedRot = Mathf.Abs(normalizedRot) * 1.425f * 90f;
                normalizedRot = invertCameras ? 180 - normalizedRot : normalizedRot;
                return normalizedRot;
            }
        }
        private void Awake(){
            povInitialYRot = PovYRot;
        }
        private void ApplyRotationOffset() {
            // Get initial rotation of tracked pose driver to offset the rotation of the POV transform
            initialRotationOffset = trackedPoseDriver.transform.rotation;
            // Set the transform by the offset because the tracked pose driver is a parent with an arbitrary rotation
            // New rotation is the inverse of the tracked pose driver rotation
            transform.rotation = Quaternion.Inverse(initialRotationOffset);
            Debug.Log("Initial Rotation Offset:" + initialRotationOffset.eulerAngles);
        }
        public void AddPointOfViewTracker(OffAxisCameraData tracker) {
            if (povTrackers == null) {
                povTrackers = new List<OffAxisCameraData>();
            }
            if (!povTrackers.Contains(tracker)) {
                povTrackers.Add(tracker);
            }
        }

        private void Update() {
            if(!hasInitialRotationOffset && trackedPoseDriver.transform.rotation != quaternion.identity) {
                ApplyRotationOffset();
                hasInitialRotationOffset = true;
            }
            if (this.transform.hasChanged) {
                UpdateTrackers();
            }
        }
        public float GetPOVTransformRotation(){
            return Mathf.Abs(povInitialYRot - PovYRot);
        }

        public void UpdateTrackers() {

            if (povTrackers == null) {
                return;
            }
            Debug.Log("PovRotation:" + GetPOVTransformRotation());
            foreach (OffAxisCameraData povTracker in povTrackers) {
                povTracker.cameraIPD = cameraOffset;
                povTracker.UpdatePOVPosition(GetPOVTransformRotation());
            }
        }
        // Remove this later this makes no sense here
        public void UpdateRenderTextureForCameras(RenderTexture newRT1, RenderTexture newRT2) {
            foreach (OffAxisCameraData povTracker in povTrackers) {
                povTracker.UpdateRenderTextures(newRT1, newRT2);
            }
        }
        public void UpdateCameraOffset(string cameraOffsetString)
        {
            float.TryParse(cameraOffsetString, out cameraOffset);
        }
        public void ToggleInvertCameras() {
            invertCameras = !invertCameras;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PointOfViewTransform))]
    public class PointOfViewTransformEditor : Editor {
        SerializedProperty povTrackers;

        private void OnEnable() {
            povTrackers = serializedObject.FindProperty(nameof(povTrackers));
            EditorApplication.update += UpdateTrackersInEditor;
        }
        private void OnDisable() {
            EditorApplication.update -= UpdateTrackersInEditor;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            using (new EditorGUI.DisabledScope(true)) {
                EditorGUILayout.PropertyField(povTrackers);
            }
        }
        public void UpdateTrackersInEditor() {
            ((PointOfViewTransform)target).UpdateTrackers();
        }

        //protected void OnSceneGUI() {
        //    if (Event.current.type == EventType.Repaint) {
        //        Handles.color = Color.white;
        //        Handles.SphereHandleCap(
        //          1,
        //          target.,
        //          this.cameraOffAxisProjection.transform.rotation,
        //          0.1f,
        //          EventType.Repaint);
        //    }

        //    EditorGUI.BeginChangeCheck();

        //    var position = Handles.PositionHandle(this.cameraOffAxisProjection.WorldPointOfView, this.cameraOffAxisProjection.transform.rotation);

        //    if (EditorGUI.EndChangeCheck()) {
        //        this.cameraOffAxisProjection.WorldPointOfView = position;

        //        if (!Application.isPlaying) {
        //            Undo.RegisterCompleteObjectUndo(this.cameraOffAxisProjection, this.cameraOffAxisProjection.GetType().FullName);
        //            EditorUtility.SetDirty(this.cameraOffAxisProjection);
        //        }
        //    }
        //}
    }
#endif
}
