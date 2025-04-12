using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;
    private GameManager _gameManager;
    private Collider2D _collider;
    private PlayerMovement playerMovement;
    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>(); 
        _collider = GetComponent<Collider2D>();
        _gameManager = FindAnyObjectByType<GameManager>();
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
        CameraManager.Instance.SwapCamera(
            customInspectorObjects.cameraOnLeft,
            customInspectorObjects.cameraOnRight,
            null, null,
            exitDirection
        );
    }
    private IEnumerator SetSpriteOpacity(GameObject parent, float targetAlpha)
    {
        float duration = 0.8f;
        float time = 0f;

        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();
        float[] initialAlphas = new float[spriteRenderers.Length];
        
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            initialAlphas[i] = spriteRenderers[i].color.a;
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null) continue;
                Color color = spriteRenderers[i].color;
                color.a = Mathf.Lerp(initialAlphas[i], targetAlpha, t);
                spriteRenderers[i].color = color;
            }
            yield return null;
        }
        // PlayerDataSave.Instance.secretZoneOpened = true;
        // _gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        Destroy(parent);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - _collider.bounds.center).normalized;
            if (customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                if (customInspectorObjects.IsCameraFromLeftSecret || customInspectorObjects.IsCameraFromRightSecret)
                {
                    if (customInspectorObjects.HiddenObject != null)
                    {
                        if (!PlayerDataSave.Instance.secretZoneOpened)
                        {
                            SoundManager.Instance.PlaySound(SoundManager.SoundID.SecretZoneAppearence, worldPos: new Vector2(125.28f,-12), soundType: 2);
                            PlayerDataSave.Instance.secretZoneOpened = true;
                            _gameManager.SaveGame(PlayerDataSave.Instance.saveID);
                            StartCoroutine(SetSpriteOpacity(customInspectorObjects.HiddenObject, 0));
                        }
                    }
                }
                if (customInspectorObjects.IsCameraFromLeftTight || customInspectorObjects.IsCameraFromRightTight)
                {
                    StartCoroutine(WaitForLanding(exitDirection));
                }
                else
                {
                    CameraManager.Instance.SwapCamera(
                        customInspectorObjects.cameraOnLeft,
                        customInspectorObjects.cameraOnRight,
                        customInspectorObjects.LockPosLeft,
                        customInspectorObjects.LockPosRight,
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
    [HideInInspector] public bool IsCameraFromRightTight = false;
    [HideInInspector] public bool IsCameraFromLeftSecret = false;
    [HideInInspector] public bool IsCameraFromRightSecret = false;
    [HideInInspector] public bool ChangeLockPosition = false;
    [HideInInspector] public GameObject LockPosLeft;
    [HideInInspector] public GameObject LockPosRight;
    [HideInInspector] public GameObject HiddenObject;
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

