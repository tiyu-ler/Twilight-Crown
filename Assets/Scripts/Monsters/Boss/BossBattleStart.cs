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
            MusicManager.Instance.PlayLoop(MusicManager.Instance.BossMusic, MusicManager.MusicType.Boss);
            foreach (DoorScript doorScript in DoorList)
            {
                doorScript.DoorInteractor(true, true);
            }
            Debug.Log("Battle Start");
            CatBoss.SetActive(true);
            StartCoroutine(BossController.Instance.AppearBeforeIdle(true));
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void OpenDoors(bool Restart, bool PlaySound)
    {
        if (Restart){
            MusicManager.Instance.PlayLoop(MusicManager.Instance.DefaultRoomMusic, MusicManager.MusicType.DefaultRoom);
            foreach (DoorScript doorScript in DoorList)
            {
                doorScript.DoorInteractor(false, PlaySound);
            }
            StartCoroutine(EnableCollider());
        } 
        else 
        {
            DoorList[0].DoorInteractor(false, PlaySound);
        }
    }
    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
