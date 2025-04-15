using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public SpriteRenderer _renderer;
    public Color HitColor;
    public BossBattleStart bossBattleStart;
    private bool _isDead = false;
    static private float _maxHealth = 18;
    private float _currentHealth = 18;
    private bool _canTakeDamage = true;
    private bool _hasLaunchedPlayer = false;
    private PlayerMovement _player;
    private Coroutine _airLaunch;
    private GameObject _bossBattleStart;
    private GameManager gameManager;
    void Start()
    {
        // Debug.Log(_currentHealth);
        RestartHp();
        gameManager = FindObjectOfType<GameManager>();
        _player = FindAnyObjectByType<PlayerMovement>();
        _bossBattleStart = FindAnyObjectByType<BossBattleStart>().gameObject;
        if (PlayerDataSave.Instance.catBossKilled)
        {  
            DestroyObjects();
        }
    }
    public void RestartHp()
    {
        _currentHealth = _maxHealth;
    }
    public void TakeDamage(float damage, string attackDirection)
    {
        if (_isDead || !_canTakeDamage) return;
        
        if (attackDirection == "bottom" && !_hasLaunchedPlayer)
        {
            if (_airLaunch == null)
            {
                _airLaunch = StartCoroutine(LaunchInTheAir());
            }
        }

        _currentHealth -= damage;
        _canTakeDamage = false;
        if (_currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(GetHitted());
        }
    }

    private IEnumerator GetHitted()
    {   
        Color originalColor = _renderer.color;

        _renderer.color = HitColor;

        yield return new WaitForSeconds(0.1f);

        _canTakeDamage = true;
        _renderer.color = originalColor;
        
    }

    private IEnumerator LaunchInTheAir()
    {
        yield return new WaitForSeconds(0.05f);
        _hasLaunchedPlayer = true;
        _player.RigidBody.AddForce(Vector2.up * 30f, ForceMode2D.Impulse);
        StartCoroutine(ResetLaunchFlag());
    }
    private IEnumerator ResetLaunchFlag()
    {
        yield return new WaitForSeconds(0.1f);
        _hasLaunchedPlayer = false;
    }

    private IEnumerator Die()
    {
        Debug.Log("Cat Boss Died after "+ BossController.Instance.GetAnimationLength("Hide"));
        BossController.Instance.BossAnimator.Play("Hide");
        MusicManager.Instance.PlayLoop(MusicManager.Instance.DefaultRoomMusic, MusicManager.MusicType.DefaultRoom);
        yield return new WaitForSeconds(BossController.Instance.GetAnimationLength("Hide"));
        BossController.Instance.BossAnimator.Play("None");
        bossBattleStart.OpenDoors(false, true);
        PlayerDataSave.Instance.catBossKilled = true;
        gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        DestroyObjects();
    }
    private void DestroyObjects()
    {
        Destroy(_bossBattleStart);
        Destroy(BossController.Instance.gameObject);
    }
}
