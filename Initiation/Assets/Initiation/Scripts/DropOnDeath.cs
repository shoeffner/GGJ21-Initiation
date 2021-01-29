using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterStats))]
public class DropOnDeath : NetworkBehaviour
{
    public GameObject spawnOnDeath;
    public float spawnOffset = 1;

    private CharacterStats cs;
    private bool spawned = false;

    void Start()
    {
        if (cs == null)
        {
            cs = GetComponent<CharacterStats>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cs.isDead && !spawned)
        {
            GameObject toSpawn = Instantiate(spawnOnDeath);
            toSpawn.transform.position = transform.position + transform.forward * spawnOffset;
            NetworkServer.Spawn(toSpawn);
            spawned = true;
        }
    }
}
