using System.Collections;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    [Header("Camera Setup")]
    public static CameraManager Instance;
    [SerializeField] private CinemachineVirtualCamera[] AllVirtualCameras;

    [Header("Y Lerp Settings")]
    [SerializeField] private float FallPanAmount = 0.25f;
    [SerializeField] private float FallPanTime = 0.35f;
    public float FallSpeedDampingLimit = -15f;
    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private float _normalYPanAmount;
    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransporter;
    private Vector2 _startingTrackedObjectOffset;
    private float SavedTrackedObjectOffsetX;
    private float SavedTrackedObjectOffsetY;
    // private CinemachineConfiner confiner;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < AllVirtualCameras.Length; i++)
        {
            if (AllVirtualCameras[i].enabled)
            {
                _currentCamera = AllVirtualCameras[i];
            
                _framingTransporter = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                // confiner = _currentCamera.GetComponent<CinemachineConfiner>();
            }
        }

        _normalYPanAmount = _framingTransporter.m_YDamping;

        _startingTrackedObjectOffset = _framingTransporter.m_TrackedObjectOffset;
    }

    public void LerpYDamping(bool isPlayerFalling)
    {
        StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYOffset(bool lerpToZero)
    {
        float elapsedTime = 0f;
        float offsetLerpTime = 0.5f;
        Vector3 startOffset, endOffset;
        if (lerpToZero)
        {
            startOffset = _framingTransporter.m_TrackedObjectOffset;
            endOffset = Vector3.zero;
            
        }
        else
        {
            startOffset = Vector3.zero;
            endOffset = new Vector3(SavedTrackedObjectOffsetX, SavedTrackedObjectOffsetY, 0);
        }
        while (elapsedTime < offsetLerpTime)
        {
            elapsedTime += Time.deltaTime;

            _framingTransporter.m_TrackedObjectOffset = Vector3.Lerp(startOffset, endOffset, elapsedTime/offsetLerpTime);

            yield return null;
        }
    }
    private IEnumerator LerpYAction(bool isPlayerFalling)
    { 
        IsLerpingYDamping = true;

        float startDampAmount = _framingTransporter.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = FallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normalYPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < FallPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime/FallPanTime);
            _framingTransporter.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }
    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos;

        if (!panToStartingPos)
        {
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }
        else
        {
            startingPos = _framingTransporter.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        float elapsedTime  = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, elapsedTime/panTime);
            _framingTransporter.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    public void ReturnCameraToDefault(CinemachineVirtualCamera defaultCamera)
    {
        _currentCamera.GetComponent<CinemachineVirtualCamera>().enabled = false;
        defaultCamera.enabled = true;
        _currentCamera = defaultCamera;
        // CinemachineBrain camBrain = _currentCamera.GetComponent<CinemachineBrain>();
        // camBrain.m_DefaultBlend.m_Time = 0f; // No blending, instant transition
        _framingTransporter = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight,
        GameObject LockPosLeft, GameObject LockPosRight,
        Vector2 triggerExitDirection)
    {
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            if (cameraFromLeft != cameraFromRight)
            {
                cameraFromRight.enabled = true;
                cameraFromLeft.enabled = false;
                _currentCamera = cameraFromRight;
                _framingTransporter = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
            if (LockPosRight != null)
            {
                ChangeLockedCameraTargetPosition(LockPosRight);
            }
        }
        else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            if (cameraFromLeft != cameraFromRight)
            {
                cameraFromLeft.enabled = true;
                cameraFromRight.enabled = false;
                _currentCamera = cameraFromLeft;
                _framingTransporter = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
            if (LockPosLeft != null)
            {
                ChangeLockedCameraTargetPosition(LockPosLeft);
            }
        }
    }

    public void ChangeLockedCameraTargetPosition(GameObject newLockPos)
    {
        _currentCamera.Follow = newLockPos.transform;
    }
}
