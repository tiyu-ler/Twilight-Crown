using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    public float MoveSpeed = 6f;                   
    public float JumpForce = 14f;                 
    public float MaxJumpHoldTime = 0.2f;
    public float LowJumpMultiplier = 4f;
    public float FallMultiplier = 5f;

    [Header("Ground Settings")]
    public Transform GroundCheck;                 
    public float GroundCheckDistance = 0.2f;    
    public LayerMask GroundLayer;                 

    [Header("Animation Settings")]
    public Animator UpperBodyAnimator;
    public Animator LowerBodyAnimator;
    public GameObject UpperBody;

    [Header("Camera Setup")]
    public bool IsFacingRight = true;
    public CameraFollowObject _cameraFollowObject;

    private float MaxFallSpeed = 40f;
    private Rigidbody2D _rb;
    private bool _isGrounded = true, _isJumping, _jumpButtonHeld;
    private float _horizontalInput, _jumpTimeCounter;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        UpperBodyAnimator.SetBool("IsGrounded", _isGrounded);
        LowerBodyAnimator.SetBool("IsGrounded", _isGrounded);

        UpperBodyAnimator.SetFloat("VerticalVelocity", _rb.velocity.y);
        LowerBodyAnimator.SetFloat("VerticalVelocity", _rb.velocity.y);

        _horizontalInput = Input.GetAxisRaw("Horizontal");

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
    }


    private void FixedUpdate()
    {
        FlipCharacter();

        _rb.velocity = new Vector2(_horizontalInput * MoveSpeed, _rb.velocity.y);

        UpperBodyAnimator.SetFloat("RunSpeed", Mathf.Abs(_horizontalInput));
        LowerBodyAnimator.SetFloat("RunSpeed", Mathf.Abs(_horizontalInput));

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


    private void FlipCharacter()
    {
        if (_horizontalInput > 0 && !IsFacingRight)
        {
            UpperBody.transform.localPosition = new Vector3(0,0,-0.01f);
            IsFacingRight = !IsFacingRight;
            Vector3 rotation = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotation);

            _cameraFollowObject.CallTurn();
        } 
        else if (_horizontalInput < 0 && IsFacingRight)
        {
            UpperBody.transform.localPosition = new Vector3(0,0,0.01f);
            IsFacingRight = !IsFacingRight;
            Vector3 rotation = new Vector3(transform.rotation.x, 180, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotation);

            _cameraFollowObject.CallTurn();
        }
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
