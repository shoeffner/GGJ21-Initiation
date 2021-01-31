using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityManager : NetworkBehaviour
{
    public enum Ability
    {
        FIREBALL, HEALING
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
    public float healingRange = 10f;
    public GameObject healingRangeIndicator;

    [SyncVar]
    public float remainingCooldownHealing = 0;

    [Header("Permanent and available")]
    [SyncVar]
    public Ability permanentAbility;
    public readonly SyncList<Ability> abilities = new SyncList<Ability>();

    public override void OnStartLocalPlayer()
    {
        CmdLearnAllAbilities();
        healingRangeIndicator.transform.localScale = new Vector3(healingRange, 0, healingRange);
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
        if (Vector3.Distance(targetPosition, transform.position) > healingRange)
        {
            return;
        }
        remainingCooldownHealing = cooldownHealing;
        GameObject healing = Instantiate(healingArea);
        healing.GetComponent<HealingArea>().CastAt(targetPosition);
        NetworkServer.Spawn(healing);
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
            Ability.HEALING
        });
    }

    public void Update()
    {
        float dt = Time.deltaTime;
        remainingCooldownFireball -= dt;
        remainingCooldownHealing -= dt;
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
