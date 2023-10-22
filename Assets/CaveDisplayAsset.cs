using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETC.CaveCavern
{
    [Serializable]
    public struct DisplayBinding {
        public CaveCameraType caveDisplay;
        public UnityDisplay unityDisplay;

        public int GetDisplayIndex() {
            return unityDisplay switch {
                UnityDisplay.Display1 => 0,
                UnityDisplay.Display2 => 1,
                UnityDisplay.Display3 => 2,
                UnityDisplay.Display4 => 3,
                _ => 0,
            };
        }
    }
    [CreateAssetMenu(fileName ="Cave Display Output")]
    public class CaveDisplayAsset : ScriptableObject
    {
        //   public Rect outputDisplayRect;
        [SerializeField] private DisplayBinding displayBinding1;
        [SerializeField] private DisplayBinding displayBinding2;
        [SerializeField] private DisplayBinding displayBinding3;

        public int GetDisplayIndexFromCaveDisplay(CaveCameraType targetDisplay) {
            return BindingFromCaveDisplay(targetDisplay).GetDisplayIndex();
        }
        public DisplayBinding BindingFromCaveDisplay(CaveCameraType targetDisplay) {
            // Return the first binding that matches the target display
            foreach (DisplayBinding displayBinding in new DisplayBinding[] { displayBinding1, displayBinding2, displayBinding3 }) {
                if (displayBinding.caveDisplay == targetDisplay)
                    return displayBinding;
            }
            Debug.LogError("Did not find matching display binding for " + targetDisplay);
            return displayBinding1;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(CaveDisplayAsset))]
    public class CaveDisplayAssetEditor : Editor
    {
        //const int resolutionMax = 8192;
        //SerializedProperty outputDisplayRect;
        //SerializedProperty resWidth;
        //SerializedProperty resHeight;

        //private void OnEnable() {
        //    outputDisplayRect = serializedObject.FindProperty(nameof(outputDisplayRect));
        //    resWidth = outputDisplayRect.FindPropertyRelative("width");
        //    resHeight = outputDisplayRect.FindPropertyRelative("height");
        //}

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            //EditorGUI.BeginChangeCheck();
            //EditorGUILayout.LabelField("Display Settings (If edited on playmode, will remain saved!)");
            //EditorGUILayout.PropertyField(outputDisplayRect);
            //if (EditorGUI.EndChangeCheck()) {
            //    ValidateOnEdited();
            //    serializedObject.ApplyModifiedProperties();
            //}
        }

        //public void ValidateOnEdited() {

        //    if (IsResolutionAboveLimit(resWidth.floatValue))
        //        resWidth.floatValue = resolutionMax;
        //    if (IsResolutionAboveLimit(resHeight.floatValue))
        //        resHeight.floatValue = resolutionMax;
        //}
        //private bool IsResolutionAboveLimit(float resolution) {
        //    if (resolution > resolutionMax) {
        //        Debug.LogWarning("Resolution above limit! Setting to 8192");
        //        return true;
        //    }
        //    return false;
        //}
    }
#endif
}