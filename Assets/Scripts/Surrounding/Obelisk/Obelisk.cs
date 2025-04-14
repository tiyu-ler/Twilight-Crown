using UnityEngine;

public class Obelisk : MonoBehaviour
{
    public int obeliskID;
    public LayerMask playerLayer;
    public GameObject BlueLight;
    public GameObject GreenLight;
    private Animator obeliskAnimator;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private CameraFollowObject cameraFollowObject;
    // private PlayerData playerData;
    private bool _isActive = false;
    private static Obelisk currentActiveObelisk;
    private Vector3 SpawnPoint;
    // public PlayerDataSave playerDataSave;
    private GameManager gameManager;
    public MarkerTextPopUp markerTextPopUp;
    private void Start()
    {
        SpawnPoint = new Vector3(transform.position.x - 1.2f, transform.position.y - 1.15f, 0);
        obeliskAnimator = GetComponent<Animator>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        cameraFollowObject = FindObjectOfType<CameraFollowObject>();
        // playerData = FindObjectOfType<PlayerData>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        gameManager = FindObjectOfType<GameManager>();
        if (PlayerDataSave.Instance.ObeliskID == obeliskID)
        {
            currentActiveObelisk = this;
        }
        TeleportPlayerAndCamera(); //spawn player on game load
    }

    private void TeleportPlayerAndCamera()
    {
        if (obeliskID == PlayerDataSave.Instance.ObeliskID)
        {
            BlueLight.SetActive(false);
            GreenLight.SetActive(true);
            playerHealth.gameObject.transform.position = SpawnPoint;
            cameraFollowObject.transform.position = SpawnPoint;
            _isActive = true;
            obeliskAnimator.Play("Active");
            currentActiveObelisk = this;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            if (playerMovement._isGrounded && Input.GetKey(KeyCode.W) && !_isActive)
            {
                InvokeObelisk();
            }
        }
    }
    private void InvokeObelisk()
    {
        if (currentActiveObelisk != null && currentActiveObelisk != this)
        {
            currentActiveObelisk.markerTextPopUp.ActivateMarkUp();
            currentActiveObelisk.DeactivateObelisk();
            currentActiveObelisk._isActive = false;
        }
        SoundManager.Instance.PlaySound(SoundManager.SoundID.Invoke, worldPos: transform.position, volumeUpdate: 0.7f);
        currentActiveObelisk = this;
        markerTextPopUp.DisableMarkUp();
        if (!_isActive)
        {
            BlueLight.SetActive(false);
            GreenLight.SetActive(true);
            _isActive = true;
            obeliskAnimator.Play("Active");
        }

        playerHealth.SetLastObelisk(this);
        PlayerDataSave.Instance.ObeliskID = obeliskID;
        gameManager.SaveGame(PlayerDataSave.Instance.saveID);
    }

    public Vector3 GetSpawnPoint()
    {
        return SpawnPoint;
    }

    private void DeactivateObelisk()
    {
        BlueLight.SetActive(true);
        GreenLight.SetActive(false);
        obeliskAnimator.Play("InActive");
    }
}
