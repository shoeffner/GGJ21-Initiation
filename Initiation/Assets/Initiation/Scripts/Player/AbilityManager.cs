﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityManager : NetworkBehaviour
{
    public enum Ability
    {
        FIREBALL, HEALING
    }

    public GameObject fireballPrefab;
    public Transform shootAnchor;

    [Range(0, 5)] 
    public float cooldownFireball = 2.0f;

    [SyncVar]
    private float remainingCooldownFireball = 0;

    [SyncVar]
    public Ability permanentAbility;

    public readonly SyncList<Ability> abilities = new SyncList<Ability>();

    public override void OnStartLocalPlayer()
    {
        CmdLearnAllAbilities();
    }

    [Command]
    public void CmdCastFireball()
    {
        if (!abilities.Contains(Ability.FIREBALL))
        {
            Debug.Log("Fireball unavailable");
            return;
        }
        if (remainingCooldownFireball > 0)
        {
            return;
        }
        remainingCooldownFireball = cooldownFireball;
        GameObject fireball = Instantiate(fireballPrefab);
        fireball.GetComponent<Fireball>().CastBy(gameObject);
        NetworkServer.Spawn(fireball);
    }

    [Command]
    public void CmdHeal(CharacterStats target, int amount)
    {  
        if (!abilities.Contains(Ability.HEALING))
        {
            Debug.Log("Healing unavailable");
            return;
        }
        target.Heal(amount);
    }

    [Command]
    public void CmdLoseAbility()
    {
        if (abilities.Count <= 1)
        {
            return;
        }

        List<Ability> available = new List<Ability>(abilities);
        available.Remove(permanentAbility);
        available.Shuffle();
        abilities.Remove(available[0]);
    }

    [Command]
    public void CmdLearnAllAbilities()
    {
        abilities.AddRange(new List<Ability>{
            Ability.FIREBALL,
            Ability.HEALING
        });
    }

    public void Update()
    {
        remainingCooldownFireball -= Time.deltaTime;
    }

    [ClientRpc]
    public void RpcOnRuneActivation(RuneManager runeManager)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Debug.Log("Rune!");
    }
}
