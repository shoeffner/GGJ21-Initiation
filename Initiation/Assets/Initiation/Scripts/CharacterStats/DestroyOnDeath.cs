using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterStats))]
public class DestroyOnDeath : NetworkBehaviour
{
    CharacterStats cs;

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
        if (cs.isDead)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
