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
    public float FallLimit = 2f;
    private Vector3 _velocity = Vector3.zero;
    private float _currentLookAhead = 0f;
    private float _lastLookAhead = 0f;
    private void LateUpdate()
    {
        if (Player == null) return;

        float moveDirection = Input.GetAxisRaw("Horizontal");
        // float playerVelocityY = Player.GetComponent<Rigidbody2D>().velocity.y;
        // float playerY = Player.position.y;

        // 🔥 Сохранение последнего взгляда вперёд
        if (moveDirection != 0)
        {
            _lastLookAhead = moveDirection * LookAheadDistance;
        }
        _currentLookAhead = Mathf.Lerp(_currentLookAhead, _lastLookAhead, LookAheadSmoothing * Time.deltaTime);

        Vector3 targetPositionX = new Vector3(Player.position.x + _currentLookAhead, transform.position.y, transform.position.z);
        // Vector3 targetPositionY = new Vector3(transform.position.x, Player.position.y + VerticalOffset, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPositionX, ref _velocity, 1f / FollowSpeed); //horizontal
        transform.position = new Vector3(transform.position.x,Player.position.y + VerticalOffset, transform.position.z); //vertical
    }
}
