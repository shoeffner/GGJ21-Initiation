using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(SphereCollider))]
public class HealingArea : NetworkBehaviour
{
    [Header("Functionality")]
    public float duration = 4.0f;
    public int healAmount = 5;

    [Header("Internal")]
    public readonly SyncList<GameObject> healed = new SyncList<GameObject>();

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    public void CastAt(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    void OnTriggerStay(Collider collider)
    {
        GameObject target = collider.gameObject;
        if (target.GetComponent<CharacterStats>() == null)
        {
            return;
        }
        if (isServer)
        {
            if (healed.Contains(target))
            {
                return;
            }
            target.GetComponent<CharacterStats>().Heal(healAmount);
            healed.Add(target);
        }
    }
   
}
