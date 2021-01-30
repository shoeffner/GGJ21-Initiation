using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterStats))]
public class DestroyOnDeathAfterTime : NetworkBehaviour
{
    public float timeToLiveAfterDeath = 10;
    private CharacterStats cs;

    void Start()
    {
        if (cs == null)
        {
            cs = gameObject.GetComponent<CharacterStats>();
        }
    }

    void Update()
    {
        if (cs.dead)
        {
            timeToLiveAfterDeath -= Time.deltaTime;
            if (timeToLiveAfterDeath <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
