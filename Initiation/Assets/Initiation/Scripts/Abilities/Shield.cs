using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Shield : NetworkBehaviour
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

    void FixedUpdate()
    {
        if (characterStats != null) {
            transform.position = characterStats.transform.position;
            transform.Rotate(Vector3.up, Mathf.PingPong(Time.time * 0.5f, 360));
        }
    }

    public void CastAt(CharacterStats character, float duration = 0f)
    {
        this.duration = duration;
        characterStats = character;
        characterStats.isShielded = true;
    }

    public void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.isShielded = false;
        }
    }
}
