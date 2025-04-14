using UnityEngine;

public class MoneyBag : MonoBehaviour
{
    public int hitsRequired = 1;
    public int minCoinsPerHit = 5;
    public int maxCoinsPerHit = 9;
    public GameObject coinPrefab;

    private int currentHits = 0;

    public void SpawnCoins()
    {
        int coinsToSpawn = Random.Range(minCoinsPerHit, maxCoinsPerHit + 1);
        SoundManager.Instance.PlaySound(SoundManager.SoundID.MoneyBagBreak, worldPos: transform.position, volumeUpdate: 0.02f);
        for (int i = 0; i < coinsToSpawn; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            // coin.GetComponent<Coin>().value = 1;
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // float angle = Random.Range(315f, 405f);
                // Vector2 force = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * Random.Range(20f, 21f);
                Vector2 force = new Vector2(Random.Range(-12f, 12f), Random.Range(9f, 12f));
                // Vector2 force = new Vector2(-0.5f, 1);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        currentHits += 1;
        if (currentHits == hitsRequired)
        {
            Destroy(gameObject);
        }
    }
}
