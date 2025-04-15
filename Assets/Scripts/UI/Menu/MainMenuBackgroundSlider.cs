using System.Collections;
using UnityEngine;

public class MainMenuBackgroundSlider : MonoBehaviour
{

    void Awake()
    {
        transform.localScale = new Vector3(2f, 2f, 2f);
        StartCoroutine(LerpBackground());
    }

    private IEnumerator LerpBackground()
    {
        float elapsedTime = 0f;
        float LerpTime = 5f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(3, 3, 3);

        while (elapsedTime < LerpTime)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime <= LerpTime/2)
            {
                elapsedTime += Time.deltaTime;
            }
            if (elapsedTime <= LerpTime/1.2)
            {
                elapsedTime += Time.deltaTime;
            }
            if (elapsedTime <= LerpTime/1.03)
            {
                elapsedTime += Time.deltaTime;
            }
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / LerpTime);

            yield return null;
        }
    }
}
