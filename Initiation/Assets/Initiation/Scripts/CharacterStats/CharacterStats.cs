using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterStats : NetworkBehaviour
{
    [SyncVar]
    public int health = 10;

    [SyncVar]
    public int armor = 0;

    [SyncVar]
    public bool isDead = false;

    void Update()
    {
        if (isLocalPlayer)
        {
            if (isDead)
                Debug.Log($"{gameObject} died");
            return;
        }

        if (health <= 0)
        {
            isDead = true;
            if (gameObject.GetComponentInChildren<Animator>() != null)
            {
                gameObject.GetComponentInChildren<Animator>().SetBool("isDead", true);
            }
            return;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CharacterStatChanger[] statChangers = collision.collider.GetComponents<CharacterStatChanger>();
        foreach (CharacterStatChanger statChanger in statChangers)
        {
            statChanger.Apply(this);
        }
    }
}
