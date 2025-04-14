using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    public GameObject Bullet;
    public BossController bossController;
    private readonly List<float> _spawnCoordinates = new List<float> 
    { 
        -1.52f, -1.33f, -1.14f, -0.95f, -0.76f, -0.57f, -0.38f, -0.19f, 
        0f, 0.19f, 0.38f, 0.57f, 0.76f, 0.95f, 1.14f, 1.33f, 1.52f 
    }; 
    private readonly List<List<int>> _spawnPresets = new List<List<int>>()
    {
        new List<int> { 0, 1, 7, 8, 9, 15, 16 },
        new List<int> { 0, 3, 4, 6, 10, 12, 13, 16 },
        new List<int> { 0, 1, 5, 6, 10, 11, 15, 16 },
        new List<int> { 3, 4, 7, 8, 9, 13, 14 }
    };
    void SpawnBullets(int id)
    {
        foreach (int pos in _spawnPresets[id])
        {
            GameObject bullet = Instantiate(Bullet, transform);
            bullet.transform.localPosition = new Vector3(_spawnCoordinates[pos], 0, 0);
        }
    }

    public void StartBulletAttack()
    {
        StartCoroutine(BulletAttack(Random.Range(2,5)));
    }

    private IEnumerator BulletAttack(int count)
    {
        yield return new WaitForSeconds(0.2f);
        while(count > 0)
        {
            count -= 1;
            SpawnBullets(Random.Range(0,4));
            yield return new WaitForSeconds(1.1f);
        }
        StartCoroutine(bossController.NextAction());
    }
}
