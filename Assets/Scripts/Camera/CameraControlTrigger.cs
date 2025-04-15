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
                            SoundManager.Instance.PlaySound(SoundManager.SoundID.SecretZoneAppearence, worldPos: new Vector2(125.28f,-12), soundType: 2, volumeUpdate: 0.4f, spatialBlend: 0);
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


