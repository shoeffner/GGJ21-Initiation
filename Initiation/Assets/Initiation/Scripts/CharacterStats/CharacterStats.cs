﻿using UnityEngine;
using Mirror;
using System;

public class CharacterStats : NetworkBehaviour
{
    public event Action<CharacterStats> OnTakeDamage;
    public event Action<CharacterStats> OnHeal;
    public event Action<CharacterStats> OnDie;

    [SyncVar]
    public int maxHealth = 10;

    [SyncVar]
    public int health = 10;
    
    [SyncVar]
    public bool dead = false;

    [SyncVar]
    public bool isShielded = false;

    public void Awake()
    {
        health = Math.Min(health, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (dead)
        {
            return;
        }

        if (!isShielded)
        {
            health -= amount;
            OnTakeDamage?.Invoke(this);
        }
        if (health <= 0)
        {
            health = 0;
            OnDie?.Invoke(this);
            dead = true;
        }
    }

    public void Heal(int amount)
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
}
