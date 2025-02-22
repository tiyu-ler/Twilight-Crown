using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;  
    public float FollowSpeed = 10f;  
    public float LookAheadDistance = 2f;  
    public float VerticalOffset = 3f;  
    public float JumpLookUpOffset = 1f;  
    public float FallLookDownOffset = 1.5f;  
    public float LookAheadSmoothing = 2f;  

    public float JumpThresholdHeight = 1.5f;  
    public float FallLimit = 2f;  // 🔥 Минимальная высота, ниже которой камера не опускается
    // private float _jumpStartY;
    // private bool _isJumping = false;

    private Vector3 _velocity = Vector3.zero;
    private float _currentLookAhead = 0f;
    private float _lastLookAhead = 0f;  // 🔥 Запоминаем последний взгляд вперёд
    private void LateUpdate()
    {
        if (Player == null) return;

        float moveDirection = Input.GetAxisRaw("Horizontal");
        float playerVelocityY = Player.GetComponent<Rigidbody2D>().velocity.y;
        float playerY = Player.position.y;

        // 🔥 Сохранение последнего взгляда вперёд
        if (moveDirection != 0)
        {
            _lastLookAhead = moveDirection * LookAheadDistance;
        }
        _currentLookAhead = Mathf.Lerp(_currentLookAhead, _lastLookAhead, LookAheadSmoothing * Time.deltaTime);

        // 🔥 Отложенный подъём камеры (ждёт, пока игрок подойдёт близко)
        bool _enableFall = false;

        // if (playerVelocityY > 0.1f && !_isJumping)
        // {
        //     _isJumping = true;
        //     _jumpStartY = playerY;
        // }
        // else if (_isJumping && playerVelocityY <= 0.1f) 
        // {
        //     _isJumping = false;
        // }

        float verticalAdjustment = VerticalOffset;

        // if (_isJumping)
        // {
            // 🔥 Ждём, пока игрок приблизится к верхнему краю камеры
            // if (playerY > transform.position.y - 0.8f)  
                verticalAdjustment += JumpLookUpOffset;
                // _enableFall = true;
        // }
        if (playerVelocityY < -0.1f && _enableFall || playerY < transform.position.y + 0.5f) 
        {
            // Debug.Log("DOWN");
            // 🔥 Ограничиваем падение камеры (она не уходит слишком низко)
            verticalAdjustment -= FallLookDownOffset;
            verticalAdjustment = Mathf.Max(verticalAdjustment, Player.position.y - FallLimit);
        }

        Vector3 targetPosition = new Vector3(Player.position.x + _currentLookAhead, verticalAdjustment, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 1f / FollowSpeed);
    }
}
