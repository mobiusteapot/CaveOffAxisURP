using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PointOfViewTransform : MonoBehaviour
{
    [SerializeField, HideInInspector] private List<PointOfViewTracker> povTrackers;

    public void AddPointOfViewTracker(PointOfViewTracker tracker) {
        if (povTrackers == null) {
            povTrackers = new List<PointOfViewTracker>();
        }
        if (!povTrackers.Contains(tracker)) {
            povTrackers.Add(tracker);
        }
    }

    public void UpdateTrackers(){

        if (povTrackers == null) {
            return;
        }
        foreach (PointOfViewTracker povTracker in povTrackers) {
            povTracker.UpdatePOV();
        }
    }

    void OnDrawGizmos() {
        /*
        if (povTrackers == null) {
            return;
        }
        Matrix4x4 tempMat = Gizmos.matrix;
        foreach (PointOfViewTracker povTracker in povTrackers) {
            Camera c = povTracker.GetPOVCamera();
            Color tempColor = Gizmos.color;
            Gizmos.matrix = povTracker.transform.localToWorldMatrix;

            if (c.orthographic) {
                var size = c.orthographicSize;
                Gizmos.DrawWireCube(Vector3.forward * (c.nearClipPlane + (c.farClipPlane - c.nearClipPlane) / 2)
                    , new Vector3(size * 2.0f, size * 2.0f * c.aspect, c.farClipPlane - c.nearClipPlane));
            } else {
                Gizmos.matrix = Matrix4x4.TRS(povTracker.transform.position, povTracker.transform.rotation, Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, c.fieldOfView, c.farClipPlane, c.nearClipPlane, c.aspect);
            }
            Gizmos.color = tempColor;
           
        }
        Gizmos.matrix = tempMat;
        */
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
