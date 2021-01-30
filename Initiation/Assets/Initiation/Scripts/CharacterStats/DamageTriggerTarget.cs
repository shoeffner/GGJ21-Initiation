using UnityEngine;
using Mirror;


[RequireComponent(typeof(Collider))]
public class DamageTriggerTarget : NetworkBehaviour
{
    public int damage = 1;
    
    void OnTriggerEnter(Collider collider)
    {
        print($"DamageTriggerTarget {name} hits {collider.name} OnTriggerEnter");
        CharacterStats characterStats = collider.gameObject.GetComponent<CharacterStats>();
        if (characterStats != null)
        {
            characterStats.TakeDamage(damage);
        }
    }
}
