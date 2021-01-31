using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(SphereCollider))]
public class RitualTrigger : NetworkBehaviour
{
    public List<GameObject> activateOnGameWon;
    public SphereCollider sphereCollider;

    public void OnRuneActivation(RuneManager runeManager)
    {
        activeRunes.Add(runeManager);
    }

    [SyncVar]
    public int requiredRunes = 4;

    public readonly SyncHashSet<RuneManager> activeRunes = new SyncHashSet<RuneManager>();

    [SyncVar]
    public int playersRequiredForRitual = 0;

    [SyncVar]
    public int playersReadyForRitual = 0;

    void OnTriggerStay(Collider collider)
    {
        if (!isServer)
        {
            return;
        }
        if (activeRunes.Count < requiredRunes)
        {
            return;
        }
        if (playersRequiredForRitual < 1)
        {
            return;
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position + sphereCollider.center - Vector3.up, sphereCollider.radius * transform.localScale.x, Vector3.up, sphereCollider.radius, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
        playersReadyForRitual = hits.Length;

        
        if (playersReadyForRitual >= playersRequiredForRitual)
        {
            RpcGameWon();
        }
    }

    [ClientRpc]
    void RpcGameWon()
    {
        Debug.Log("GAME WON!");
		foreach(GameObject obj in activateOnGameWon) {
            obj.SetActive(true);
		}
    }
}
