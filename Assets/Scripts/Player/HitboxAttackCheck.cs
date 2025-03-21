using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAttackCheck : MonoBehaviour
{
    private float Damage = 1f;
    public string attackDirection; //right, up, bottom

    void Start()
    {
        Damage = 0.4f + 0.6f * PlayerDataSave.Instance.SwordLevel; // 1, 1.6, 2.2
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Monster":
                MonsterScript monster = collision.GetComponent<MonsterScript>();
                if (monster != null) monster.TakeDamage(Damage, attackDirection);
                break;
            case "MoneyBag":
                MoneyBag moneyBag = collision.GetComponent<MoneyBag>();
                if (moneyBag != null) moneyBag.SpawnCoins();
                break;
            case "CatBoss":
                BossHealth bossHealth = collision.GetComponent<BossHealth>();
                if (bossHealth != null) bossHealth.TakeDamage(Damage, attackDirection);
                break;
        }
    }
}
