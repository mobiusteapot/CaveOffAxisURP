using Assets.CameraOffAxisProjection.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CameraOffAxisProjection))]
public class PointOfViewTracker : MonoBehaviour
{
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
    public void UpdatePOV() {
        Vector3 newPosition = povTransform.transform.position;
        // Rotate the X and Z relative to this object's Y rotation
        cameraOffAxis.PointOfView = newPosition;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PointOfViewTracker))]
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