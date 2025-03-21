using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BossBattleStart : MonoBehaviour
{
    public List<DoorScript> DoorList = new List<DoorScript>();
    public GameObject CatBoss;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (DoorScript doorScript in DoorList)
            {
                doorScript.DoorInteractor(true);
            }
            Debug.Log("Battle Start");
            CatBoss.SetActive(true);
            StartCoroutine(BossController.Instance.AppearBeforeIdle(true));
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void OpenDoors(bool Restart)
    {
        if (Restart){
            foreach (DoorScript doorScript in DoorList)
            {
                doorScript.DoorInteractor(false);
            }
            StartCoroutine(EnableCollider());
            Debug.Log("OpenDoors");
        } 
        else 
        {
            DoorList[0].DoorInteractor(false);
        }
    }
    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
