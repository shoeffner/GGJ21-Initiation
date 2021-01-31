using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ghost : NetworkBehaviour
{
    [Header("Functionality")]
    [SyncVar]
    public float duration = 3.0f;
    [SyncVar]
    public CharacterStats characterStats;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    public void CastAt(CharacterStats character, float duration = 0f)
    {
        this.duration = duration;
        characterStats = character;
        characterStats.ghost = true;
    }

    public void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.ghost = false;
        }
    }
}
