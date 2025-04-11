using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Collider2D first;
    public Collider2D second;
    public CoinUIManager coinUIManager;

    private Transform player;
    private bool shouldMagnet = false;
    private float magnetSpeed = 15f;
    private float acceleration = 30f;
    private Rigidbody2D rb;

    private void Start()
    {
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            first.enabled = false;
            PlayerDataSave.Instance.Money++;
            coinUIManager.UpdateCoinsUI(PlayerDataSave.Instance.Money);
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Static;

            Destroy(gameObject, 0.1f);
        }
    }
}
