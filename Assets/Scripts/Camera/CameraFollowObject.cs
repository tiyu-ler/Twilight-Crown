using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("Follow Object Settings")]
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private float FlipYRotationTime = 0.5f;
     
    private Coroutine _turnCoroutine;
    private GameObject _player;
    private bool _isFacingRight;

    void Awake()
    {
        _player = PlayerTransform.gameObject;
        _isFacingRight = _player.GetComponent<PlayerMovement>().IsFacingRight;
    }


    void Update()
    {
        transform.position = PlayerTransform.position;
    }


    public void CallTurn()
    {
        if (_turnCoroutine != null) StopCoroutine(_turnCoroutine);

        _turnCoroutine = StartCoroutine(FlipYLerp());
    }


    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotation = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < FlipYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            yRotation = Mathf.Lerp(startRotation, endRotation, (elapsedTime/FlipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }


    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;

        if (_isFacingRight)
        {
            return 0f;
        }
        else
        {
            return 180f;
        }
    }
}
