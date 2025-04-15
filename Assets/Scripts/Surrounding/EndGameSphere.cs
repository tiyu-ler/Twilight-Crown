using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
public class EndGameSphere : MonoBehaviour
{
    private bool _canBePressed = false;
    private PlayerMovement playerMovement;
    public GameObject BigCat;
    public Material maskMaterial;
    public GameObject SmallCat;
    public Transform FollowObject;
    public Transform MiddlePosition, SmallCatPosition;
    public GameObject Light, LightAdditional;
    public List<SpriteRenderer> Tiles = new List<SpriteRenderer>();
    public CinemachineVirtualCamera DynamicCamera;
    // public CanvasGroup Credits;
    public float targetRadius = 0.09f;
    public Sprite MeowSprite;
    // public GameObject CreditsText;
    private Animator _bigCatAnimator, _smallCatAnimator, SphereAnimator;
    private bool _retunBackToMainMenu;
    private GameObject _player;
    public Color DefaultColor;
    public MarkerTextPopUp DestroyText;
    void Start()
    {
        maskMaterial.SetFloat("_Radius", 0.4f);
        SphereAnimator = GetComponent<Animator>();
        _bigCatAnimator = BigCat.GetComponent<Animator>();
        _smallCatAnimator = SmallCat.GetComponent<Animator>();
        _retunBackToMainMenu = false;
        BigCat.SetActive(false);
        SmallCat.SetActive(false);
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            _player = collider.gameObject;
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
                    _canBePressed = false;
                    playerMovement.StopSound = true;
                    playerMovement.CanMove = false;
                    playerMovement.audioSource.Pause();
                    playerMovement.HorizontalInput = 0;
                    DestroyText.DisableMarkUp();
                    MiddlePosition.transform.position = (SmallCat.transform.position + _player.transform.position)/2;
                    MiddlePosition.transform.position = new Vector2(MiddlePosition.transform.position.x, 91.74f);
                    StopAllCoroutines();
                    StartCoroutine(SphereInteraction());            
                }
            }
        }
        if (_retunBackToMainMenu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(TransferToMainMenu());
            }
        }
    }

    public IEnumerator SphereFly()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 pos1 = startPos;
        Vector3 pos2 = startPos + new Vector3(0, -0.18f, 0);
        Vector3 pos3 = startPos + new Vector3(0,  0.18f, 0);


        while (true)
        {
            yield return LerpPosition(pos1, pos2, 0.5f);
            yield return LerpPosition(pos2, pos1, 0.5f);
            yield return LerpPosition(pos1, pos3, 0.5f);
            yield return LerpPosition(pos3, pos1, 0.5f);
        }
    }

    private IEnumerator LerpPosition(Vector3 startState, Vector3 endState, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startState, endState, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endState;
    }

    private void HideLigth()
    {
        Destroy(Light);
        Destroy(LightAdditional);
        foreach (SpriteRenderer sr in Tiles)
        {
            sr.color = DefaultColor;
        }
    }

    private IEnumerator SphereInteraction()
    {
        FollowObject.transform.position = _player.transform.position; 
        DynamicCamera.Follow = FollowObject;
        BigCat.SetActive(true);
        BigCat.GetComponent<Animator>().Play("Appear");
        SoundManager.Instance.PlaySound(SoundManager.SoundID.CatbossAppear, worldPos: transform.position, volumeUpdate: 0.03f);
        yield return StartCoroutine(FollowObjectMover(MiddlePosition));
        yield return new WaitForSeconds(GetAnimationLength("Appear", _bigCatAnimator));
        
        BigCat.GetComponent<Animator>().Play("Idle");
        
        // SphereAnimator.Play("SphereDestruction");
        // SoundManager.Instance.PlaySound(SoundManager.SoundID.SphereDestructSoundGlass, worldPos: transform.position, volumeUpdate: 0.03f);
        // SoundManager.Instance.PlaySound(SoundManager.SoundID.SphereDestructSoundSouls, worldPos: transform.position, volumeUpdate: 0.03f); 
        // HideLight в определенный момент анимации
        HideLigth();
        yield return new WaitForSeconds(1.5f);

        BigCat.GetComponent<Animator>().Play("Hide");
        SoundManager.Instance.PlaySound(SoundManager.SoundID.CatbossHide, worldPos: transform.position, volumeUpdate: 0.03f);
        yield return new WaitForSeconds(GetAnimationLength("Hide", _bigCatAnimator)*0.8f);

        SmallCat.SetActive(true);
        yield return new WaitForSeconds(GetAnimationLength("Hide", _bigCatAnimator)*0.2f);
        Destroy(BigCat);

        SmallCat.GetComponent<Animator>().Play("SmallCatWalk");
        yield return StartCoroutine(SmallCatLerpRight());
        SmallCat.GetComponent<Animator>().Play("SmallCatStand");
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(FollowObjectMover(SmallCatPosition));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ZoomCamera(DynamicCamera, 1.5f, 1f));

        yield return StartCoroutine(LerpMaskRadius(maskMaterial,0.12f, 3f));

        // SoundManager.Instance.PlaySound(SoundManager.SoundID.Meow, worldPos: transform.position, volumeUpdate: 0.03f); 

        SmallCat.GetComponent<Animator>().StopPlayback();
        SmallCat.GetComponent<SpriteRenderer>().sprite = MeowSprite;
        yield return new WaitForSeconds(0.2f);
        SmallCat.GetComponent<Animator>().StartPlayback();
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(LerpMaskRadius(maskMaterial, 0, 1.2f));

        // титры

        yield return null;
    }

    private IEnumerator SmallCatLerpRight()
    {
        Vector3 startPos = SmallCat.transform.position;
        Vector3 endPos = startPos + new Vector3(5f, 0f, 0f);
        float duration = 4f;
        float time = 0f;
        while (time < duration)
        {
            SmallCat.transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        SmallCat.transform.position = endPos;
    }

    private IEnumerator FollowObjectMover(Transform newPos)
    {
        float speed = 8f;
        while (Vector3.Distance(FollowObject.transform.position, newPos.position) > 0.01f)
        {
            FollowObject.transform.position = Vector3.MoveTowards(FollowObject.transform.position, newPos.position, speed * Time.deltaTime);
            yield return null;
        }
        FollowObject.transform.position = newPos.position;
    }

    private IEnumerator ZoomCamera(CinemachineVirtualCamera cam, float targetSize, float duration)
    {
        float startSize = cam.m_Lens.OrthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.m_Lens.OrthographicSize = targetSize;
    }

    private IEnumerator LerpMaskRadius(Material mat, float targetRadius, float duration)
    {
        float startRadius = mat.GetFloat("_Radius");
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float currentRadius = Mathf.Lerp(startRadius, targetRadius, elapsed / duration);
            mat.SetFloat("_Radius", currentRadius);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetFloat("_Radius", targetRadius);
    }

    private IEnumerator TransferToMainMenu()
    {

        yield return null;
    }

    public float GetAnimationLength(string animationName, Animator animator)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
                return clip.length;
        }
        return 0.1f;
    }
}

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