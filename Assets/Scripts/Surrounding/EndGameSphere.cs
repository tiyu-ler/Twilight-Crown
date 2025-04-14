using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameSphere : MonoBehaviour
{
    private bool _canBePressed = false;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            _canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            _canBePressed = false;
        }
    }

    void Update()
    {
        if (_canBePressed)
        {
            if (playerMovement._isGrounded)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    playerMovement.StopSound = true;
                    playerMovement.CanMove = false;
                    playerMovement.audioSource.Pause();
                    playerMovement.HorizontalInput = 0;
                    StartCoroutine(SphereInteraction());            
                }
            }
        }
    }

    private IEnumerator SphereInteraction()
    {
        // появляеться кот из земли
        // камера становиться статично, и ставиться посредине экрана
        // запускаеться анимация разрушения сферы через 0.3 секунды
        // запускаеться звуковое сопровождение разрушению сферы
        // кот опускаеться под землю
        // на одном из кадров, через какое-то время, пока кот опускаеться, под ним появляеться маленький кот
        // удаляеться большой кот
        // статичная камера перемещаеться на него и уменьшаеться 
        // сужение черного круга вокруг кота
        // звуковой эффект от маленького кота "МЯУ", когда камера приблизиться немного и круг сузиться до определеннного предела
        // круг закрываеться до конца
        // появляеться надпись "Создано таким-то человеком... в качестве бакаларской работы" и нажми любую кнопку
        // после нажатия, текст пропадает, переход на сцену главного меню, данное сохранение удаляеться
        yield return null;
    }
}
