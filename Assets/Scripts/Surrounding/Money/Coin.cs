using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Collider2D first;
    public Collider2D second;
    public CoinUIManager coinUIManager;

    private bool hasFallen = false;
    private LayerMask groundLayer;
    private Transform player;
    private bool shouldMagnet = false;
    private float magnetSpeed = 15f;
    private float acceleration = 30f;
    private Rigidbody2D rb;

    private void Start()
    {
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        coinUIManager = FindAnyObjectByType<CoinUIManager>();
        StartCoroutine(WaitBeforeMagnet());
    }

    private IEnumerator WaitBeforeMagnet()
    {
        yield return new WaitForSeconds(1f);
        shouldMagnet = true;
    }

    private void Update()
    {
        if (shouldMagnet && player != null)
        {
            second.enabled = false;

            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * magnetSpeed * Time.deltaTime;

            // Accelerate as it flies
            magnetSpeed += acceleration * Time.deltaTime;
        }

         if (!hasFallen)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
            if (hit.collider != null)
            {
                FallOnGround();
                hasFallen = true;
            }
        }
    }
    private void FallOnGround()
    {
        int i = Random.Range(0, 1);
        SoundManager.SoundID CoinHitGround = SoundManager.SoundID.CoinHitGround1 + i;
        SoundManager.Instance.PlaySound(CoinHitGround, worldPos: transform.position, volumeUpdate: 0.12f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int i = Random.Range(0, 4);
            SoundManager.SoundID CoinCollect = SoundManager.SoundID.CoinCollect1 + i;
            SoundManager.Instance.PlaySound(CoinCollect, worldPos: transform.position, volumeUpdate: 0.02f);

            first.enabled = false;
            PlayerDataSave.Instance.Money++;
            coinUIManager.UpdateCoinsUI(PlayerDataSave.Instance.Money);
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Static;

            Destroy(gameObject, 0.1f);
        }
    }
}
