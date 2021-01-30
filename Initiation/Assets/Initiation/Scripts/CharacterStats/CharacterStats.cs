using UnityEngine;
using Mirror;
using System;

public class CharacterStats : NetworkBehaviour
{
    public event Action<CharacterStats> OnTakeDammage;
    public event Action<CharacterStats> OnHeal;
    public event Action<CharacterStats> OnDie;


    public int maxHealth = 10;

    [SyncVar]
    public int health = 10;
    
    [SyncVar]
    public bool dead = false;

    [Command]
    public void CmdTakeDamage(int amount)
    {
        if (dead)
        {
            return;
        }

        health -= amount;
        OnTakeDammage?.Invoke(this);
        if (health <= 0)
        {
            health = 0;
            OnDie?.Invoke(this);
            dead = true;
        }
    }

    [Command]
    public void CmdHeal(int amount)
    {
        if (dead)
        {
            return;
        }

        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        OnHeal?.Invoke(this);
    }

	//private void OnTriggerEnter(Collider other)
	//{
 //       print($"CharacterStats {name} OnTriggerEnter");
	//}

}
