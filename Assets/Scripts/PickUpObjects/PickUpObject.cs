using System.Collections;
using UnityEngine;

public abstract class PickUpObject : MonoBehaviour
{
    public string pickupAnimationName = "";
    protected Animator animator;
    protected Animator _lowerBodyAnimator;
    protected Animator _upperBodyAnimator;
    public GameObject LowerBody;
    public GameObject UpperBody;
    public Vector2 EndPosition;
    protected BoxCollider2D collider2d;
    protected bool isCollected = false;
    protected PlayerMovement playerMovement;
    protected PlayerAttack playerAttack;
    // public PlayerDataSave playerDataSave;
    private bool _canBePressed = false;
    protected float moveSpeed;
    public GameObject Player;
    // public GameObject BlackBorderTop;
    // public GameObject BlackBorderBottom;
    protected CameraFollowObject cameraFollowObject;
    public CameraManager cameraManager;
    protected bool _isDead;
    public GameManager gameManager;
    public MarkerTextPopUp markerTextPopUp;
    protected virtual void Start()
    {
        // gameManager = FindObjectOfType<GameManager>();
        cameraFollowObject = FindAnyObjectByType<CameraFollowObject>();
        _lowerBodyAnimator = LowerBody.GetComponent<Animator>();
        _upperBodyAnimator = UpperBody.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        moveSpeed = playerMovement.MoveSpeed;
        _isDead = Player.GetComponent<PlayerHealth>().isDead;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !isCollected)
        {
            _canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            _canBePressed = false;
        }
    }

    void Update()
    {
        _isDead = Player.GetComponent<PlayerHealth>().isDead;
        if (_canBePressed && !_isDead)
        {
            if (playerMovement._isGrounded && !isCollected)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    playerMovement.CanMove = false;
                    playerMovement.HorizontalInput = 0;
                    if (Player.transform.position.x - EndPosition.x < 0 && Player.transform.rotation. y != 0)
                    {
                        playerMovement.FlipCharacter(true, 1);
                    }
                    else if (Player.transform.position.x - EndPosition.x > 0 && Player.transform.rotation. y == 0)
                    {
                        playerMovement.FlipCharacter(true, 0);
                    }
                    StartCoroutine(MoveToPickUp());
                }
            }
        }
    }

    protected IEnumerator MoveToPickUp()
    {   
        // cameraManager.CutsceneCamera(true);
        // BlackBorderTop.SetActive(true);
        // BlackBorderBottom.SetActive(true);
        // float elapsedTime = 0f;
        // float LerpTime = 0.5f;

        
        // while (elapsedTime < LerpTime)
        // {
        //     elapsedTime += Time.deltaTime;

        //     BlackBorderTop.transform.localPosition = new Vector3(0, Mathf.Lerp(640, 440, elapsedTime/LerpTime), 0);
        //     BlackBorderBottom.transform.localPosition = new Vector3(0, Mathf.Lerp(-640, -440, elapsedTime/LerpTime), 0);
            
        //     yield return null;
        // }
        isCollected = true;
        collider2d.enabled = false;
        playerMovement.RigidBody.velocity = Vector2.zero;
        playerAttack.enabled = false;

        _lowerBodyAnimator.SetFloat("RunSpeed", 1);
        _upperBodyAnimator.SetFloat("RunSpeed", 1);
        
        Vector2 startPos = Player.transform.position;

        while (Vector2.Distance(Player.transform.position, EndPosition) > 0.1f)
        {
                Player.transform.position = Vector2.MoveTowards(
                Player.transform.position, 
                EndPosition, 
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        Collect();
    }

    protected virtual void Collect()
    {
        if (pickupAnimationName != "")
        {
            animator.Play(pickupAnimationName);
            LowerBody.SetActive(false);
            UpperBody.SetActive(false);
        }

        StartCoroutine(DestroyAfterAnimation());
        CollectItem();
    }

    protected virtual IEnumerator DestroyAfterAnimation()
    {
        if (pickupAnimationName != "")
        {
            yield return new WaitForSeconds(GetAnimationLength());

            LowerBody.SetActive(true);
            UpperBody.SetActive(true);

            UpperBody.GetComponent<Animator>().Play("U_Idle");
            LowerBody.GetComponent<Animator>().Play("L_Idle");
        }

        playerAttack.enabled = true;
        Debug.Log("Player rotation " + Player.transform.rotation.y);
        playerMovement.FlipCharacter(true, 1);
        playerMovement.CanMove = true;
        Debug.Log(cameraFollowObject.transform.rotation.y != Player.transform.rotation.y);
        // cameraManager.CutsceneCamera(false);
        // BlackBorderTop.SetActive(false);
        // BlackBorderBottom.SetActive(false);
        // float elapsedTime = 0f;
        // float LerpTime = 0.5f;
        // while (elapsedTime < LerpTime)
        // {
        //     elapsedTime += Time.deltaTime;

        //     BlackBorderTop.transform.localPosition = new Vector3(0, Mathf.Lerp(440, 640, elapsedTime/LerpTime), 0);
        //     BlackBorderBottom.transform.localPosition = new Vector3(0, Mathf.Lerp(-440, -640, elapsedTime/LerpTime), 0);
            
        //     yield return null;
        // }
        if (cameraFollowObject.transform.rotation.y != Player.transform.rotation.y)
        {
            Debug.Log("CALLTURN");
            cameraFollowObject.CallTurn();
        }
        Destroy(gameObject);
    }

    protected float GetAnimationLength()
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == pickupAnimationName)
                return clip.length;
        }
        return 0.5f;
    }

    protected abstract void CollectItem();
}
