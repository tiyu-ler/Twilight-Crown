using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Settings")]
    public static BossController Instance;
    public Transform BossTransform;
    public Animator BossAnimator;

    public BossBattleStart bossBattleStart;
    public BossHealth bossHealth;
    public BulletSpawn bulletSpawn;
    public Collider2D DefaultCollider;
    public GameObject TentaclesCollider;

    private Vector2 _idlePositionLeft = new Vector2(-0.425f, 0f);
    private Quaternion _idleRotationLeft = Quaternion.Euler(0f, 180f, 0f);
    private Vector2 _idlePositionRight = new Vector2(0.425f, 0f);
    private Quaternion _idleRotationRight = Quaternion.Euler(0f, 0f, 0f);
    private int _currentAttack;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        bossHealth = FindAnyObjectByType<BossHealth>();
    }
    public void RestartBossBattle()
    {
        bossHealth.RestartHp();
        bulletSpawn.StopAllCoroutines();
        StopAllCoroutines();
        bossBattleStart.OpenDoors(true);
        BossAnimator.Play("None");
    }
    public void EndBossBattle()
    {
        bulletSpawn.StopAllCoroutines();
        StopAllCoroutines();
    }
    public IEnumerator IdleState()
    {
        BossAnimator.Play("Idle");
        DefaultCollider.enabled = true;
        yield return new WaitForSeconds(GetAnimationLength("Idle")*Random.Range(2, 5));
        DefaultCollider.enabled = false;
        StartCoroutine(DisappearBeforeAttack());
    }

    private IEnumerator DisappearBeforeAttack()
    {
        BossAnimator.Play("Hide");
        yield return new WaitForSeconds(GetAnimationLength("Hide"));

        StartCoroutine(ChooseRandomAttack());
    }

    private IEnumerator ChooseRandomAttack()
    {
        _currentAttack = Random.Range(0, 2);

        switch (_currentAttack)
        {
            case 0:
                yield return StartCoroutine(Attack1());
                break;
            case 1:
                yield return StartCoroutine(TentaclesAttack());
                break;
            // case 2:
            //     yield return StartCoroutine(TentaclesAttack());
            //     break;
        }
    }

    public IEnumerator AppearBeforeIdle(bool isStart)
    {
        int randomPos = Random.Range(0, 2);
        if (isStart) randomPos = 1;
        BossTransform.localPosition = randomPos == 0 ? _idlePositionLeft : _idlePositionRight;
        BossTransform.localRotation = randomPos == 0 ? _idleRotationLeft : _idleRotationRight;
        DefaultCollider.gameObject.transform.localRotation = randomPos == 0 ? _idleRotationLeft : _idleRotationRight;
        BossAnimator.Play("Appear");
        yield return new WaitForSeconds(GetAnimationLength("Appear"));

        StartCoroutine(IdleState());
    }

    private IEnumerator Attack1()
    {
        BossTransform.gameObject.SetActive(false);
        bulletSpawn.StartBulletAttack();
        yield return null;
    }

    private IEnumerator Attack2()
    {
        BossAnimator.Play("Attack2");
        yield return new WaitForSeconds(GetAnimationLength("Attack2"));
    }

    private IEnumerator TentaclesAttack()
    {
        float[] positions = { -0.2f, 0f, 0.2f };
        float tentacleX = positions[Random.Range(0, positions.Length)];

        BossTransform.localPosition = new Vector2(tentacleX, 0);
        TentaclesCollider.transform.localPosition = new Vector2(tentacleX, 0);

        BossAnimator.Play("Tentacles");

        yield return new WaitForSeconds(GetAnimationLength("Tentacles")*0.5f);
        TentaclesCollider.SetActive(true);
        yield return new WaitForSeconds(GetAnimationLength("Tentacles")*0.4f);
        TentaclesCollider.SetActive(false);
        yield return new WaitForSeconds(GetAnimationLength("Tentacles")*0.1f);

        StartCoroutine(NextAction());
    }

    public IEnumerator NextAction()
    {
        BossTransform.gameObject.SetActive(true);
        float nextAction = Random.Range(0, 2);
        if (nextAction == 0)
        {
            StartCoroutine(AppearBeforeIdle(false));
        }
        else
        {
            StartCoroutine(ChooseRandomAttack());
        }
        yield return null;
    }
    public float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = BossAnimator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
                return clip.length;
        }
        return 0.1f;
    }
}
