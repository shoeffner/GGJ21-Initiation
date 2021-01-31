using UnityEngine;
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


    [Header("Ghost")]
    [SyncVar]
    public bool ghost = false;
    private bool wasGhost = false;
    public Material ghostBodyMaterial;
    public Material ghostHatMaterial;
    public MeshRenderer body;
    public MeshRenderer hat;
    private Material originalBodyMaterial;
    private Material originalHatMaterial;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (ghost && !wasGhost)
        {
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = false;
            }
            wasGhost = true;
            ChangeMaterial(true);
        }
        else if (!ghost && wasGhost)
        {
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = true;
            }
            wasGhost = false;
            ChangeMaterial(false);
        }
    }
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

    public void ChangeMaterial(bool ghost)
    {
        if (originalBodyMaterial == null)
        {
            originalBodyMaterial = body.material;
        }
        if (originalHatMaterial == null)
        {
            originalHatMaterial = hat.material;
        }

        body.material = ghost ? ghostBodyMaterial : originalBodyMaterial;
        hat.material = ghost ? ghostHatMaterial : originalHatMaterial;
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
