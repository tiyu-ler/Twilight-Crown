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
    public GameObject SwordCase;
    public Vector2 EndPosition;
    protected BoxCollider2D collider2d;
    protected bool isCollected = false;
    protected PlayerMovement playerMovement;
    protected PlayerAttack playerAttack;
    protected PlayerData playerData;
    private bool _canBePressed = false;
    protected float moveSpeed;
    public GameObject Player;
    public CameraFollowObject cameraFollowObject;
    protected virtual void Start()
    {
        _lowerBodyAnimator = LowerBody.GetComponent<Animator>();
        _upperBodyAnimator = UpperBody.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerData = FindObjectOfType<PlayerData>();
        moveSpeed = playerMovement.MoveSpeed;
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
        if (_canBePressed)
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
        // cameraFollowObject.NeedToFollow = false;
        
        isCollected = true;
        collider2d.enabled = false;
        playerMovement.RigidBody.velocity = Vector2.zero;
        // playerMovement.enabled = false;
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
        // Player.transform.position = EndPosition;
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
        Debug.Log(Mathf.Abs(Player.transform.eulerAngles.y));

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

        // playerMovement.enabled = true;
        playerAttack.enabled = true;
        playerMovement.FlipCharacter(true, 1);
        // playerMovement.FlipCharacter(true, 1);
        playerMovement.CanMove = true;
        // cameraFollowObject.NeedToFollow = true;
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
