﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityManager : NetworkBehaviour
{
    public enum Ability
    {
        FIREBALL, HEALING, SHIELD, GHOST
    }

    [Header("Fireball")]
    public GameObject fireballPrefab;
    public float cooldownFireball = 2.0f;
    [SyncVar]
    private float remainingCooldownFireball = 0;
    public Transform shootAnchor;

    [Header("Healing")]
    public GameObject healingArea;
    public float cooldownHealing = 3.0f;
    public float healingCastRange = 10f;
    public GameObject healingRangeIndicator;
    [SyncVar]
    public float remainingCooldownHealing = 0;

    [Header("Shield")]
    public GameObject shield;
    public float cooldownShield = 4.0f;
    public float shieldCastRange = 5f;
    public float shieldDuration = 3.0f;
    public GameObject shieldRangeIndicator;
    [SyncVar]
    public float remainingCooldownShield = 0;

    [Header("Ghost")]
    public GameObject ghost;
    public float cooldownGhost = 30.0f;
    public float ghostCastRange = 5.0f;
    public float ghostDuration = 3.0f;
    public GameObject ghostRangeIndicator;
    [SyncVar]
    public float remainingCooldownGhost = 0;


    [Header("Permanent and available")]
    [SyncVar]
    public Ability permanentAbility;
    public readonly SyncList<Ability> abilities = new SyncList<Ability>();

    public override void OnStartLocalPlayer()
    {
        CmdLearnAllAbilities();
        healingRangeIndicator.transform.localScale = new Vector3(healingCastRange, 0, healingCastRange);
        shieldRangeIndicator.transform.localScale = new Vector3(shieldCastRange, 0, shieldCastRange);
        ghostRangeIndicator.transform.localScale = new Vector3(ghostCastRange, 0, ghostCastRange);
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
    public void CmdHeal(Vector3 targetPosition)
    {  
        if (!abilities.Contains(Ability.HEALING))
        {
            Debug.Log("Healing unavailable");
            return;
        }
        if (remainingCooldownHealing > 0)
        {
            return;
        }
        if (Vector3.Distance(targetPosition, transform.position) > healingCastRange)
        {
            return;
        }
        remainingCooldownHealing = cooldownHealing;
        GameObject healing = Instantiate(healingArea);
        healing.GetComponent<HealingArea>().CastAt(targetPosition);
        NetworkServer.Spawn(healing);
    }

    [Command]
    public void CmdShield(CharacterStats character)
    {
        if (!abilities.Contains(Ability.SHIELD))
        {
            Debug.Log("Shield unavailable");
            return;
        }
        if (remainingCooldownShield > 0)
        {
            return;
        }
        if (Vector3.Distance(character.transform.position, transform.position) > shieldCastRange)
        {
            return;
        }
        remainingCooldownShield = cooldownShield;
        GameObject shield = Instantiate(this.shield);
        shield.GetComponent<Shield>().CastAt(character, shieldDuration);
        NetworkServer.Spawn(shield);
    }


    [Command]
    public void CmdGhost(CharacterStats character)
    {
        if (!abilities.Contains(Ability.GHOST))
        {
            Debug.Log("Ghost unavailable");
            return;
        }
        if (remainingCooldownGhost > 0)
        {
            return;
        }
        if (Vector3.Distance(character.transform.position, transform.position) > ghostCastRange)
        {
            return;
        }
        remainingCooldownGhost = cooldownGhost;
        GameObject ghost = Instantiate(this.ghost);
        ghost.GetComponent<Ghost>().CastAt(character, ghostDuration);
        NetworkServer.Spawn(ghost);
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
        Debug.Log($"Lost {available[0]}");
    }

    [Command]
    public void CmdLearnAllAbilities()
    {
        abilities.AddRange(new List<Ability>{
            Ability.FIREBALL,
            Ability.HEALING,
            Ability.SHIELD,
            Ability.GHOST,
        });
    }

    public void Update()
    {
        float dt = Time.deltaTime;
        remainingCooldownFireball -= dt;
        remainingCooldownHealing -= dt;
        remainingCooldownShield -= dt;
        remainingCooldownGhost -= dt;
    }

    [ClientRpc]
    public void RpcOnRuneActivation(RuneManager runeManager)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Debug.Log("Rune!");
        CmdLoseAbility();
    }
}
