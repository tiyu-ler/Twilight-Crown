using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
public class EndGameSphere : MonoBehaviour
{
    private bool _canBePressed = false;
    private PlayerMovement playerMovement;
    public GameObject BigCat;
    public Material maskMaterial;
    public GameObject SmallCat;
    public GameObject MaskGameobject;
    public Transform FollowObject;
    public Transform MiddlePosition, SmallCatPosition;
    public GameObject Light, LightAdditional;
    public List<SpriteRenderer> Tiles = new List<SpriteRenderer>();
    public CinemachineVirtualCamera DynamicCamera;
    public CanvasGroup Credits;
    public CanvasGroup CreditsText;
    public float targetRadius = 0.09f;
    private Animator _bigCatAnimator, _smallCatAnimator, SphereAnimator;
    private bool _retunBackToMainMenu;
    private GameObject _player;
    public Color DefaultColor;
    private InGamePauseMenu inGamePauseMenu;
    public MarkerTextPopUp DestroyText;
    void Start()
    {
        Credits.alpha = 0;
        Credits.gameObject.SetActive(false);
        CreditsText.alpha = 1;
        MaskGameobject.SetActive(false);
        inGamePauseMenu = FindAnyObjectByType<InGamePauseMenu>();
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
                    inGamePauseMenu.ResumeGame();
                    inGamePauseMenu.IsAbleToPause = false;
                    _player.GetComponent<PlayerMovement>().FlipCharacter(true, 0);
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

    private IEnumerator HideLigth(float duration)
    {
        float time = 0f;

        Light2D lightMain = Light.GetComponent<Light2D>();
        Light2D lightAdditional = LightAdditional.GetComponent<Light2D>();

        float startIntensityMain = lightMain.intensity;
        float startIntensityAdditional = lightAdditional.intensity;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            lightMain.intensity = Mathf.Lerp(startIntensityMain, 0f, t);
            lightAdditional.intensity = Mathf.Lerp(startIntensityAdditional, 0.1f, t);

            yield return null;
        }

        Destroy(Light);
        // Destroy(LightAdditional);

        // foreach (SpriteRenderer sr in Tiles)
        // {
        //     sr.color = DefaultColor;
        // }
    }


    private IEnumerator SphereInteraction()
    {
        MaskGameobject.SetActive(true);
        FollowObject.transform.position = _player.transform.position; 
        DynamicCamera.Follow = FollowObject;
        BigCat.SetActive(true);
        BigCat.GetComponent<Animator>().Play("Appear");
        SoundManager.Instance.PlaySound(SoundManager.SoundID.CatbossAppear, worldPos: transform.position, volumeUpdate: 0.03f);
        yield return StartCoroutine(FollowObjectMover(MiddlePosition));
        yield return new WaitForSeconds(GetAnimationLength("Appear", _bigCatAnimator));
        
        BigCat.GetComponent<Animator>().Play("Idle");
        
        SphereAnimator.Play("SphereBreakAnimation");
        StartCoroutine(HideLigth(GetAnimationLength("SphereBreakAnimation", SphereAnimator)));
        yield return new WaitForSeconds(GetAnimationLength("SphereBreakAnimation", SphereAnimator)*0.05f);
        SoundManager.Instance.PlaySound(SoundManager.SoundID.SphereDestructSoundGlass, worldPos: transform.position, volumeUpdate: 0.1f);
        SoundManager.Instance.PlaySound(SoundManager.SoundID.SphereDestructSoundSouls, worldPos: transform.position, volumeUpdate: 0.2f); 
        yield return new WaitForSeconds(GetAnimationLength("SphereBreakAnimation", SphereAnimator)*0.75f);
        SoundManager.Instance.PlaySound(SoundManager.SoundID.Invoke, worldPos: transform.position, volumeUpdate: 0.4f); 
        
        SphereAnimator.Play("SphereBroken");
        

        BigCat.GetComponent<Animator>().Play("Hide");
        SoundManager.Instance.PlaySound(SoundManager.SoundID.CatbossHide, worldPos: transform.position, volumeUpdate: 0.03f);
        yield return new WaitForSeconds(GetAnimationLength("Hide", _bigCatAnimator)*0.8f);

        SmallCat.SetActive(true);
        yield return new WaitForSeconds(GetAnimationLength("Hide", _bigCatAnimator)*0.2f);
        Destroy(BigCat);

        // StartCoroutine(HideLigth());
        yield return new WaitForSeconds(1f);

        SmallCat.GetComponent<Animator>().Play("SmallCatWalk");
        yield return StartCoroutine(SmallCatLerpRight());
        SmallCat.GetComponent<Animator>().Play("SmallCatStand");
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(FollowObjectMover(SmallCatPosition));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ZoomCamera(DynamicCamera, 1.5f, 1f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(LerpMaskRadius(maskMaterial,0.12f, 2f));

        SoundManager.Instance.PlaySound(SoundManager.SoundID.Meow, worldPos: transform.position, volumeUpdate: 0.32f); 

        SmallCat.GetComponent<Animator>().Play("SmallCatMeow");
        yield return new WaitForSeconds(0.3f);
        SmallCat.GetComponent<Animator>().Play("SmallCatStand");
        yield return new WaitForSeconds(0.5f);

        Credits.gameObject.SetActive(true);
        yield return StartCoroutine(LerpMaskRadius(maskMaterial, 0, 0.8f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(CanvasGroupLerp(Credits, 1f));
        yield return new WaitForSeconds(1.6f);
        
        yield return StartCoroutine(CanvasGroupLerp(CreditsText, 0f));
        yield return new WaitForSeconds(0.3f);
        SaveSystem.DeleteSave(PlayerDataSave.Instance.saveID);
        SceneManager.LoadScene("MainMenu");
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

    private IEnumerator CanvasGroupLerp(CanvasGroup canvasGroup, float endAlpha)
    {
        float duration = 1f;
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
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