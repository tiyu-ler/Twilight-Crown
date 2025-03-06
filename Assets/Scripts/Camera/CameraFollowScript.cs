// using UnityEngine;

// public class CameraFollow : MonoBehaviour
// {

//     [Header("Follow Settings")]
//     public Transform Player;  
//     public float FollowSpeed;  
//     public float VerticalOffset;
//     public float LookAheadDistance;  
//     public float LookAheadSmoothing;  

//     private Vector3 _velocity = Vector3.zero;
//     private float _currentLookAhead = 0f;
//     private float _lastLookAhead = 0f;

//     private void LateUpdate()
//     {
//         if (Player == null) return;

//         float moveDirection = Input.GetAxisRaw("Horizontal");
        
//         if (moveDirection != 0)
//         {
//             _lastLookAhead = moveDirection * LookAheadDistance;
//         }
        
//         _currentLookAhead = Mathf.Lerp(_currentLookAhead, _lastLookAhead, LookAheadSmoothing * Time.deltaTime);

//         Vector3 targetPositionX = new Vector3(Player.position.x + _currentLookAhead, transform.position.y, transform.position.z);

//         transform.position = Vector3.SmoothDamp(transform.position, targetPositionX, ref _velocity, 1f / FollowSpeed); //horizontal
//         transform.position = new Vector3(transform.position.x,Player.position.y + VerticalOffset, transform.position.z); //vertical
//     }
// }
