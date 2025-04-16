using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRespawnController : MonoBehaviour
{
    private List<RhinoMonster> rhinoMonsters;
    private List<GoblinMonster> goblinMonsters;
    void Start()
    {
        rhinoMonsters = new List<RhinoMonster>(FindObjectsOfType<RhinoMonster>());
        goblinMonsters = new List<GoblinMonster>(FindObjectsOfType<GoblinMonster>());
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
            foreach (GoblinMonster goblinMonster in goblinMonsters)
            {
                if (goblinMonster.IsDead)
                {
                    goblinMonster.RessurectMonster();
                }
            }
        }
    }
}
