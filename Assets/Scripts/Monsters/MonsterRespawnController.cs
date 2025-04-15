using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRespawnController : MonoBehaviour
{
    private List<RhinoMonster> rhinoMonsters;
    // private List<GoblinMonster> goblinMonsters = new List<GoblinMonster>();
    void Start()
    {
        rhinoMonsters = new List<RhinoMonster>(FindObjectsOfType<RhinoMonster>());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            foreach (RhinoMonster rhinoMonster in rhinoMonsters)
            {
                if (rhinoMonster.IsDead)
                {
                    rhinoMonster.RessurectMonster();
                }
            }
        }
    }
}
