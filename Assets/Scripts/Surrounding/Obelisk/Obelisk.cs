using UnityEngine;

public class Obelisk : MonoBehaviour
{
    public int obeliskID;
    public LayerMask playerLayer;

    private Animator obeliskAnimator;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private PlayerData playerData;
    private bool _isActive = false;
    private static Obelisk currentActiveObelisk;
    private Vector3 SpawnPoint;
    private void Start()
    {
        SpawnPoint = new Vector3(transform.position.x - 1.2f, transform.position.y + 1.2f, 0);
        obeliskAnimator = GetComponent<Animator>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerData = FindObjectOfType<PlayerData>();
        playerHealth = FindObjectOfType<PlayerHealth>();
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
            currentActiveObelisk.DeactivateObelisk();
            currentActiveObelisk._isActive = false;
        }

        currentActiveObelisk = this;
        if (!_isActive)
        {
            _isActive = true;
            obeliskAnimator.Play("Active");
        }

        playerHealth.SetLastObelisk(this);

        SaveSystem.SaveGame(
            obeliskID,
            playerData.HasSword,
            playerData.SwordLevel,
            playerData.HasWallClimb,
            playerData.HasDash,
            playerData.HasMagic,
            playerData.MagicLevel,
            playerData.Money,
            playerData.HasWallClimb, 
            playerData.HasDash
        );
    }

    public Vector3 GetSpawnPoint()
    {
        return SpawnPoint;
    }

    private void DeactivateObelisk()
    {
        obeliskAnimator.Play("InActive");
    }
}
