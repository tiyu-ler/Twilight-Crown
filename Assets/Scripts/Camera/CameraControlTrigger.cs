using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;

    private Collider2D _collider;
    private PlayerMovement playerMovement;
    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>(); 
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && customInspectorObjects.panCameraOnContact)
        {
            CameraManager.Instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime,
                customInspectorObjects.panDirection, false);
        }
    }
    private IEnumerator WaitForLanding(Vector2 exitDirection)
    {
        yield return new WaitUntil(() => playerMovement._isGrounded);
        Debug.Log("Swap");
        CameraManager.Instance.SwapCamera(
            customInspectorObjects.cameraOnLeft,
            customInspectorObjects.cameraOnRight,
            exitDirection
        );
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - _collider.bounds.center).normalized;

            if (customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                if (customInspectorObjects.IsCameraFromLeftTight || customInspectorObjects.IsCameraFromRightTight)
                {
                    StartCoroutine(WaitForLanding(exitDirection));
                }
                else
                {
                    CameraManager.Instance.SwapCamera(
                        customInspectorObjects.cameraOnLeft,
                        customInspectorObjects.cameraOnRight,
                        exitDirection
                    );
                }
            }

            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.Instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime,
                    customInspectorObjects.panDirection, true);
            }
        }
    }
}


[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;  
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
    [HideInInspector] public bool IsCameraFromLeftTight = false;
    [HideInInspector]public bool IsCameraFromRightTight = false;
}

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}


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

            cameraControlTrigger.customInspectorObjects.IsCameraFromLeftTight = EditorGUILayout.Toggle("Is Left Camera Tight?", 
                cameraControlTrigger.customInspectorObjects.IsCameraFromLeftTight);
            cameraControlTrigger.customInspectorObjects.IsCameraFromRightTight = EditorGUILayout.Toggle("Is Right Camera Tight?", 
                cameraControlTrigger.customInspectorObjects.IsCameraFromRightTight);
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

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}

