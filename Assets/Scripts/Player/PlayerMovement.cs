using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MoveSpeed = 6f;                   
    public float JumpForce = 14f;  // Hollow Knight has lower but tighter jumps                  
    public float MaxJumpHoldTime = 0.2f;  // Max time for variable jump
    public float LowJumpMultiplier = 4f;  // Extra gravity when releasing jump early
    public float FallMultiplier = 5f;  // Extra gravity when falling
    
    [Header("Ground Check")]
    public Transform GroundCheck;                 
    public float GroundCheckDistance = 0.2f;  // Slightly lower for precise detection     
    public LayerMask GroundLayer;                 

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _isJumping;
    private float _horizontalInput;
    private float _jumpTimeCounter;
    private bool _jumpButtonHeld;
    
    public Animator _playerAnimator;
    private bool _isFacingRight = true;  

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        // Time.timeScale = 0.1f;
    }

    private void Update()
    {
        _playerAnimator.SetBool("IsGrounded", _isGrounded);

        _playerAnimator.SetFloat("VerticalVelocity", _rb.velocity.y);

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
            _isJumping = false;  // Stop adding force when released
        } 
        FlipCharacter();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_horizontalInput * MoveSpeed, _rb.velocity.y);
        _playerAnimator.SetFloat("RunSpeed", Mathf.Abs(_horizontalInput));

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
        if (_isGrounded) // Reset gravity and stop bounce when landing
        {
            _playerAnimator.SetBool("IsGrounded", _isGrounded);
            _rb.gravityScale = 1f;
            _isJumping = false; // Ensure jumping state is reset
            return;
        }

        if (_rb.velocity.y > 0) // Ascending
        {
            if (_jumpButtonHeld && _jumpTimeCounter > 0)
            {
                _jumpTimeCounter -= Time.fixedDeltaTime; // Allow holding for higher jumps
                _rb.velocity = new Vector2(_rb.velocity.x, JumpForce);
            }
            else
            {
                _rb.gravityScale = LowJumpMultiplier; // Stronger gravity if jump is released early
            }
        }
        else if (_rb.velocity.y < 0) // Falling
        {
            _rb.gravityScale = FallMultiplier; // Increase fall speed
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(GroundCheck.position, Vector2.down, GroundCheckDistance, GroundLayer);
        Debug.DrawRay(GroundCheck.position, Vector2.down * GroundCheckDistance, Color.green);
        return hit.collider != null;
    }

    private void FlipCharacter()
    {
        if ((_horizontalInput > 0 && !_isFacingRight) || (_horizontalInput < 0 && _isFacingRight))
        {
            _isFacingRight = !_isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
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
