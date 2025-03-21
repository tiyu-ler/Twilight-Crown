using System.Collections;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Vector3 _startPosition;
    public float EndHeight = 0.5f;

    void Start()
    {
        _startPosition = transform.localPosition;
    }

    public void DoorInteractor(bool LerpDown)
    {
        StartCoroutine(LerpDoor(LerpDown));
    }
    private IEnumerator LerpDoor(bool LerpDown)
    {
        float elapsedTime = 0f;
        float LerpTime = 0.1f;
        Vector3 currentHeight, endHeight;
        if (LerpDown)
        {
            currentHeight = _startPosition;
            endHeight = new Vector3(transform.localPosition.x, EndHeight, transform.localPosition.z);
        }
        else
        {
            currentHeight = new Vector3(transform.localPosition.x, EndHeight, transform.localPosition.z);
            endHeight = _startPosition;
        }
        while (elapsedTime < LerpTime)
        {
            elapsedTime += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(currentHeight, endHeight, elapsedTime/LerpTime);
            
            yield return null;
        }
        transform.localPosition = endHeight;
    }
}
