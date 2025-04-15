using UnityEditor;
using UnityEngine;
using Cinemachine;

[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControlTrigger.customInspectorObjects.swapCameras)
        {
            cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left",
                cameraControlTrigger.customInspectorObjects.cameraOnLeft, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right",
                cameraControlTrigger.customInspectorObjects.cameraOnRight, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.customInspectorObjects.IsCameraFromLeftTight = EditorGUILayout.Toggle("Camera on Left is Tight",
                cameraControlTrigger.customInspectorObjects.IsCameraFromLeftTight);
            cameraControlTrigger.customInspectorObjects.IsCameraFromRightTight = EditorGUILayout.Toggle("Camera on Right is Tight",
                cameraControlTrigger.customInspectorObjects.IsCameraFromRightTight);

            cameraControlTrigger.customInspectorObjects.IsCameraFromLeftSecret = EditorGUILayout.Toggle("Reveal Secret on Left",
                cameraControlTrigger.customInspectorObjects.IsCameraFromLeftSecret);
            cameraControlTrigger.customInspectorObjects.IsCameraFromRightSecret = EditorGUILayout.Toggle("Reveal Secret on Right",
                cameraControlTrigger.customInspectorObjects.IsCameraFromRightSecret);

            cameraControlTrigger.customInspectorObjects.ChangeLockPosition = EditorGUILayout.Toggle("Change Lock Position",
                cameraControlTrigger.customInspectorObjects.ChangeLockPosition);
        }

        if (cameraControlTrigger.customInspectorObjects.IsCameraFromLeftSecret || cameraControlTrigger.customInspectorObjects.IsCameraFromRightSecret)
        {
            cameraControlTrigger.customInspectorObjects.HiddenObject = EditorGUILayout.ObjectField("What to hide on Secret Found",
                cameraControlTrigger.customInspectorObjects.HiddenObject, typeof(GameObject), true) as GameObject;
        }

        if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                cameraControlTrigger.customInspectorObjects.panDirection);
            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance",
                cameraControlTrigger.customInspectorObjects.panDistance);
            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time",
                cameraControlTrigger.customInspectorObjects.panTime);
        }

        if (cameraControlTrigger.customInspectorObjects.ChangeLockPosition)
        {
            cameraControlTrigger.customInspectorObjects.LockPosLeft = EditorGUILayout.ObjectField("Left Locked Position",
                cameraControlTrigger.customInspectorObjects.LockPosLeft, typeof(GameObject), true) as GameObject;
            cameraControlTrigger.customInspectorObjects.LockPosRight = EditorGUILayout.ObjectField("Right Locked Position",
                cameraControlTrigger.customInspectorObjects.LockPosRight, typeof(GameObject), true) as GameObject;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}
