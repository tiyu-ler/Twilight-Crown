using System;
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
    public Rigidbody2D RigidBody;

    [Header("Dash Settings")]
    public float DashSpeed = 20f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 0.6f;
    public bool IsDashing;
    public bool CanDash = false;

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
    [Header("Wall Jump Settings")]
    public Transform WallCheck;
    public float WallCheckDistance = 0.2f;
    public LayerMask WallLayer;
    public bool CanWallClimb = false;
    public bool StopSound;
    private bool _isTouchingWall;
    private float FallSpeedDampingChange;
    private float MaxFallSpeed = 30f;
    private bool _isJumping, _jumpButtonHeld;
    private float _jumpTimeCounter;
    private bool _wasGroundedLastFrame = true;
    public AudioSource audioSource;
    private void Start()
    {
        StopSound = false;
        audioSource = GetComponent<AudioSource>();
        CanDash = PlayerDataSave.Instance.HasDash;
        // CanWallClimb = PlayerDataSave.Instance.HasWallClimb;
        CanWallClimb = true;
        RigidBody = GetComponent<Rigidbody2D>();
        FullBodyAnimator.StopPlayback();
        FallSpeedDampingChange = cameraManager.FallSpeedDampingLimit;
        UpperBodyAnimator.SetBool("HasSword", PlayerDataSave.Instance.HasSword);
    }


    private void Update()
    {
        if (IsDashing || !CanMove) return;

        UpperBodyAnimator.SetBool("IsGrounded", _isGrounded);
        LowerBodyAnimator.SetBool("IsGrounded", _isGrounded);

        UpperBodyAnimator.SetFloat("VerticalVelocity", RigidBody.velocity.y);
        LowerBodyAnimator.SetFloat("VerticalVelocity", RigidBody.velocity.y);

        HorizontalInput = Input.GetAxisRaw("Horizontal");

        _isTouchingWall = Physics2D.Raycast(WallCheck.position, Vector2.right * (IsFacingRight ? 1 : -1), WallCheckDistance, WallLayer);

        if (RigidBody.velocity.y < FallSpeedDampingChange && !cameraManager.IsLerpingYDamping 
            && !cameraManager.LerpedFromPlayerFalling)
        {
            cameraManager.LerpYDamping(true);
        }

        if (RigidBody.velocity.y >= 0f && !cameraManager.IsLerpingYDamping && cameraManager.LerpedFromPlayerFalling)
        {
            cameraManager.LerpedFromPlayerFalling = false;

            cameraManager.LerpYDamping(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && CanDash)
        {
            StartCoroutine(Dash());
        }
    }


    private void FixedUpdate()
    {
        if (IsDashing || !CanMove) return;
        FlipCharacter(false, -1);

        if (!_isTouchingWall)
        {
            RigidBody.velocity = new Vector2(HorizontalInput * MoveSpeed, RigidBody.velocity.y);
            if (Math.Abs(HorizontalInput) > 0.1f && !audioSource.isPlaying && IsGrounded() && !StopSound && !_isTouchingWall)
            {
                audioSource.Play();
            }
            else if (Math.Abs(HorizontalInput) <= 0.1f && audioSource.isPlaying || !IsGrounded() || StopSound)
            {
                audioSource.Pause();
            }
        }
        UpperBodyAnimator.SetFloat("RunSpeed", Mathf.Abs(HorizontalInput));
        LowerBodyAnimator.SetFloat("RunSpeed", Mathf.Abs(HorizontalInput));

        _isGrounded = IsGrounded();

        ApplyJumpPhysics();
    }


    private void StartJump()
    {
        // if (_isTouchingWall && !_isGrounded && CanWallClimb)
        // {
        //     float pushDirection = IsFacingRight ? 1 : -1;
        //     RigidBody.velocity = new Vector2(140 * (IsFacingRight ? -1 : 1), JumpForce*1.75f);

        //     SoundManager.Instance.PlaySound(SoundManager.SoundID.HeroJump, volumeUpdate: 0.003f);
        // }
        if (_isTouchingWall && !_isGrounded && CanWallClimb)
        {
            float horizontalJumpForce = 10f;
            float verticalJumpForce = JumpForce * 1.3f; 

            RigidBody.velocity = Vector2.zero;

            float direction = IsFacingRight ? -1f : 1f;

            Vector2 wallJumpVector = new Vector2(direction * horizontalJumpForce, verticalJumpForce);
            RigidBody.AddForce(wallJumpVector, ForceMode2D.Impulse);

            StartCoroutine(DisableMovementTemporarily(0.15f)); 

            SoundManager.Instance.PlaySound(SoundManager.SoundID.HeroJump, volumeUpdate: 0.01f); // Slightly louder for clarity
        }

        else if (_isGrounded)
        {
            _isJumping = true;
            _jumpButtonHeld = true;
            _jumpTimeCounter = MaxJumpHoldTime;
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, JumpForce);
            SoundManager.Instance.PlaySound(SoundManager.SoundID.HeroJump, volumeUpdate: 0.01f);
        }
    }
    private IEnumerator DisableMovementTemporarily(float duration)
    {
        RigidBody.gravityScale = 0.1f;
        CanMove = false;
        yield return new WaitForSeconds(duration);
        CanMove = true;
        RigidBody.gravityScale = 1f;
    }



    private void ApplyJumpPhysics()
    {
        if (_isGrounded)
        {
            if (!_wasGroundedLastFrame) SoundManager.Instance.PlaySound(SoundManager.SoundID.HeroLand, volumeUpdate: 0.16f);

            LowerBodyAnimator.SetBool("IsGrounded", _isGrounded);
            UpperBodyAnimator.SetBool("IsGrounded", _isGrounded);

            RigidBody.gravityScale = 1f;
            _isJumping = false;
            _wasGroundedLastFrame = true;
            return;
        }
        if (RigidBody.velocity.y > 0)
        {
            if (_jumpButtonHeld && _jumpTimeCounter > 0)
            {
                _jumpTimeCounter -= Time.fixedDeltaTime;
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, JumpForce);
            }
            else
            {
                RigidBody.gravityScale = LowJumpMultiplier; 
            }
        }
        else if (RigidBody.velocity.y < 0)
        {
            RigidBody.gravityScale = FallMultiplier;
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, Mathf.Max(RigidBody.velocity.y, -MaxFallSpeed));
        }
        _wasGroundedLastFrame = false;
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
            if (!ForcedTurn)
            {
                IsFacingRight = !IsFacingRight;
            }
            Vector3 rotation = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotation);

            if (!ForcedTurn) _cameraFollowObject.CallTurn();
        } 
        else if (HorizontalInput < 0 && IsFacingRight || ForcedTurn && direction == 0)
        {
            UpperBody.transform.localPosition = new Vector3(0,0,0.01f);
            if (!ForcedTurn)
            {
                IsFacingRight = !IsFacingRight;
            }
            Vector3 rotation = new Vector3(transform.rotation.x, 180, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotation);

            if (!ForcedTurn) _cameraFollowObject.CallTurn();
        }
    }



    private IEnumerator Dash()
    {
        CanDash = false;
        IsDashing = true;
        UpperBody.SetActive(false);
        LowerBody.SetActive(false);
        RigidBody.gravityScale = 0f;

        float dashDirection = IsFacingRight ? 1 : -1;
        RigidBody.velocity = new Vector2(dashDirection * DashSpeed, 0);

        FullBodyAnimator.Play("Dash");
        SoundManager.Instance.PlaySound(SoundManager.SoundID.Dash, worldPos: transform.position, volumeUpdate: 0.4f);
        yield return new WaitForSeconds(0.07f + DashDuration);

        FullBodyAnimator.Play("DashStop");

        yield return new WaitForSeconds(0.1f);
        FullBodyAnimator.Play("None");
        UpperBody.SetActive(true);
        LowerBody.SetActive(true);
        RigidBody.gravityScale = 1f;
        IsDashing = false;
        UpperBodyAnimator.SetBool("HasSword", true);
        yield return new WaitForSeconds(DashCooldown);
        CanDash = true;
    }

    // private float GetAnimationLength(string animationName)
    // {
    //     RuntimeAnimatorController ac = FullBodyAnimator.runtimeAnimatorController;
    //     foreach (AnimationClip clip in ac.animationClips)
    //     {
    //         if (clip.name == animationName)
    //             return clip.length;
    //     }
    //     return 0.1f;
    // }

    private void OnDrawGizmos()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(GroundCheck.position, GroundCheck.position + Vector3.down * GroundCheckDistance);
        }
    }
}
