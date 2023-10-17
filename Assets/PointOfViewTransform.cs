using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETC.CaveCavern {
    public class PointOfViewTransform : MonoBehaviour {
        public float cameraOffset = 0;
        private float povInitialY = 0;
        [SerializeField, HideInInspector] private List<OffAxisCameraData> povTrackers;

        public float PovYRot {
            get {
                float normalizedRot = transform.rotation.y;
                normalizedRot = Mathf.Abs(normalizedRot) * 1.425f * 90f;
                return normalizedRot;
            }
        }
        private void Awake(){
            povInitialY = PovYRot;
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
            if (this.transform.hasChanged) {
                UpdateTrackers();
            }
        }
        public float GetPOVTransformRotation(){
            return Mathf.Abs(PovYRot - povInitialY);
        }

        public void UpdateTrackers() {

            if (povTrackers == null) {
                return;
            }
            foreach (OffAxisCameraData povTracker in povTrackers) {
                povTracker.cameraIPD = cameraOffset;
                povTracker.UpdatePOVPosition(GetPOVTransformRotation());
            }
        }

        public void UpdateCameraOffset(string cameraOffsetString)
        {

            float.TryParse(cameraOffsetString, out cameraOffset);
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
