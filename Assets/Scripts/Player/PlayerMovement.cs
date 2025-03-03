using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    public float HorizontalInput;
    public bool CanMove = true;
    public float MoveSpeed = 6f;                   
    public float JumpForce = 14f;                 
    public float MaxJumpHoldTime = 0.2f;
    public float LowJumpMultiplier = 4f;
    public float FallMultiplier = 5f;

    [Header("Dash Settings")]
    public float DashSpeed = 20f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 0.6f;
    public bool IsDashing;

    [Header("Ground Settings")]
    public Transform GroundCheck;                 
    public float GroundCheckDistance = 0.2f;    
    public LayerMask GroundLayer;             
    public bool _isGrounded = true;    

    [Header("Animation Settings")]
    public Animator UpperBodyAnimator;
    public Animator LowerBodyAnimator;
    public Animator FullBodyAnimator;
    public GameObject UpperBody;
    public GameObject LowerBody;

    [Header("Camera Setup")]
    public bool IsFacingRight = true;
    public CameraFollowObject _cameraFollowObject;
    public CameraManager cameraManager;

    private bool _canDash = true;
    private float FallSpeedDampingChange;
    private float MaxFallSpeed = 30f;
    private Rigidbody2D _rb;
    private bool _isJumping, _jumpButtonHeld;
    private float _jumpTimeCounter;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // _spriteRenderer = GetComponent<SpriteRenderer>();
        FullBodyAnimator.StopPlayback();
        // _spriteRenderer.sprite =  null;
        FallSpeedDampingChange = cameraManager.FallSpeedDampingLimit;
    }


    private void Update()
    {
        if (IsDashing || !CanMove) return;

        UpperBodyAnimator.SetBool("IsGrounded", _isGrounded);
        LowerBodyAnimator.SetBool("IsGrounded", _isGrounded);

        UpperBodyAnimator.SetFloat("VerticalVelocity", _rb.velocity.y);
        LowerBodyAnimator.SetFloat("VerticalVelocity", _rb.velocity.y);

        HorizontalInput = Input.GetAxisRaw("Horizontal");

        if (_rb.velocity.y < FallSpeedDampingChange && !cameraManager.IsLerpingYDamping 
            && !cameraManager.LerpedFromPlayerFalling)
        {
            cameraManager.LerpYDamping(true);
        }

        if (_rb.velocity.y >= 0f && !cameraManager.IsLerpingYDamping && cameraManager.LerpedFromPlayerFalling)
        {
            cameraManager.LerpedFromPlayerFalling = false;

            cameraManager.LerpYDamping(false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            StartJump();
        }

        if (Input.GetKey(KeyCode.Space) && _isJumping)
        {
            _jumpButtonHeld = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _jumpButtonHeld = false;
            _isJumping = false;
        } 

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            StartCoroutine(Dash());
        }
    }


    private void FixedUpdate()
    {
        if (IsDashing || !CanMove) return;
        FlipCharacter(false, -1);

        _rb.velocity = new Vector2(HorizontalInput * MoveSpeed, _rb.velocity.y);

        UpperBodyAnimator.SetFloat("RunSpeed", Mathf.Abs(HorizontalInput));
        LowerBodyAnimator.SetFloat("RunSpeed", Mathf.Abs(HorizontalInput));

        _isGrounded = IsGrounded();

        ApplyJumpPhysics();
    }


    private void StartJump()
    {
        _isJumping = true;
        _jumpButtonHeld = true;
        _jumpTimeCounter = MaxJumpHoldTime;
        _rb.velocity = new Vector2(_rb.velocity.x, JumpForce);
    }


    private void ApplyJumpPhysics()
    {
        if (_isGrounded)
        {
            LowerBodyAnimator.SetBool("IsGrounded", _isGrounded);
            UpperBodyAnimator.SetBool("IsGrounded", _isGrounded);

            _rb.gravityScale = 1f;
            _isJumping = false;
            return;
        }

        if (_rb.velocity.y > 0)
        {
            if (_jumpButtonHeld && _jumpTimeCounter > 0)
            {
                _jumpTimeCounter -= Time.fixedDeltaTime;
                _rb.velocity = new Vector2(_rb.velocity.x, JumpForce);
            }
            else
            {
                _rb.gravityScale = LowJumpMultiplier; 
            }
        }
        else if (_rb.velocity.y < 0)
        {
            _rb.gravityScale = FallMultiplier;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -MaxFallSpeed));
        }
    }


    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(GroundCheck.position, Vector2.down, GroundCheckDistance, GroundLayer);
        Debug.DrawRay(GroundCheck.position, Vector2.down * GroundCheckDistance, Color.green);

        return hit.collider != null;
    }


    public void FlipCharacter(bool ForcedTurn, int direction)
    {
        if (HorizontalInput > 0 && !IsFacingRight || ForcedTurn && direction == 1)
        {
            UpperBody.transform.localPosition = new Vector3(0,0,-0.01f);
            IsFacingRight = !IsFacingRight;
            Vector3 rotation = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotation);

            _cameraFollowObject.CallTurn();
            // if (!ForcedTurn) _cameraFollowObject.CallTurn();
        } 
        else if (HorizontalInput < 0 && IsFacingRight || ForcedTurn && direction == 0)
        {
            UpperBody.transform.localPosition = new Vector3(0,0,0.01f);
            IsFacingRight = !IsFacingRight;
            Vector3 rotation = new Vector3(transform.rotation.x, 180, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotation);

            _cameraFollowObject.CallTurn();
            // if (!ForcedTurn) _cameraFollowObject.CallTurn();
        }
    }



    private IEnumerator Dash()
    {
        _canDash = false;
        IsDashing = true;
        UpperBody.SetActive(false);
        LowerBody.SetActive(false);
        _rb.gravityScale = 0f;

        float dashDirection = IsFacingRight ? 1 : -1;
        _rb.velocity = new Vector2(dashDirection * DashSpeed, 0);

        FullBodyAnimator.Play("Dash");

        yield return new WaitForSeconds(0.07f + DashDuration);

        FullBodyAnimator.Play("DashStop");

        yield return new WaitForSeconds(0.1f);
        FullBodyAnimator.Play("None");
        UpperBody.SetActive(true);
        LowerBody.SetActive(true);
        _rb.gravityScale = 1f;
        IsDashing = false;
        UpperBodyAnimator.SetBool("HasSword", true);
        yield return new WaitForSeconds(DashCooldown);
        _canDash = true;
    }

    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = FullBodyAnimator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
                return clip.length;
        }
        return 0.1f;
    }

    private void OnDrawGizmos()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(GroundCheck.position, GroundCheck.position + Vector3.down * GroundCheckDistance);
        }
    }
}
