using UnityEngine;

public class Coin : MonoBehaviour
{
    public Collider2D first;
    public Collider2D second;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDataSave.Instance.Money += 5;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            first.enabled = false;
            second.enabled = false;
            Destroy(gameObject, 0.1f);
        }
    }
}
