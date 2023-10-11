using Assets.CameraOffAxisProjection.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETC.CaveCavern {
    [RequireComponent(typeof(CameraOffAxisProjection))]
    public class OffAxisCameraData : MonoBehaviour {
        [SerializeField] private CaveCameraType cameraType;
        [SerializeField, HideInInspector] private CameraOffAxisProjection cameraOffAxis;

        [SerializeField] private PointOfViewTransform povTransform;

        private void Reset() {
            this.cameraOffAxis = this.GetComponent<CameraOffAxisProjection>();
            this.povTransform = FindObjectOfType<PointOfViewTransform>();
        }

        private void OnValidate() {
            if (povTransform != null) {
                povTransform.AddPointOfViewTracker(this);
            }
        }

        public Camera GetPOVCamera() {
            return cameraOffAxis.Camera;
        }
        public void UpdatePOVPosition() {
            Vector3 newPosition = povTransform.transform.position;
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